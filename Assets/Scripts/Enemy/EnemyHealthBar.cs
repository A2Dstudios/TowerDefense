
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemy;
    [SerializeField] private Transform _pivot;

    private void OnEnable()
    {
        _enemy.OnHealthChanged += UpdateHealthBar;
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    private void OnDisable()
    {
        _enemy.OnHealthChanged -= UpdateHealthBar;
    }

    private void UpdateHealthBar()
    {
        float healthPercentage = _enemy.Hp / _enemy.MaxHp;

        _pivot.localScale = new Vector3( healthPercentage, 1f,1f);
    }
}
