using UnityEngine;

/// <summary>
/// 현재 Stage 정보를 기반으로 카메라 이동 최대 영역을 디버그 표시하는 컴포넌트.
/// 오프셋을 반영해서 정확한 위치에 표시.
/// </summary>
public class CameraBoundsDebugger : MonoBehaviour
{
    private Vector2 _stageTileSize = Vector2.zero;
    private Vector2 _fullSize = Vector2.zero;

    private const float TILE_UNIT_SIZE = 1.0f;
    private const int WALL_THICKNESS = 3;

    private void Update()
    {
        if (_stageTileSize == Vector2.zero)
        {
            if (Managers.Instance != null && Managers.Stage != null && Managers.Stage.CurrentStage != null)
            {
                _stageTileSize = Managers.Stage.CurrentStage.spawnAreaSize;
                _fullSize = (_stageTileSize + new Vector2(WALL_THICKNESS * 2, WALL_THICKNESS * 2)) * TILE_UNIT_SIZE;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_fullSize == Vector2.zero) return;

        Gizmos.color = Color.red;

        float width = _fullSize.x;
        float height = _fullSize.y;

        Vector2 offset = new Vector2(
            Mathf.FloorToInt(-width / 2f),
            Mathf.FloorToInt(-height / 2f)
        );

        Vector3 bottomLeft = new Vector3(offset.x, offset.y, 0f);
        Vector3 bottomRight = new Vector3(offset.x + width, offset.y, 0f);
        Vector3 topLeft = new Vector3(offset.x, offset.y + height, 0f);
        Vector3 topRight = new Vector3(offset.x + width, offset.y + height, 0f);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
