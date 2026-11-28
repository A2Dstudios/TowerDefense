using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;

public class Egg : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float _maxHP = 100f;
    [SerializeField] private float _regenPerSecond = 0.5f;
    [SerializeField] private float _regenDelay = 3f;

    [Header("Damage")]
    [SerializeField] private float _damagePerSecond = 4f;
    [SerializeField] private float _tickInterval = 0.25f;
    [SerializeField] private VisualEffect _vfx;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _hpText;

    private readonly List<EnemyBase> _enemiesInCenter = new List<EnemyBase>();

    private float _hp;
    private float _tickTimer;
    private float _timeSinceDamage;
    private bool _isDead;

    public static Action OnGameOver;

    public float MaxHp => _maxHP;
    public float Hp => _hp;

    public static Egg Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        _hp = _maxHP;
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void Update()
    {
        if (_isDead) return;

        // quitar los que esten muertos
        _enemiesInCenter.RemoveAll(e => e == null);

        _tickTimer += Time.deltaTime;
        _timeSinceDamage += Time.deltaTime;

        // cada tick el huevo pega a todos los del centro y ellos le quitan vida a el
        if (_tickTimer >= _tickInterval)
        {
            _tickTimer -= _tickInterval;

            if(_enemiesInCenter.Count > 0) _vfx.Play();

            foreach (EnemyBase enemy in _enemiesInCenter)
            {
                TakeDamage(_damagePerSecond * _tickInterval);
                enemy.TakeDamage(_damagePerSecond * _tickInterval);

                if (_isDead) return;
            }
        }

        // si lleva un rato sin recibir daño se regenera poco a poco
        if (_timeSinceDamage >= _regenDelay && _hp < _maxHP)
        {
            _hp = Mathf.Min(_hp + _regenPerSecond * Time.deltaTime, _maxHP);
            UpdateHealthBar();
        }
    }

    // lo llama el enemigo cuando entra en un carril del centro
    public void Register(EnemyBase enemy)
    {
        if (!_enemiesInCenter.Contains(enemy)) _enemiesInCenter.Add(enemy);
    }

    private void TakeDamage(float damage)
    {
        if (_isDead || damage <= 0f) return;

        _hp = Mathf.Max(_hp - damage, 0f);
        _timeSinceDamage = 0f;

        UpdateHealthBar();

        if (_hp <= 0f) Die();
    }

    // la barra de vida aqui y no en la UI pq es un canvas en worldspace
    private void UpdateHealthBar()
    {
        _hpText.text = Mathf.CeilToInt((_hp/_maxHP) * 100) + "%";
    }

    private void Die()
    {
        _isDead = true;

        OnGameOver?.Invoke();
        GameManager.Instance.Pause();
    }
}