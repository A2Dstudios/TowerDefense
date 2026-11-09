
using UnityEngine;

public class Furby : EnemyBase
{
    [SerializeField] private float _screamInterval = 4f;
    [SerializeField] private float _screamDuration = 1f;
    [SerializeField] private float _speedMultiplier = 2f;

    private float _timer;
    private float _originalSpeed;
    private bool _isScreaming;

    protected override void Awake()
    {
        base.Awake();

        _originalSpeed = _speed;
    }

    protected override void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _screamInterval && !_isScreaming)
        {
            Scream();
        }

        base.Update();
    }

    private void Scream()
    {
        _timer = 0f;
        _isScreaming = true;

        _speed = _originalSpeed * _speedMultiplier;

        Invoke(nameof(EndScream), _screamDuration);
    }

    private void EndScream()
    {
        _speed = _originalSpeed;
        _isScreaming = false;
    }
}
