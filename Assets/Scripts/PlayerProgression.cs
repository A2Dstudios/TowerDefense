
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    [SerializeField] protected float _playerXP = 0f;
    [SerializeField] protected int _playerLVL = 0;

    [SerializeField] protected float _baseLevelXP = 10f;
    [SerializeField] protected float _nextLevelXP;
    [SerializeField] protected float _multiplierXP = 1.25f;

    protected virtual void Start()
    {
        _nextLevelXP = _baseLevelXP;
    }

    public virtual void AddXP(float xp)
    {
        _playerXP += xp;

        LevelUP();
    }

    protected virtual void LevelUP()
    {
        while (_playerXP >= _nextLevelXP)
        {
            _playerXP -= _nextLevelXP;

            _playerLVL++;

            _nextLevelXP *= _multiplierXP;

        }
    }
}
