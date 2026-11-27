using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class BallerinaTower : TowerBase
{
    [Header("Ballerina")]
    [SerializeField] private VisualEffect _singEffect;

    protected override void Shoot(EnemyBase target)
    {
        foreach(EnemyBase e in _enemiesInRange)
        {
            // se usa el damage de 0 a 1 para el efecto de slow
            e.SlowDown(_weaponData.attackDamage);     
        }

        _singEffect.SetFloat("radius", AttackRange);
        _singEffect.Play();

        _animator.SetTrigger("shoot");
    }
}
