using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _edgeRadius = 0.9f;
    [SerializeField] private float _moveSpeed = 5f;

    [Header("Zoom")]
    [SerializeField] private float _zoomSpeed = 1f;
    [SerializeField] private float _minZoom = 2.5f;
    [SerializeField] private float _maxZoom = 7.5f;

    private PlayerInputs _input;
    private Camera _cam;
    private CinemachineCamera _vCam;
    private CinemachineConfiner2D _confiner;

    // el transform de la vcam se puede salir de los margenes mientras la camara de verdad esta quieta en el borde
    private float _maxDistanceFromCamera = 0.1f;

    private float _zoom;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineCamera>();
        _confiner = GetComponent<CinemachineConfiner2D>();
        _input = new PlayerInputs();
        _input.Player.Enable();

        _cam = Camera.main;

        Cursor.lockState = CursorLockMode.Confined;

        _zoom = _vCam.Lens.OrthographicSize;
    }

    private void OnDestroy()
    {
        _input.Player.Disable();
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void MoveCamera()
    {
        Vector2 _mousePos = _input.Player.Mouse.ReadValue<Vector2>();
        // centro = 0,0 
        _mousePos = _cam.ScreenToViewportPoint(_mousePos) * 2 - Vector3.one;

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

    private float _zoomVelocity; // se usa para el smooth
    private void ZoomCamera()
    {
        float scroll = _input.Player.Zoom.ReadValue<Vector2>().y;


        if (Mathf.Abs(scroll) > 0.01f)
        {
            _zoom -= Mathf.Sign(scroll) * _zoomSpeed * 0.01f;
            _zoom = Mathf.Clamp(_zoom, _minZoom, _maxZoom);
        }

        // super smooth super satisfactorio
        _vCam.Lens.OrthographicSize = Mathf.SmoothDamp(_vCam.Lens.OrthographicSize, _zoom, ref _zoomVelocity, 0.15f);

        _confiner.InvalidateBoundingShapeCache(); // para comentar con lucas si me acuerdo
    }
}
