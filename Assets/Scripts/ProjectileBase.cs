using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBase : MonoBehaviour
{
    private float _damage = 10f;
    private float _speed = 10f;
    private int _piercing = 0;

    private Rigidbody2D _rb;

    public void Init(WeaponData wd, Vector2 direction)
    {
        _rb = GetComponent<Rigidbody2D>();

        _damage = wd.attackDamage;
        _speed = wd.projectileSpeed;
        _piercing = wd.piercing;

        _rb.linearVelocity = direction * _speed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out IDamageable target))
        {
            target.TakeDamage(_damage);

            _piercing--;
            if (_piercing < 0) Destroy(gameObject);
        }
    }
}
