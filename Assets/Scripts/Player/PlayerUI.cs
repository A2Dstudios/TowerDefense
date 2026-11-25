using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Leveling")]
    [SerializeField] private Image _xpBar;
    [SerializeField] private GameObject _levelUpScreen;
    [SerializeField] private Transform _radialBg;
    [SerializeField] private float _bgSpeed;

    void Start()
    {
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
        _levelUpScreen.SetActive(true);
        Time.timeScale = 0f;
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f;
        _levelUpScreen.SetActive(false);
    }
}
