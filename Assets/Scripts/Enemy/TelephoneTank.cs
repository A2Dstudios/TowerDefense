
using UnityEngine;

public class TelephoneTank : EnemyBase
{
    [SerializeField] private float _armor = 1f;
    [SerializeField] private float _minimumDamage = 0.25f;

    public override void TakeDamage(float damage)
    {
        float finalDamage = Mathf.Max(damage - _armor, _minimumDamage);

        base.TakeDamage(finalDamage);
    }
}
    