
using System.Collections;
using UnityEngine;

public class EnemyHitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _flashTime = 0.1f;

    private EnemyBase _enemy;
    private Color _baseColor;
    private Coroutine _flashCoroutine;

    private void Awake()
    {
        _enemy = GetComponent<EnemyBase>();

        if (_sprite == null)
        {
            _sprite = GetComponentInChildren<SpriteRenderer>();
        }

        _baseColor = _sprite.color;
    }

    private void OnEnable()
{
    _enemy.OnHealthChanged += Flash;
}

private void OnDisable()
{
    _enemy.OnHealthChanged -= Flash;
}

private void Flash()
{
    if (_flashCoroutine != null)
    {
        StopCoroutine(_flashCoroutine);
    }

    _flashCoroutine = StartCoroutine(FlashRoutine());
}

    private IEnumerator FlashRoutine()
    {
        _sprite.color = _flashColor;

        yield return new WaitForSeconds(_flashTime);

        _sprite.color = _baseColor;

        _flashCoroutine = null;
    }
}
