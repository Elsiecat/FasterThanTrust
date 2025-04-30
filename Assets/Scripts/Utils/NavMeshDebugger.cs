using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

/// <summary>
/// Tilemap 상의 NavMesh 영역 유효 여부를 시각적으로 디버깅하는 유틸리티.
/// FloorTilemap의 모든 타일 위치를 스캔하며 파란색(유효), 빨간색(무효) 디버그 타일을 생성.
/// </summary>
public class NavMeshDebugger : MonoBehaviour
{
    [Header("디버그 타일 리소스 경로")]
    private const string VALID_TILE_PATH = "DebugTiles/Tile_Debug_Valid";
    private const string INVALID_TILE_PATH = "DebugTiles/Tile_Debug_Invalid";

    [Header("NavMesh 샘플링 반지름")]
    public float sampleRadius = 0.2f;

    private TileBase _validTile;
    private TileBase _invalidTile;

    private Tilemap _debugTilemap;
    private Tilemap _floorTilemap;

    private void Start()
    {
        _validTile = Resources.Load<TileBase>(VALID_TILE_PATH);
        _invalidTile = Resources.Load<TileBase>(INVALID_TILE_PATH);

        if (_validTile == null || _invalidTile == null)
        {
            Debug.LogError("[NavMeshDebugger] 디버그 타일 리소스를 불러올 수 없습니다.");
            return;
        }

        GameObject floorGO = GameObject.Find("Tilemap");
        if (floorGO == null)
        {
            Debug.LogError("[NavMeshDebugger] 'Tilemap' 오브젝트를 찾을 수 없습니다.");
            return;
        }

        _floorTilemap = floorGO.GetComponent<Tilemap>();
        if (_floorTilemap == null)
        {
            Debug.LogError("[NavMeshDebugger] Tilemap 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        CreateDebugTilemap(floorGO.transform);
        VisualizeNavMesh();
    }

    /// <summary>
    /// 디버그 타일맵을 FloorTilemap과 동일한 위치에 생성
    /// </summary>
    private void CreateDebugTilemap(Transform parent)
    {
        GameObject debugGO = new GameObject("NavMeshDebugTilemap");
        debugGO.transform.SetParent(parent);
        debugGO.transform.localPosition = Vector3.zero;

        _debugTilemap = debugGO.AddComponent<Tilemap>();
        TilemapRenderer renderer = debugGO.AddComponent<TilemapRenderer>();
        renderer.sortingOrder = -5;
    }

    /// <summary>
    /// 바닥 타일 좌표 순회하며 NavMesh 유효 여부 표시
    /// </summary>
    private void VisualizeNavMesh()
    {
        BoundsInt bounds = _floorTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);
                TileBase tile = _floorTilemap.GetTile(cellPos);
                if (tile == null) continue;

                Vector3 worldPos = _floorTilemap.GetCellCenterWorld(cellPos);
                bool isValid = NavMesh.SamplePosition(worldPos, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas);

                _debugTilemap.SetTile(cellPos, isValid ? _validTile : _invalidTile);
            }
        }
    }
}
