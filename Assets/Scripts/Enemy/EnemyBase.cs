using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHP = 10f;
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _xp = 1f;

    protected float _hp;
    protected bool _isDead;

    protected Vector3 _objective;

    public float MaxHp => _maxHP;
    public float Hp => _hp;
    public float Speed => _speed;
    public float XP => _xp;

    protected virtual void Awake()
    {
        _hp = _maxHP;
    }

    protected virtual void Start()
    {
        _objective = GameManager.Instance.Objective;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _objective,
            _speed * Time.deltaTime
        );
    }

    public virtual void TakeDamage(float damage)
    {
        if (_isDead) return;

        _hp -= damage;

        if (_hp <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (_isDead) return;

        _isDead = true;

        // dar XP
        // evento de muerte

        Destroy(gameObject);
    }
}