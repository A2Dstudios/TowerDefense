using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _edgeRadius = 0.9f;
    [SerializeField] private float _moveSpeed = 5f;

    private PlayerInputs _input;
    private Camera _cam;

    private void Awake()
    {
        _input = new PlayerInputs();
        _input.Player.Enable();

        _cam = Camera.main;

        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnDestroy()
    {
        _input.Player.Disable();
    }

    private void Update()
    {
        Vector2 _mousePos = _input.Player.Mouse.ReadValue<Vector2>();
        // centro = 0,0 
        _mousePos = _cam.ScreenToViewportPoint(_mousePos)*2 - Vector3.one;

        Vector3 moveDir = Vector3.zero;
        if (_mousePos.x < -_edgeRadius) moveDir.x = -1;
        if (_mousePos.x > _edgeRadius) moveDir.x = 1;
        if (_mousePos.y < -_edgeRadius) moveDir.y = -1;
        if (_mousePos.y > _edgeRadius) moveDir.y = 1;
        moveDir.Normalize();

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }
}
