using UnityEngine;

public class CameraController : MonoBehaviour
{
    private PlayerInputs _input;
    private Vector2 _mousePos;

    private void Awake()
    {
        _input = new PlayerInputs();
        _input.Player.Enable();
    }

    private void OnDestroy()
    {
        _input.Player.Disable();
    }

    private void Update()
    {
        _mousePos = _input.Player.Mouse.ReadValue<Vector2>();


    }
}
