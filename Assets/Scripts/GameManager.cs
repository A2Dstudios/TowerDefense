using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("Towers")]
    [SerializeField] private TowerBase[] _towerPrefabs;

    [Header("Leveling")]
    [SerializeField] private float _baseLevelXP = 10f;
    [SerializeField] private float _multiplierXP = 1.25f;
    [SerializeField] private float _addXP = 5f;

    private int _playerLVL = 0;
    private float _playerXP = 0f;
    private float _nextLevelXP;
    public Action<float, float> OnXpUpdate;
    public Action OnLevelUp;
    private bool _paused;
    private float _gameSpeed = 1f;

    [Header("Map")]
    [SerializeField] private Transform _objective;
    public Vector3 Objective => _objective.position;

    public TowerBase[] TowerPrefabs => _towerPrefabs;
    public bool IsPaused => _paused;

    public static GameManager Instance { get; private set; }

    private PlayerInputs _inputs;

    private void Awake()
    {
        _inputs = new PlayerInputs();
        _inputs.Player.Enable();
        _inputs.Player.speed.performed += ChangeSpeed;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;

        _nextLevelXP = _baseLevelXP;

        DontDestroyOnLoad(gameObject);
    }

    public void GetXp(float xp)
    {
        _playerXP += xp;

        while(_playerXP >= _nextLevelXP)
        {
            LevelUP();
        }

        OnXpUpdate?.Invoke(_playerXP, _nextLevelXP);
    }

    private void LevelUP()
    {
        _playerXP -= _nextLevelXP;

        _playerLVL++;

        _nextLevelXP =  _nextLevelXP * _multiplierXP + _addXP;

        OnLevelUp?.Invoke();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        _paused = true;
    }

    public void UnPause()
    {
        Time.timeScale = _gameSpeed;
        _paused = false;
    }

    private void ChangeSpeed(InputAction.CallbackContext context)
    {
        if (_gameSpeed == 1f) _gameSpeed = 3f;
        else _gameSpeed = 1f;

        Time.timeScale = _gameSpeed;
    }

    private void OnDestroy()
    {
        _inputs.Player.Disable();
        _inputs.Player.speed.performed -= ChangeSpeed;
    }
}
