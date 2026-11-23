using System.Collections;
using System.IO;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevelPhysics2D;
using UnityEngine.Splines.ExtrusionShapes;

public class TowerMenu : MonoBehaviour
{

    [SerializeField] private Sprite _bgSprite;
    [SerializeField] private TowerPlacer _towerPlacer;
    [SerializeField] private TowerBase[] _towerPrefabs;

    private ArrayList _towerButtons = new ArrayList();
    private Camera _cam;
    private PlayerInputs _input;
    private Vector3 _mouseWorldPos;
    private float _menuPop = 0;
    private bool _menuOpen = false;
    private int _newTower = -1;

    [SerializeField] private float _popupTime = 0.25f;

    [SerializeField] private float _menuRadius = 1.25f;

    private ArrayList _towerQuantities = new ArrayList();

    private void Awake()
    {
        _input = new PlayerInputs();
        _input.Player.Enable();
        _input.Player.RClick.performed += OpenMenu;
        _input.Player.LClick.performed += SelectTower;

        int numPos = 0;
        foreach (TowerBase tb in _towerPrefabs)
        {
            _towerQuantities.Add(5);
            GameObject go = new GameObject();
            Transform goSprite = Instantiate(tb.VisualTransform);
            goSprite.parent = go.transform;
            go.transform.parent = transform;
            float posAngle = 2*math.PI/_towerPrefabs.Length*numPos;
            go.transform.localPosition = new Vector3(math.cos(posAngle),math.sin(posAngle),0) * _menuRadius;
            _towerButtons.Add(go);
            numPos++;

            SpriteRenderer bgSprite = go.AddComponent<SpriteRenderer>();
            bgSprite.sprite = _bgSprite;
            bgSprite.sortingLayerName = "UI";
            bgSprite.sortingOrder = -999;
            foreach(SpriteRenderer sprite in go.GetComponentsInChildren<SpriteRenderer>())
            {
                sprite.sortingLayerName = "UI";
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cam = Camera.main;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (_menuOpen)
        {
            if (_menuPop < 1)
                _menuPop += Time.deltaTime/_popupTime;
            else if (_menuPop > 1)
                _menuPop = 1;
        } else
        {
            if (_menuPop > 0)
                _menuPop -= Time.deltaTime/_popupTime;
            else if (_menuPop < 0)
                _menuPop = 0;
        }

        transform.localScale = new Vector3(_menuPop, _menuPop, 1);

        if (_newTower != -1)
        {
            _towerPlacer.SelectTower(_towerPrefabs[_newTower]);
            _towerQuantities[_newTower] = (int)_towerQuantities[_newTower] - 1;
            _newTower = -1;
        }
    }

    private void OnDestroy()
    {
        _input.Player.RClick.performed -= OpenMenu;
        _input.Player.LClick.performed -= SelectTower;
    }

    void OpenMenu(InputAction.CallbackContext context)
    {
        if (_towerPlacer._selectedTower != null) {
            addTower(_towerPlacer._selectedTower);
            _towerPlacer.SelectTower(null);
        }

        _menuOpen = !_menuOpen;
        if (_menuOpen)
        {
            _mouseWorldPos = _cam.ScreenToWorldPoint(_input.Player.Mouse.ReadValue<Vector2>());
            _mouseWorldPos.z = 0;
            transform.position = _mouseWorldPos;
            for (int i = 0; i < _towerButtons.Count; i++)
            {
                GameObject button = (GameObject) _towerButtons[i];
                SpriteRenderer[] sprites = button.GetComponentsInChildren<SpriteRenderer>();
                Color imageColor = Color.white;
                if ((int) _towerQuantities[i] == 0)
                    imageColor = Color.gray;

                foreach (SpriteRenderer sprite in sprites)
                    sprite.color = imageColor;
            }
        }
    }

    void SelectTower(InputAction.CallbackContext context)
    {
        if (_menuOpen && _menuPop == 1)
        {
            _mouseWorldPos = _cam.ScreenToWorldPoint(_input.Player.Mouse.ReadValue<Vector2>());
            _mouseWorldPos.z = 0;
            for (int i = 0; i < _towerButtons.Count; i++)
            {
                GameObject button = (GameObject)_towerButtons[i];
                if ((button.transform.position - _mouseWorldPos).magnitude < 0.5 && (int) _towerQuantities[i] > 0)
                {
                    _newTower = i;
                }
            }
            _menuOpen = false;
        }
    }

    public void addTower(TowerBase tower)
    {
        for (int i = 0; i < _towerPrefabs.Length; i++)
            if (_towerPrefabs[i] == tower)
                _towerQuantities[i] = (int)_towerQuantities[i] + 1;
    }
}
