using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TargetingMode { FIRST, CLOSEST, STRONGEST, WEAKEST }

public class TowerBase : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float _attackRange = 5f;

    [Header("Shooting")]
    [SerializeField] private TargetingMode _targetingMode;
    [SerializeField] private WeaponData _weaponData;
    [SerializeField] private Transform _objective;
    [SerializeField] private Transform _firePoint;

    private readonly List<EnemyBase> _enemiesInRange = new();

    private float _nextFireTime;

    private void Update()
    {
        EnemyBase target = GetTarget();

        if (target == null) return;

        if (Time.time < _nextFireTime) return;

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
                return _enemiesInRange.OrderBy(e => Vector2.Distance(e.transform.position, _objective.position)).FirstOrDefault();

            case TargetingMode.CLOSEST:
                return _enemiesInRange .OrderBy(e => Vector2.Distance(e.transform.position,transform.position)).FirstOrDefault();

            case TargetingMode.STRONGEST:
                return _enemiesInRange.OrderByDescending(e => e.MaxHp).FirstOrDefault();

            case TargetingMode.WEAKEST:
                return _enemiesInRange.OrderBy(e => e.Hp).FirstOrDefault();
        }

        return null;
    }

    private void Shoot(EnemyBase target)
    {
        Vector2 direction = ((Vector2)target.transform.position - (Vector2)_firePoint.position).normalized;

        GameObject projectileObject = Instantiate(_weaponData.projectilePrefab, _firePoint.position, Quaternion.identity);

        projectileObject.GetComponent<ProjectileBase>().Init(_weaponData, direction);
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

}
