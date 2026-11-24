
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Enemy Stats")]
    [SerializeField] protected float _maxHP = 10f;
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _xp = 1f;

    [Header("Movement")]
    [SerializeField] public PathScript _path;
    [SerializeField] public Transform _visualTransform;

    protected float _hp;
    protected bool _isDead;

    private SplineContainer _pathSC;
    [HideInInspector] public float _pathProgress = 0f;

    private float _speedMult = 1f;


    public float MaxHp => _maxHP;
    public float Hp => _hp;
    public float Speed => _speed;
    public float XP => _xp;

    public event Action OnHealthChanged;
    public event Action<float> OnDeath;

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

        _pathProgress += Time.deltaTime * _speed * _speedMult;

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

        Vector3 newPos = _pathSC.EvaluatePosition(
            _pathProgress / pathLength
        );

        // flip visuals
        float dir = Mathf.Sign(newPos.x - transform.position.x);
        Vector3 scale = _visualTransform.localScale.Abs();
        _visualTransform.localScale = new Vector3(dir * scale.x, scale.y, scale.z);

        transform.position = newPos;
    }

   public virtual void TakeDamage(float damage)
    {
        if (_isDead) return;

        _hp -= damage;

        if (_hp < 0f)
        {
            _hp = 0f;
        }

        OnHealthChanged?.Invoke();

        if (_hp <= 0f)
        {
            Die();
        }
    }

  
    protected virtual void Die()
    {
        if (_isDead) return;

        _isDead = true;

        OnDeath?.Invoke(_xp);
        GameManager.Instance.GetXp(_xp);

        Destroy(gameObject);
    }

    private Coroutine _slowCoroutine;
    public void SlowDown(float amount)
    {
        // si hay 2 bailarinas y una está más mejorada se aplica la mas fuerte
        if (amount > _speedMult) return;

        if (_slowCoroutine != null) StopCoroutine(_slowCoroutine);

        _slowCoroutine = StartCoroutine(SlowCoroutine(amount));
    }

    private IEnumerator SlowCoroutine(float amount)
    {
        _speedMult = amount;

        yield return new WaitForSeconds(0.5f);

        _speedMult = 1f;
    }
}
