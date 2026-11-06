using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private EnemyBase _enemy;
    [SerializeField] private Transform _pivot;

    private void Update()
    {
        _pivot.localScale = new Vector3(_enemy.Hp / _enemy.MaxHp, 1f, 1f);
    }
}