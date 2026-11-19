using UnityEngine;
using UnityEngine.VFX;

public class Bomb : ProjectileBase
{
    [Header("Bomb")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private VisualEffect _explosionVfx;

    private float _range = 1f;

    protected override void OnHit()
    {
        _explosionVfx.SetFloat("radius", _range);
        _explosionVfx.Play();

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _range, _enemyLayer);

        foreach(Collider2D hit in hits)
        {
            if (!hit.TryGetComponent(out EnemyBase enemy)) continue;

            enemy.TakeDamage(_damage);
        }
    }

    protected override void OnInit(WeaponData wd)
    {
        if (wd is BombWeaponData bombData) _range = bombData.explosionRadius;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gold;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

}
