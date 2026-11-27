using UnityEngine;

public class Boniato : EnemyBase
{
    [SerializeField] private EnemyBase _partEnemyPrefab;   
    [SerializeField] private SpriteRenderer[] _parts;    

    private int _nextPart = 0;

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        if (_isDead) return;

        float hpPerPart = _maxHP / (_parts.Length + 1);

        while (_nextPart < _parts.Length && _hp <= _maxHP - hpPerPart * (_nextPart + 1))
        {
            ReleasePart(_parts[_nextPart]);
            _nextPart++;
        }
    }

    private void ReleasePart(SpriteRenderer part)
    {
        part.gameObject.SetActive(false);

        EnemyBase enemy = Instantiate(_partEnemyPrefab, part.transform.position, Quaternion.identity);
        enemy.transform.SetParent(transform.parent, true);

        enemy._path = _path;
        enemy._pathProgress = _pathProgress;

        enemy.GetComponentInChildren<SpriteRenderer>().sprite = part.sprite;
    }
}