
using UnityEngine;
using UnityEngine.Splines;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] protected float _maxHP = 10f;
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _xp = 1f;

    [Header("Movement")]
    [SerializeField] protected PathScript _path;

    protected float _hp;
    protected bool _isDead;

    private SplineContainer _pathSC;
    private float _pathProgress = 0f;

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
        if (_path != null)
        {
            _pathSC = _path.GetComponent<SplineContainer>();
        }
    }

    protected virtual void Update()
    {
        Move();
    }

    protected virtual void Move()
    {
        if (_pathSC == null) return;

        float pathLength = _pathSC.CalculateLength();

        if (pathLength <= 0f) return;

        _pathProgress += Time.deltaTime * _speed;

        while (_pathProgress >= pathLength && _path.getNext() != null)
        {
            _pathProgress -= pathLength;

            _path = _path.getNext();
            _pathSC = _path.GetComponent<SplineContainer>();

            if (_pathSC == null) return;

            pathLength = _pathSC.CalculateLength();

            if (pathLength <= 0f) return;
        }

        if (_pathProgress >= pathLength)
        {
            _pathProgress = pathLength;
        }

        transform.position = _pathSC.EvaluatePosition(
            _pathProgress / pathLength
        );
    }

    public virtual void TakeDamage(float damage)
    {
        if (_isDead) return;

        _hp -= damage;

        if (_hp <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (_isDead) return;

        _isDead = true;

        // TODO: Give XP
        // TODO: Death event

        Destroy(gameObject);
    }
}
