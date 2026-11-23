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

        // que se gire cada vez que ataca para que parezca que baila lalalalala soy una bailarina mira que bonitaaa
        Vector3 scale = _visualTransform.localScale;
        _visualTransform.localScale = new Vector3(-scale.x, scale.y, scale.z);

        _singEffect.SetFloat("radius", AttackRange);
        _singEffect.Play();
    }
}
