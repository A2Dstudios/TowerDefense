using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Splines;
using static UnityEngine.Rendering.HableCurve;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GameObject _preview;
    [SerializeField] private Transform _pathsRoot;
    [SerializeField] private LayerMask _towerLayer;

    private const float RoadWidth = 0.5f;

    private Camera _cam;
    private PlayerInputs _input;

    private bool _placing;
    private bool _canPlace;
    private Vector3 _mouseWorldPos;

    private TowerBase _towerToUpgrade;
    private bool _canUpgrade;

    private TowerBase _selectedTower;
    private float _selectedTowerRadius;
    private SpriteRenderer[] _previewSprites;

    private LineRenderer _previewRangeLine;

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

        _previewRangeLine = _preview.GetComponent<LineRenderer>();
        _previewRangeLine.useWorldSpace = true;
        _previewRangeLine.loop = true;
        _previewRangeLine.enabled = false;
    }

    private void OnDestroy()
    {
        _input.Player.Disable();

        _input.Player.LClick.performed -= PlaceTower;
    }

    void Update()
    {
        ShowPreview();
        DrawPreviewRange();
    }

    private void ShowPreview()
    {
        _mouseWorldPos = _cam.ScreenToWorldPoint(_input.Player.Mouse.ReadValue<Vector2>());
        _mouseWorldPos.z = 0;

        _preview.transform.position = _mouseWorldPos;

        if (_selectedTower == null) return;

        // check si se puede mejorar una torre
        Collider2D[] hits = Physics2D.OverlapCircleAll(_mouseWorldPos, _selectedTowerRadius, _towerLayer);

        bool blocked = false;
        bool touchingTower = false;

        foreach(Collider2D hit in hits)
        {
            if (hit.isTrigger) continue;

            if (!hit.transform.parent.TryGetComponent(out TowerBase tower)) continue;

            touchingTower = true;

            if (tower.TowerType == _selectedTower.TowerType)
            {
                _canUpgrade = true;
                _towerToUpgrade = tower;
                blocked = false;
                break;
            }
            else
            {
                blocked = true;
            }
        }

        if (!touchingTower)
        {
            _canUpgrade = false;
            _towerToUpgrade = null;
        }

        if (!blocked && !touchingTower)
        {
            float minDist = float.MaxValue;
            foreach (SplineContainer path in _paths)
            {
                float3 relative = _mouseWorldPos - path.gameObject.transform.position;
                float dist = SplineUtility.GetNearestPoint(path.Spline, relative, out float3 nearest, out float t);

                if (dist < minDist) minDist = dist;
            }

            if (minDist <= _selectedTowerRadius + RoadWidth)
            {
                blocked = true;
            }
        }

        _canPlace = _placing && !blocked;

        foreach (SpriteRenderer sprite in _previewSprites)
        {
            sprite.color = _canPlace ? Color.white : new Color(1f, 0f, 0f, 0.7f);
            
            sprite.color = _canUpgrade ? new Color(1f, 1f, 0f, 0.7f) : sprite.color;
        }
    }

    private const int CircleSegments = 33;
    private void DrawPreviewRange()
    {
        bool draw = _selectedTower != null;

        _previewRangeLine.enabled = draw;

        if (!draw) return;

        float radius = _selectedTower.AttackRange;

        _previewRangeLine.positionCount = CircleSegments;

        Vector3[] points = new Vector3[CircleSegments];

        float angleStep = (2f * Mathf.PI) / CircleSegments;

        for (int i = 0; i < CircleSegments; i++)
        {
            float currentAngle = i * angleStep;
            float a = Mathf.Cos(currentAngle) * radius;
            float b = Mathf.Sin(currentAngle) * radius;

            points[i] = _mouseWorldPos + new Vector3(a, b, 0f);
        }

        _previewRangeLine.SetPositions(points);
    }

    private void PlaceTower(InputAction.CallbackContext context)
    {
        if (_selectedTower == null) return;
        if (!_canPlace) return;
        _placing = false;

        if (_canUpgrade) _towerToUpgrade.Upgrade();
        else Instantiate(_selectedTower, _mouseWorldPos, Quaternion.identity);
        
        _preview.SetActive(false);
        Destroy(_preview.transform.GetChild(0).gameObject);
        _selectedTower = null;
        _towerToUpgrade = null;
        _canUpgrade = false;
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
