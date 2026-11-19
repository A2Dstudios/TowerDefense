using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public enum TargetingMode { FIRST, CLOSEST, STRONGEST, WEAKEST }

public class TowerBase : MonoBehaviour
{
    [Header("Tower")]
    [SerializeField] protected TowerType _type;

    [Header("Detection")]
    [SerializeField] private CircleCollider2D _rangeCollider;

    [Header("Shooting")]
    [SerializeField] private TargetingMode _targetingMode;
    [SerializeField] protected WeaponData _weaponData;
    [SerializeField] private Transform[] _firePoint;
    [SerializeField] private Transform _visualTransform;
    [SerializeField] private bool _faceEnemy = true;

    [Header("VFX")]
    [SerializeField] private VisualEffect _upgradeEffect;
    [SerializeField] private DamageFlash _flashScript;

    private readonly List<EnemyBase> _enemiesInRange = new();

    private float _nextFireTime = 0f;

    private Vector3 _objective;

    private int _tier = 0;

    public Transform VisualTransform => _visualTransform;
    public TowerType TowerType => _type;

    // setter para actualizar el radio del collider automaticamente
    public float AttackRange
    {
        get => _weaponData.attackRange;
        set
        {
            _rangeCollider.radius = value;
        }
    }

    private void Awake()
    {
        AttackRange = _weaponData.attackRange;
    }

    private void Start()
    {
        _objective = GameManager.Instance.Objective;
    }

    private void Update()
    {
        EnemyBase target = GetTarget();

        if (target == null) return;

        if (Time.time < _nextFireTime) return;

        if(_faceEnemy) Face(target);
        Shoot(target);

        _nextFireTime = Time.time + _weaponData.fireRate;
    }

    private EnemyBase GetTarget()
    {
        _enemiesInRange.RemoveAll(e => e == null);

        if (_enemiesInRange.Count == 0) return null;

        switch (_targetingMode)
        {
            case TargetingMode.FIRST:
                if (_objective == null) return null;
                return _enemiesInRange.OrderBy(e => Vector2.Distance(e.transform.position, _objective)).FirstOrDefault();

            case TargetingMode.CLOSEST:
                return _enemiesInRange .OrderBy(e => Vector2.Distance(e.transform.position,transform.position)).FirstOrDefault();

            case TargetingMode.STRONGEST:
                return _enemiesInRange.OrderByDescending(e => e.MaxHp).FirstOrDefault();

            case TargetingMode.WEAKEST:
                return _enemiesInRange.OrderBy(e => e.Hp).FirstOrDefault();
        }

        return null;
    }

    protected virtual void Shoot(EnemyBase target)
    {
        Vector2 firepoint = _firePoint[Random.Range(0, _firePoint.Length)].position;
        Vector2 direction = ((Vector2)target.transform.position - firepoint).normalized;

        GameObject projectileObject = Instantiate(_weaponData.projectilePrefab, firepoint, Quaternion.identity);

        projectileObject.GetComponent<ProjectileBase>().Init(_weaponData, direction);
    }

    private void Face(EnemyBase target)
    {
        Vector2 direction = ((Vector2)target.transform.position - (Vector2)transform.position).normalized;

        Vector3 scale = _visualTransform.localScale;
        if (direction.x < 0) _visualTransform.localScale = new Vector3(-Mathf.Abs(scale.x), scale.y, scale.z);
        if (direction.x > 0) _visualTransform.localScale = new Vector3(Mathf.Abs(scale.x), scale.y, scale.z);
    }

    public void Upgrade()
    {
        if (_weaponData.upgradedWeapon == null) return;

        _weaponData = _weaponData.upgradedWeapon;
        AttackRange = _weaponData.attackRange;

        _visualTransform.transform.localScale *= 1.1f;

        _upgradeEffect.Play();
        _flashScript.CallDamageFlash();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out EnemyBase enemy))
        {
            if(!_enemiesInRange.Contains(enemy)) _enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyBase enemy))
        {
            _enemiesInRange.Remove(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _weaponData.attackRange);
    }
}
