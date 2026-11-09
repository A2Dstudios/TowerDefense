using System.Threading;
using UnityEngine;
using UnityEngine.Splines;

public class EnemyBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected float _maxHP = 10f;
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected PathScript _path;
    private SplineContainer _pathSC;
    protected float _hp;
    protected bool _isDead;
    private float _pathProgress = 0f;

    protected Vector3 _objective;

    public float MaxHp => _maxHP;
    public float Hp => _hp;

    private void Awake()
    {
        _hp = _maxHP;

    }

    private void Start()
    {
        _objective = GameManager.Instance.Objective;
        _pathSC = _path.GetComponent<SplineContainer>();
    }

    private void Update()
    {
        // TODO: movimiento avanzado con rigidbody
        _pathProgress += Time.deltaTime*_speed;

        if (_pathProgress > _pathSC.CalculateLength())
        {
            if (_path.getNext() == null)
            {
                _pathProgress = _pathSC.CalculateLength();
            } else
            {

                _pathProgress -= _pathSC.CalculateLength();
                _path = _path.getNext();
                _pathSC = _path.GetComponent<SplineContainer>();
                transform.position = _pathSC.EvaluatePosition(_pathProgress / _pathSC.CalculateLength());
            }
        } else
        {
            transform.position = _pathSC.EvaluatePosition(_pathProgress / _pathSC.CalculateLength());
        }
    }


    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _hp -= damage;

        if(_hp <= 0)
        {
            _isDead = true;
            Destroy(gameObject);
            // TODO: evento de ondeath y cosas de muerte, quitar destroy
        }
    }

}
