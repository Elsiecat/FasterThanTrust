using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 절대 바운더리 밖으로 나가지 않는 카메라 컨트롤러 (WASD, 드래그, 줌 포함)
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _dragSpeed = 1f;

    [Header("줌 설정")]
    [SerializeField] private float _zoomSpeed = 5f;

    private float _zoomMin; // 이 맵에서 화면이 경계 넘지 않는 최소 줌 (zoom out 최대치)
    private float _zoomMax; // 약간 더 확대 허용

    private Camera _camera;
    private Vector2 _stageSize;
    private Vector2 _minLimit;
    private Vector2 _maxLimit;

    private bool _stageInitialized = false;
    private Vector3? _dragStartWorldPos = null;

    private const float TILE_UNIT_SIZE = 1f;
    private const int WALL_THICKNESS = 3;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (!_stageInitialized) return;

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

        if (scroll < 0) // 줌 아웃
        {
            if (CanZoomOut(targetSize))
            {
                _camera.orthographicSize = Mathf.Min(targetSize, _zoomMax);
                return true;
            }
        }
        else // 줌 인
        {
            _camera.orthographicSize = Mathf.Clamp(targetSize, _zoomMin, _zoomMax);
            return true;
        }

        return false;
    }
    private bool CanZoomOut(float targetSize)
    {
        float camHalfWidth = targetSize * _camera.aspect;
        float camHalfHeight = targetSize;

        Vector3 camPos = transform.position;

        bool insideX = camPos.x - camHalfWidth >= _minLimit.x && camPos.x + camHalfWidth <= _maxLimit.x;
        bool insideY = camPos.y - camHalfHeight >= _minLimit.y && camPos.y + camHalfHeight <= _maxLimit.y;

        return insideX && insideY;
    }


    private void ClampPosition()
    {
        float camHeight = _camera.orthographicSize;
        float camWidth = _camera.orthographicSize * _camera.aspect;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, _minLimit.x + camWidth, _maxLimit.x - camWidth);
        pos.y = Mathf.Clamp(pos.y, _minLimit.y + camHeight, _maxLimit.y - camHeight);
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
    /// StageManager에서 스테이지 사이즈 넘겨받아 카메라 설정
    /// </summary>
    public void InitializeCamera(Vector2 stageTileSize)
    {
        _stageInitialized = true;
        _stageSize = stageTileSize;

        Vector2 fullSize = (_stageSize + Vector2.one * WALL_THICKNESS * 2f) * TILE_UNIT_SIZE;
        Vector2 offset = new Vector2(Mathf.FloorToInt(-fullSize.x / 2f), Mathf.FloorToInt(-fullSize.y / 2f));
        _minLimit = offset;
        _maxLimit = offset + fullSize;

        transform.position = new Vector3(0, 0, -10); // 중앙 정렬

        float mapWidth = fullSize.x;
        float mapHeight = fullSize.y;
        float aspect = _camera.aspect;

        // 긴쪽에 맞춰 zoom 설정
        if (aspect >= mapWidth / mapHeight)
        {
            // 가로가 넓음 → 가로 맞춤
            _zoomMin = mapWidth / (2f * aspect);
        }
        else
        {
            // 세로가 넓음 → 세로 맞춤
            _zoomMin = mapHeight / 2f;
        }

        _zoomMax = _zoomMin * 1.2f; // 약간 zoom in 허용
        _camera.orthographicSize = _zoomMin;

        ClampPosition();
    }
}
