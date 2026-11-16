using UnityEngine;
using UnityEngine.VFX;

public class Bomb : ProjectileBase
{
    [Header("Bomb")]
    [SerializeField] private float _range;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private VisualEffect _explosionVfx;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gold;
        Gizmos.DrawWireSphere(transform.position, _range);
    }

    protected override void DeleteBullet()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
}
