using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHP = 10f;
    protected float _hp;
    protected bool _isDead;

    public float MaxHp => _maxHP;
    public float Hp => _hp;

    private void Awake()
    {
        _hp = _maxHP;
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _hp -= damage;

        if(_hp <= 0)
        {
            _isDead = true;
            // evento de ondeath
        }
    }
}
