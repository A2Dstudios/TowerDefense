using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class EnemyHitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Color _flashColor = new Color(1f, 1f, 1f, 0.4f);
    [SerializeField] private float _flashTime = 0.1f;

    private EnemyBase _enemy;
    private Color _baseColor;
    private float _lastHp;
    private float _flashEnd;

    private void Awake()
    {
        _enemy = GetComponent<EnemyBase>();
        if (_sprite == null) _sprite = GetComponentInChildren<SpriteRenderer>();
        _baseColor = _sprite.color;
    }

    private void Start()
    {
        _lastHp = _enemy.Hp;
    }

    private void Update()
    {
        if (_enemy.Hp < _lastHp) _flashEnd = Time.time + _flashTime;
        _lastHp = _enemy.Hp;

        _sprite.color = Time.time < _flashEnd ? _flashColor : _baseColor;
    }
}