using UnityEngine;
using UnityEngine.Splines;

public class PathScript : MonoBehaviour
{

    [SerializeField]
    private PathScript[] _nextPath;

    public PathScript getNext()
    {
        if (_nextPath.Length == 0)
            return null;
        return _nextPath[Random.Range(0,_nextPath.Length)];
    }
}
