using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Leveling")]
    [SerializeField] private Image _xpBar;
    [SerializeField] private GameObject _levelUpScreen;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Image[] _icons;
    [SerializeField] private Transform _radialBg;
    [SerializeField] private float _bgSpeed;

    [Header("Waves")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private RectTransform _waveIndicator;
    [SerializeField] private RectTransform _indicatorArrow;
    [SerializeField] private Animator _indicatorAnimator;
    [SerializeField] private float _indicatorMargin = 60f;

    [Header("refs")]
    [SerializeField] private TowerMenu _towerMenu;
    [SerializeField] private EnemySpawner _spawner;

    private TowerBase[] _towers;

    private Camera _cam;
    private Vector2 _indicatorTarget;

    void Start()
    {
        _towers = GameManager.Instance.TowerPrefabs;

        _cam = Camera.main;

        _levelUpScreen.SetActive(false);

        GameManager.Instance.OnXpUpdate += UpdateXpBar; 
        GameManager.Instance.OnLevelUp += StartLevelUp;

        _spawner.OnWaveStart += WaveStart;
        _spawner.OnPathWarning += ShowIndicator;
        _spawner.OnPathUnlocked += HideIndicator;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnXpUpdate -= UpdateXpBar;
        GameManager.Instance.OnLevelUp -= StartLevelUp;

        _spawner.OnWaveStart -= WaveStart;
        _spawner.OnPathWarning -= ShowIndicator;
        _spawner.OnPathUnlocked -= HideIndicator;
    }

    private void Update()
    {
        _radialBg.eulerAngles += Vector3.forward * Time.unscaledDeltaTime * _bgSpeed;

        if (_waveIndicator.gameObject.activeSelf) UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        Vector2 screenPos = _cam.WorldToScreenPoint(_indicatorTarget);

        // pegar la flecha borde de la pantalla
        float x = Mathf.Clamp(screenPos.x, _indicatorMargin, Screen.width - _indicatorMargin);
        float y = Mathf.Clamp(screenPos.y, _indicatorMargin, Screen.height - _indicatorMargin);
        _waveIndicator.position = new Vector2(x, y);

        // +90 pq la flecha empieza mirando abajo
        Vector2 center = new Vector2(Screen.width, Screen.height) * 0.5f;
        Vector2 dir = screenPos - center;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 90f;
        _indicatorArrow.localEulerAngles = new Vector3(0f, 0f, angle);
    }

    private void UpdateXpBar(float currentXp, float maxXp)
    {
        _xpBar.fillAmount = currentXp / maxXp;
    }

    private void StartLevelUp()
    {
        // pausar juego
        GameManager.Instance.Pause();

        // escoger 3 torres
        TowerBase[] towers = PickTowers(_buttons.Length); 
        
        for(int i = 0; i < _buttons.Length; i++)
        {
            bool hasTower = i < towers.Length;
            if (!hasTower) continue;

            TowerBase tower = towers[i];

            // poner sprite
            SpriteRenderer sr = towers[i].VisualTransform.GetComponentInChildren<SpriteRenderer>();
            _icons[i].sprite = sr.sprite;

            // setear onclick
            _buttons[i].onClick.RemoveAllListeners();
            _buttons[i].onClick.AddListener(() => _towerMenu.addTower(tower));
            _buttons[i].onClick.AddListener(EndLevelUp);
        }

        _levelUpScreen.SetActive(true);
    }


    public void EndLevelUp()
    {
        GameManager.Instance.UnPause();
        _levelUpScreen.SetActive(false);
    }

    // para escoger 3 torres random del array sin repetir
    private TowerBase[] PickTowers(int count) 
    {
        List<TowerBase> towers = _towers.ToList();
        count = Mathf.Min(count, towers.Count);

        TowerBase[] selected = new TowerBase[count];

        for (int i = 0; i < count; i++)
        {
            int j = Random.Range(0, towers.Count);
            selected[i] = towers[j];
            towers.RemoveAt(j);
        }

        return selected;
    }

    private void ShowIndicator(Vector2 pos)
    {
        _indicatorTarget = pos;

        _indicatorAnimator.SetBool("active", true);

        UpdateIndicator();
    }

    private void HideIndicator(Vector2 pos)
    {
        _indicatorAnimator.SetBool("active", false);
    }

    private void WaveStart(int num)
    {
        _waveText.text = $"Wave {num}/30";
    }
}
