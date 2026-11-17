using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GameObject _preview;
    [SerializeField] private Transform _pathsRoot;
    [SerializeField] private LayerMask _obstacleLayer;

    private const float RoadWidth = 0.5f;

    private Camera _cam;
    private PlayerInputs _input;

    private bool _placing;
    private bool _canPlace;
    private Vector3 _mouseWorldPos;

    private TowerBase _selectedTower;
    private float _selectedTowerRadius;
    private SpriteRenderer[] _previewSprites;

    private SplineContainer[] _paths;

    private void Awake()
    {
        _input = new PlayerInputs();
        _input.Player.Enable();

        _input.Player.LClick.performed += PlaceTower;
    }

    void Start()
    {
        _cam = Camera.main;

        _paths = _pathsRoot.GetComponentsInChildren<SplineContainer>();
    }

    private void OnDestroy()
    {
        _input.Player.Disable();

        _input.Player.LClick.performed -= PlaceTower;
    }

    void Update()
    {
        ShowPreview();
    }

    private void ShowPreview()
    {
        _mouseWorldPos = _cam.ScreenToWorldPoint(_input.Player.Mouse.ReadValue<Vector2>());
        _mouseWorldPos.z = 0;

        _preview.transform.position = _mouseWorldPos;

        bool blocked = Physics2D.OverlapCircle(_mouseWorldPos, _selectedTowerRadius, _obstacleLayer);

        if (!blocked)
        {
            float minDist = float.MaxValue;
            foreach (SplineContainer path in _paths)
            {
                float3 relativo = _mouseWorldPos - path.gameObject.transform.position;
                float dist = SplineUtility.GetNearestPoint(path.Spline, relativo, out float3 nearest, out float t);

                if (dist < minDist) minDist = dist;                
            }

            if (minDist <= _selectedTowerRadius + RoadWidth)
            {
                blocked = true;
            }
        }

        _canPlace = _placing && !blocked;

        if (_selectedTower == null) return;
        foreach(SpriteRenderer sprite in _previewSprites)
        {
            sprite.color = _canPlace ? Color.white : new Color(1f, 0f, 0f, 0.8f);
        }
    }

    private void PlaceTower(InputAction.CallbackContext context)
    {
        if (!_canPlace) return;
        _placing = false;

        TowerBase tower = Instantiate(_selectedTower, _mouseWorldPos, Quaternion.identity);
        
        _preview.SetActive(false);
        Destroy(_preview.transform.GetChild(0).gameObject);
        _selectedTower = null;
    }

    public void SelectTower(TowerBase tower)
    {
        _placing = true;
        _selectedTower = tower;

        Transform preview = Instantiate(_selectedTower.VisualTransform, _preview.transform);
        
        CircleCollider2D col = preview.GetComponent<CircleCollider2D>();
        _selectedTowerRadius = col.radius;
        col.enabled = false;

        _previewSprites = preview.GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in _previewSprites) sprite.sortingOrder = 100;

        _preview.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = _canPlace ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_mouseWorldPos, _selectedTowerRadius);
    }
}
