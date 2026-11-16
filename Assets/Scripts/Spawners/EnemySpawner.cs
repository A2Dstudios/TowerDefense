using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase[] _enemyPrefabs;
    [SerializeField] private EnemyBase[] _bossPrefabs;
    [SerializeField] private PathScript[] _spawnPoints;
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private int _waveInterval = 15;
    [SerializeField] private int _initialEnemies = 2;
    [SerializeField] private int _enemiesPerWave = 3;
    [SerializeField] private int _wavesPerRound = 3;
    private int _wave = 0;
    private int _round = 1;
    private ArrayList _waveEnemies = new ArrayList();
    private bool _roundBoss = false;

    private float _nextSpawn;
    private float _nextWave;

    private void Update()
    {
        // Si la onada està en curs, s'envïen enemics
        if (_waveEnemies.Count > 0)
        {
            _nextSpawn += Time.deltaTime;
            if (_nextSpawn >= _spawnInterval)
            {
                if ((int) _waveEnemies[0] != -1)
                {
                    int enemiesSent = Random.Range(Mathf.Max(0, Mathf.Min(_round - 5, _spawnPoints.Length-1)), Mathf.Min(_round, _spawnPoints.Length-1, _waveEnemies.Count-1));
                    Debug.Log("Surten "+(enemiesSent+1)+" enemics!");
                    for (int i = 0; i <= enemiesSent; i++)
                    {
                        int enemyIndex = Random.Range(0, _waveEnemies.Count - 1);
                        EnemyBase prefab = Instantiate(_enemyPrefabs[(int) _waveEnemies[enemyIndex]]);
                        prefab._path = _spawnPoints[i];
                        _waveEnemies.RemoveAt(enemyIndex);
                    }
                    if (_waveEnemies.Count == 0 && _roundBoss)
                    {
                        _waveEnemies.Add(-1);
                        _roundBoss = false;
                    }
                } else
                {
                    _waveEnemies.Clear();
                    EnemyBase prefab = Instantiate(_bossPrefabs[Random.Range(0, _bossPrefabs.Length)]);
                    prefab._path = _spawnPoints[Random.Range(0, Mathf.Max(0, Mathf.Min(_round - 5, _spawnPoints.Length)))];
                }
                _nextSpawn = 0;
            }
        }
        // En cas contrari, s'espera un temps per la següent onada
        else
        {
            _nextWave += Time.deltaTime;
            if (_nextWave >= _waveInterval)
            {
                int typesInWave = Mathf.Min(_round, _enemyPrefabs.Length);
                for (int i = 0; i < typesInWave; i++)
                {
                    int numEnemies = _initialEnemies * (_round - i) + _enemiesPerWave * _wave;
                    for (int j = 0; j < numEnemies; j++)
                    {
                        _waveEnemies.Add(i);
                    }
                }
                _nextWave = 0;
                _wave++;
                if (_wave == _wavesPerRound)
                {
                    _wave = 0;
                    _round++;
                    if (typesInWave >= _enemyPrefabs.Length && typesInWave >= _spawnPoints.Length && _wave == _wavesPerRound) {
                        _roundBoss = true;
                    }
                }
                Debug.Log("Ronda " + _round + ": Venen "+_waveEnemies.Count+" enemics de l'onada " + _wave);
            }
        }
    }
}