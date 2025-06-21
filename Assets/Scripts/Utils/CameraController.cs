using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// BoxCollider2D로 정의된 영역 내에서만 이동/줌이 가능한 카메라 컨트롤러.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dragSpeed = 1f;

    [Header("줌 설정")]
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _zoomMin = 2f;
    [SerializeField] private float _zoomMax = 20f;

    private Camera _camera;
    private Bounds _cameraBounds;
    private bool _boundInitialized = false;
    private Vector3? _dragStartWorldPos = null;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!_boundInitialized) return;

        bool moved = HandleMove();
        bool dragged = HandleDrag();
        bool zoomed = HandleZoom();

        if (moved || dragged || zoomed)
            ClampPosition();
    }

    private bool HandleMove()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input == Vector2.zero) return false;

        Vector3 move = new Vector3(input.x, input.y, 0f).normalized;
        transform.Translate(move * _moveSpeed * Time.deltaTime, Space.World);
        return true;
    }

    private bool HandleDrag()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIOrInteractable())
            _dragStartWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButton(0) && _dragStartWorldPos.HasValue && !IsPointerOverUIOrInteractable())
        {
            Vector3 currentWorldPos = _camera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 offset = _dragStartWorldPos.Value - currentWorldPos;
            transform.position += offset * _dragSpeed;
            _dragStartWorldPos = currentWorldPos;
            return true;
        }

        if (Input.GetMouseButtonUp(0))
            _dragStartWorldPos = null;

        return false;
    }

    private bool HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) < 0.01f) return false;

        float targetSize = _camera.orthographicSize - scroll * _zoomSpeed;
        _camera.orthographicSize = Mathf.Clamp(targetSize, _zoomMin, _zoomMax);
        return true;
    }

    private void ClampPosition()
    {
        float camHeight = _camera.orthographicSize;
        float camWidth = _camera.orthographicSize * _camera.aspect;

        Vector3 pos = transform.position;

        float minX = _cameraBounds.min.x + camWidth;
        float maxX = _cameraBounds.max.x - camWidth;
        float minY = _cameraBounds.min.y + camHeight;
        float maxY = _cameraBounds.max.y - camHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }

    private bool IsPointerOverUIOrInteractable()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return true;

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        return hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable");
    }

    /// <summary>
    /// 맵 프리팹 내의 CameraBounds (BoxCollider2D)에서 바운드 정보를 받아 카메라 제한 영역 설정
    /// </summary>
    public void SetCameraBounds(BoxCollider2D boundCollider)
    {
        _cameraBounds = boundCollider.bounds;
        _boundInitialized = true;

        // 맵 중앙으로 이동
        Vector3 center = _cameraBounds.center;
        transform.position = new Vector3(center.x, center.y, -10f);

        // 줌 기본값도 재설정 가능
        ClampPosition();
    }
}
