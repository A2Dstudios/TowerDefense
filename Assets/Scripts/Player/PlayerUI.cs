using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Leveling")]
    [SerializeField] private Image _xpBar;
    [SerializeField] private GameObject _levelUpScreen;

    void Start()
    {
        GameManager.Instance.OnXpUpdate += UpdateXpBar; 
        GameManager.Instance.OnLevelUp += StartLevelUp; 
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnXpUpdate -= UpdateXpBar;
        GameManager.Instance.OnLevelUp -= StartLevelUp; 
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
