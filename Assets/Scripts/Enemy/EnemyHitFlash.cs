
using System.Collections;
using UnityEngine;

public class EnemyHitFlash : DamageFlash
{
    private EnemyBase _enemy;

    private void OnEnable()
    {
        if (_enemy == null) _enemy = GetComponentInParent<EnemyBase>();
        _enemy.OnHealthChanged += CallDamageFlash;
    }

    private void OnDisable()
    {
        _enemy.OnHealthChanged -= CallDamageFlash;
    }
}
