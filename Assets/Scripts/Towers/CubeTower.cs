using UnityEngine;

public class CubeTower : TowerBase
{
    private int ProjectileAmount => _weaponData is MultiShotWeaponData data ? data.projectileAmount : 1;

    private bool odd = false;

    protected override void Shoot(EnemyBase target)
    {
        float step = 360f / ProjectileAmount;

        for (int i = 0; i < ProjectileAmount; i++)
        {
            float angle = step * i;
            if (odd) angle += step * 0.5f;

            Vector2 dir = Quaternion.Euler(0f, 0f, angle) * Vector2.up;

            GameObject projectileObject = Instantiate(_weaponData.projectilePrefab, transform.position, Quaternion.identity);
            projectileObject.GetComponent<ProjectileBase>().Init(_weaponData, dir, 0.4f);
        }

        odd = !odd;
    }
}
