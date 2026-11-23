using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyBase[] _enemyPrefabs;
    [SerializeField] private EnemyBase[] _bossPrefabs;
    [SerializeField] private PathScript[] _spawnPoints;
    [SerializeField] private float _spawnInterval = 1f;
    [SerializeField] private int _waveInterval = 15;
    [SerializeField] private int _wavesPerRound = 3;
    [SerializeField] private float _initialEnergy = 1f;
    [SerializeField] private float _energyPerWave = 0.5f;
    private int _wave = 0;
    private ArrayList _waveEnemies = new ArrayList();
    private bool _roundBoss = false;

    private float _nextSpawn;
    private float _nextWave;

    private void Update()
    {
        // Si la onada està en curs, s'envïen enemics
        if (_waveEnemies.Count > 0 || _roundBoss)
        {
            _nextSpawn += Time.deltaTime;
            if (_nextSpawn >= _spawnInterval)
            {
                if (_waveEnemies.Count > 0)
                {
                    int enemiesSent = Random.Range(Mathf.Max(0, Mathf.Min(_wave/_wavesPerRound - 5, _spawnPoints.Length-1, _waveEnemies.Count - 1)), Mathf.Min(_wave / _wavesPerRound, _spawnPoints.Length-1, _waveEnemies.Count-1));
                    for (int i = 0; i <= enemiesSent; i++)
                    {
                        int enemyIndex = Random.Range(0, _waveEnemies.Count - 1);
                        EnemyBase prefab = Instantiate(_enemyPrefabs[(int) _waveEnemies[enemyIndex]]);
                        prefab._path = _spawnPoints[i];
                        prefab.transform.position = prefab._path.GetComponent<SplineContainer>().EvaluatePosition(0);
                        prefab.transform.parent = transform;
                        _waveEnemies.RemoveAt(enemyIndex);
                    }
                } else
                {
                    EnemyBase prefab = Instantiate(_bossPrefabs[Random.Range(0, _bossPrefabs.Length)]);
                    prefab._path = _spawnPoints[Random.Range(0, Mathf.Max(0, Mathf.Min(_wave / _wavesPerRound - 5, _spawnPoints.Length)))];
                    _roundBoss = false;
                }
                _nextSpawn = 0;
            }
        }
        // En cas contrari, s'espera un temps per la següent onada
        else if (transform.childCount == 0)
        {
            _nextWave += Time.deltaTime;
            if (_nextWave >= _waveInterval)
            {
                float waveXP = _initialEnergy+_energyPerWave*_wave;
                while (waveXP > 0)
                {
                    int worstEnemy = -1;
                    foreach (EnemyBase enemy in _enemyPrefabs)
                    {
                        if (enemy.XP <= waveXP)
                            worstEnemy++;
                    }
                    int newEnemy = Random.Range(0, worstEnemy);
                    _waveEnemies.Add(newEnemy);
                    waveXP -= _enemyPrefabs[newEnemy].XP;
                }
                int typesInWave = Mathf.Min(_wave / _wavesPerRound, _enemyPrefabs.Length);
                _nextWave = 0;
                _wave++;
                if (typesInWave >= _enemyPrefabs.Length && typesInWave >= _spawnPoints.Length && _wave % _wavesPerRound == 0) {
                    _roundBoss = true;
                }
                Debug.Log("Onada " + _wave + ": Venen "+_waveEnemies.Count+" enemics");
            }
        }
    }
}