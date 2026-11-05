using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _objective;
    public Vector3 Objective => _objective.position;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

}
