using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

/*  
    spawner de enemigos cada wave tiene una xp (energia) total y unos enemigos, y se van "comprando" aleatoriamente hasta gastarla
    le metimos los enemigos y las stats de las torres al claude para que haga los calculos de balancear el juego y repartirlo en un total de 30 rondas
    los caminos y enemigos se van desbloqueando y siempre se garantiza que se vean las cosas nuevas
    los enemigos mas fuertes se desbloquean como miniboss y mientras vas progresando se usan como enemigos normales
    al final de todo está el mr boniato que es super feo y muy fuerte
*/
public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public class EnemyEntry
    {
        public EnemyBase prefab;
        public int unlockWave = 1;

        // el peso reparte la xp de la wave, no la cantidad de enemigos
        public float weight = 1f;
        public float weightPerWave = 0f;

        public int introCount = 0;

        public int groupMin = 1;
        public int groupMax = 1;
        public float groupGrowth = 0f;
        public float groupSpacing = 0.4f;

        public float WeightAt(int wave)
        {
            if (wave < unlockWave) return 0f;
            return Mathf.Max(0f, weight + weightPerWave * (wave - unlockWave));
        }

        public int GroupSizeAt(int wave)
        {
            return UnityEngine.Random.Range(groupMin, groupMax + 1) + Mathf.FloorToInt(groupGrowth * (wave - unlockWave));
        }

        public float AvgGroupAt(int wave)
        {
            return Mathf.Max(1f, (groupMin + groupMax) * 0.5f + groupGrowth * (wave - unlockWave));
        }
    }

    private struct SpawnGroup
    {
        public EnemyEntry entry;
        public int count;
    }

    [Header("Enemies")]
    [SerializeField] private EnemyEntry[] _enemies;
    [SerializeField] private EnemyBase _bossPrefab;
    [SerializeField] private int _bossWave = 30;

    [Header("Paths")]
    [SerializeField] private PathScript[] _spawnPoints;
    [SerializeField] private int _startingPaths = 1;
    [SerializeField] private int[] _pathUnlockWaves = { 4, 7, 10, 13, 16, 21 };

    [Header("Energy")]
    [SerializeField] private float _initialEnergy = 4f;
    [SerializeField] private float _energyPerWave = 2.2f;
    [SerializeField] private float _energyGrowth = 0.56f;

    [Header("Timing")]
    [SerializeField] private float _baseSpawnDuration = 8f;
    [SerializeField] private float _spawnDurationPerWave = 0.95f;
    [SerializeField] private float _minGroupDelay = 0.15f;
    [SerializeField] private float _waveInterval = 1f;
    [SerializeField] private float _introPosition = 0.6f;

    private readonly List<PathScript> _lockedPaths = new();
    private readonly List<PathScript> _unlockedPaths = new();
    private readonly List<EnemyEntry> _pool = new();
    private readonly List<EnemyEntry> _affordable = new();

    private int _wave = 0;
    private bool _spawning = false;
    private bool _finished = false;
    private float _nextWave = 0f;

    public Action<int> OnWaveStart;
    public Action<Vector2> OnPathUnlocked;
    public Action OnBossSpawned;
    public Action OnAllWavesCleared;

    public int Wave => _wave;

    // xp de la wave = inicial + lineal + cuadratica
    public float GetWaveEnergy(int wave)
    {
        int w = wave - 1;
        return _initialEnergy + _energyPerWave * w + _energyGrowth * w * w;
    }

    public float GetSpawnDuration(int wave)
    {
        return _baseSpawnDuration + _spawnDurationPerWave * (wave - 1);
    }

    private void Awake()
    {
        _lockedPaths.AddRange(_spawnPoints);
    }

    private void Update()
    {
        if (_finished || _spawning) return;

        // mientras queden enemigos vivos (hijos) no empieza la siguiente
        if (transform.childCount > 0)
        {
            _nextWave = 0f;
            return;
        }

        _nextWave += Time.deltaTime;
        if (_nextWave < _waveInterval) return;
        _nextWave = 0f;

        if (_wave >= _bossWave)
        {
            _finished = true;
            OnAllWavesCleared?.Invoke();
            return;
        }

        _wave++;
        StartCoroutine(RunWave(_wave));
    }

    private IEnumerator RunWave(int wave)
    {
        _spawning = true;

        PathScript newPath = UnlockPaths(wave);

        OnWaveStart?.Invoke(wave);

        if (wave == _bossWave)
        {
            Spawn(_bossPrefab, RandomPath());
            OnBossSpawned?.Invoke();

            Debug.Log("Onada " + wave + ": MR. BONIATO");

            _spawning = false;
            yield break;
        }

        List<SpawnGroup> groups = BuildWave(wave);

        // xp por segundo, asi la wave dura lo mismo siempre
        float rate = GetWaveEnergy(wave) / GetSpawnDuration(wave);

        Debug.Log("Onada " + wave + ": Venen " + groups.Count + " grups (" + GetWaveEnergy(wave).ToString("0") + " xp)");

        for (int g = 0; g < groups.Count; g++)
        {
            SpawnGroup group = groups[g];

            // el primer grupo sale por el camino nuevo para que se vea
            PathScript path = (g == 0 && newPath != null) ? newPath : RandomPath();

            for (int i = 0; i < group.count; i++)
            {
                Spawn(group.entry.prefab, path);

                if (i < group.count - 1) yield return new WaitForSeconds(group.entry.groupSpacing);
            }

            float groupTime = group.entry.groupSpacing * (group.count - 1);
            float delay = group.entry.prefab.XP * group.count / rate - groupTime;

            yield return new WaitForSeconds(Mathf.Max(_minGroupDelay, delay));
        }

        _spawning = false;
    }

    private List<SpawnGroup> BuildWave(int wave)
    {
        List<SpawnGroup> groups = new();
        List<SpawnGroup> intros = new();
        float energy = GetWaveEnergy(wave);

        _pool.Clear();

        foreach (EnemyEntry e in _enemies)
        {
            if (e.prefab == null || e.prefab.XP <= 0f || e.WeightAt(wave) <= 0f) continue;

            if (e.unlockWave == wave)
            {
                // miniboss: salen los que toquen y no se repite en esta wave
                if (e.introCount > 0)
                {
                    intros.Add(new SpawnGroup { entry = e, count = e.introCount });
                    energy -= e.prefab.XP * e.introCount;
                    continue;
                }

                // enemigo nuevo normal: se asegura un grupo para que salga si o si
                groups.Add(new SpawnGroup { entry = e, count = e.groupMin });
                energy -= e.prefab.XP * e.groupMin;
            }

            _pool.Add(e);
        }

        // se van comprando grupos hasta gastar toda la xp
        while (true)
        {
            _affordable.Clear();
            float totalWeight = 0f;

            foreach (EnemyEntry e in _pool)
            {
                if (e.prefab.XP * Mathf.Max(1, e.groupMin) > energy + 0.001f) continue;

                _affordable.Add(e);
                totalWeight += e.WeightAt(wave) / (e.prefab.XP * e.AvgGroupAt(wave));
            }

            if (_affordable.Count == 0) break;

            float roll = UnityEngine.Random.value * totalWeight;
            EnemyEntry picked = _affordable[_affordable.Count - 1];

            foreach (EnemyEntry e in _affordable)
            {
                roll -= e.WeightAt(wave) / (e.prefab.XP * e.AvgGroupAt(wave));

                if (roll <= 0f)
                {
                    picked = e;
                    break;
                }
            }

            int maxAffordable = Mathf.FloorToInt((energy + 0.001f) / picked.prefab.XP);
            int count = Mathf.Clamp(picked.GroupSizeAt(wave), Mathf.Max(1, picked.groupMin), maxAffordable);

            groups.Add(new SpawnGroup { entry = picked, count = count });
            energy -= picked.prefab.XP * count;
        }

        Shuffle(groups);

        // los minibosses salen mas o menos a mitad de wave
        foreach (SpawnGroup intro in intros)
        {
            groups.Insert(Mathf.RoundToInt(groups.Count * _introPosition), intro);
        }

        return groups;
    }

    // devuelve el ultimo camino desbloqueado en esta wave o null
    private PathScript UnlockPaths(int wave)
    {
        int amount = 0;

        if (wave == 1) amount += _startingPaths;

        foreach (int w in _pathUnlockWaves)
        {
            if (w == wave) amount++;
        }

        PathScript last = null;

        for (int i = 0; i < amount && _lockedPaths.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, _lockedPaths.Count);

            last = _lockedPaths[index];
            _lockedPaths.RemoveAt(index);
            _unlockedPaths.Add(last);

            // posicion de salida para poner indicadores
            Vector3 exit = last.GetComponent<SplineContainer>().EvaluatePosition(0f);
            OnPathUnlocked?.Invoke(exit);
        }

        // en la primera wave no hace falta forzar el camino
        return wave == 1 ? null : last;
    }

    private PathScript RandomPath()
    {
        return _unlockedPaths[UnityEngine.Random.Range(0, _unlockedPaths.Count)];
    }

    private void Spawn(EnemyBase prefab, PathScript path)
    {
        EnemyBase enemy = Instantiate(prefab, transform);

        enemy._path = path;
        enemy.transform.position = path.GetComponent<SplineContainer>().EvaluatePosition(0f);
    }

    private void Shuffle(List<SpawnGroup> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}