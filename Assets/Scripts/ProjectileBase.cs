using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBase : MonoBehaviour
{
    private float _damage = 10f;
    private float _speed = 10f;

    private Rigidbody2D _rb;

    public void Init(float damage, float speed, Vector2 direction)
    {
        _rb = GetComponent<Rigidbody2D>();

        _damage = damage;
        _speed = speed;

        _rb.linearVelocity = direction * _speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_damage);
        }
    }
}
