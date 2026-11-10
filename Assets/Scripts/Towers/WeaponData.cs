using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "A2D/WeaponDawa")]
public class WeaponData : ScriptableObject
{
    public GameObject projectilePrefab;
    public float fireRate = 1f;
    public float projectileSpeed = 10f;
    public float attackDamage = 1f;
    public int piercing = 0;
}
