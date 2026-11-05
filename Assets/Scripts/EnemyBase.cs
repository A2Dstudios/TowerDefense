using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHP = 10f;
    [SerializeField] protected float _speed = 5f;
    protected float _hp;
    protected bool _isDead;

    protected Vector3 _objective;

    public float MaxHp => _maxHP;
    public float Hp => _hp;

    private void Awake()
    {
        _hp = _maxHP;

        _objective = GameManager.Instance.Objective;
    }

    private void Update()
    {
        // TODO: movimiento avanzado con rigidbody
        transform.position = Vector3.MoveTowards(transform.position, _objective, _speed * Time.deltaTime);
    }


    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _hp -= damage;
        print(_hp);
        if(_hp <= 0)
        {
            _isDead = true;
            Destroy(gameObject);
            // TODO: evento de ondeath y cosas de muerte, quitar destroy
        }
    }

}
