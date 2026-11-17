using UnityEngine;

public class SpriteColorRandomizer : MonoBehaviour
{
    [SerializeField] private Color[] _colors;

    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();

        _sprite.color = _colors[Random.Range(0, _colors.Length)];
    }
}
