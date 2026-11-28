using UnityEngine;
using UnityEngine.Splines;

public class PathScript : MonoBehaviour
{

    [SerializeField] private PathScript[] _nextPath;
    [SerializeField] private bool _isCenter;
    [SerializeField] private GameObject _bossSign;

    public bool IsCenter => _isCenter;

    public PathScript getNext()
    {
        if (_nextPath.Length == 0)
            return null;
        return _nextPath[Random.Range(0,_nextPath.Length)];
    }

    public void SetBossSign(bool visible)
    {
        if (_bossSign != null) _bossSign.SetActive(visible);
    }
}
