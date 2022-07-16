using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float Epsilon = 0.01f;

    private DefaultActions _input;
    public float moveDamping;
    public float zoomDamping;
    public Vector2 cameraZoomRange;
    public float moveSpeed;
    public float zoomSpeed;
    public Camera camera;

    private Vector3 _targetPosition;
    private Vector3 _currentMovementVelocity;
    private float _currentZoomVelocity;
    private float _targetOrthograficSize;

    private void Awake()
    {
        _targetOrthograficSize = camera.orthographicSize;
        _targetPosition = transform.position;

        _input = new();
        _input.Enable();
    }

    private void Update()
    {
        var movement = _input.Standard.Movement.ReadValue<Vector2>().normalized * (moveSpeed * Time.deltaTime);
        var zoom = _input.Standard.Zoom.ReadValue<float>() * zoomSpeed * Time.deltaTime;

        _targetPosition += (Vector3)movement;
        _targetOrthograficSize = Mathf.Clamp(_targetOrthograficSize + zoom, cameraZoomRange.x, cameraZoomRange.y);

        if ((_targetPosition - transform.position).magnitude > Epsilon)
        {
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentMovementVelocity,
                                    moveDamping);
        }
        if (Mathf.Abs(camera.orthographicSize - _targetOrthograficSize) > Epsilon)
        {
            camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, _targetOrthograficSize,
                                    ref _currentZoomVelocity, zoomDamping);
        }
    }
}
