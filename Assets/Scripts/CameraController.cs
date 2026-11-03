using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _edgeRadius = 0.9f;
    [SerializeField] private float _moveSpeed = 5f;

    private PlayerInputs _input;
    private Camera _cam;

    // el transform de la vcam se puede salir de los margenes mientras la camara de verdad esta quieta en el borde
    private float _maxDistanceFromCamera = 0.1f;

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

        // multiplicador de velocidad, para que el cursor justo en el borde de la pantalla haga que se mueva mas rapido
        float maxPos = Mathf.Max(Mathf.Abs(_mousePos.x), Mathf.Abs(_mousePos.y));
        float t = (maxPos - _edgeRadius) / (1f - _edgeRadius);
        float speedMult = Mathf.Lerp(0, 1f, t);

        // aplicar el movimiento al transform de la vcam
        transform.position += moveDir * _moveSpeed * speedMult * Time.deltaTime;

        // correccion por si la vcam se separa de la camara real (se sale de los bordes)
        Vector3 cameraPos = _cam.transform.position;
        cameraPos.z = transform.position.z;

        Vector3 fromCameraToTarget = transform.position - cameraPos;
        if (fromCameraToTarget.magnitude > _maxDistanceFromCamera)
        {
            transform.position = cameraPos + (fromCameraToTarget.normalized * _maxDistanceFromCamera);
        }
    }
}
