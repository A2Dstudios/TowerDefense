using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase[] _enemyPrefabs;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _interval = 2f;

    private float _nextSpawn;

    private void Update()
    {
        if (Time.time < _nextSpawn) return;

        Transform point = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        EnemyBase prefab = _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
        Instantiate(prefab, point.position, Quaternion.identity);

        _nextSpawn = Time.time + _interval;
    }
}