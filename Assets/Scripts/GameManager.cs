using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Towers")]
    [SerializeField] private TowerBase[] _towerPrefabs;

    [Header("Leveling")]
    [SerializeField] private float _baseLevelXP = 10f;
    [SerializeField] private float _multiplierXP = 1.25f;

    private int _playerLVL = 0;
    private float _playerXP = 0f;
    private float _nextLevelXP;
    public Action<float, float> OnXpUpdate;
    public Action OnLevelUp;
    private bool _paused;

    [Header("Map")]
    [SerializeField] private Transform _objective;
    public Vector3 Objective => _objective.position;

    public TowerBase[] TowerPrefabs => _towerPrefabs;
    public bool IsPaused => _paused;

    public static GameManager Instance { get; private set; }
    

    private void Awake()
    {
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

        _nextLevelXP *= _multiplierXP;

        OnLevelUp?.Invoke();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        _paused = true;
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        _paused = false;
    }
}
