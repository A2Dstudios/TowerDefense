using System.Collections.Generic;
using System.Linq;
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

    [Header("refs")]
    [SerializeField] private TowerMenu _towerMenu;

    private TowerBase[] _towers;

    void Start()
    {
        _towers = GameManager.Instance.TowerPrefabs;

        _levelUpScreen.SetActive(false);

        GameManager.Instance.OnXpUpdate += UpdateXpBar; 
        GameManager.Instance.OnLevelUp += StartLevelUp; 
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnXpUpdate -= UpdateXpBar;
        GameManager.Instance.OnLevelUp -= StartLevelUp; 
    }

    private void Update()
    {
        _radialBg.eulerAngles += Vector3.forward * Time.unscaledDeltaTime * _bgSpeed;
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
}
