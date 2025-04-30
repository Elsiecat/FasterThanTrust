using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 중앙 정렬된 바닥과 벽 타일맵을 생성하고, 이동 가능 영역과 카메라 위치를 설정하는 유틸리티.
/// </summary>
public static class TilemapFloorGenerator
{
    private const string TILEMAP_PARENT_NAME = "GeneratedTilemapRoot";
    private const int WALL_THICKNESS = 3;

    public static Bounds PlayableAreaBounds { get; private set; }

    public static void Generate(Vector2 areaSize, Vector2 center)
    {
        // 리소스 로드
        TileBase floorTile = Resources.Load<TileBase>("Tiles/Tile_Floor");
        TileBase wallTile = Resources.Load<TileBase>("Tiles/Tile_Concrete");

        if (floorTile == null || wallTile == null)
        {
            Debug.LogError("[TilemapFloorGenerator] 타일 리소스를 불러오지 못했습니다.");
            return;
        }

        ClearPreviousTilemap();
        GameObject gridRoot = CreateGridRoot();

        Vector3Int offset = CalculateOffset(areaSize);
        GenerateFloorTilemap(gridRoot.transform, areaSize, offset, floorTile);
        GenerateWallTilemap(gridRoot.transform, areaSize, offset, wallTile);

        PlayableAreaBounds = new Bounds(Vector3.zero, new Vector3(areaSize.x, areaSize.y, 1));
    }

    /// <summary>
    /// 기존 생성된 타일맵 루트를 제거한다.
    /// </summary>
    private static void ClearPreviousTilemap()
    {
        GameObject old = GameObject.Find(TILEMAP_PARENT_NAME);
        if (old != null)
            Object.DestroyImmediate(old);
    }

    /// <summary>
    /// 새로운 Grid 루트 GameObject를 생성한다.
    /// </summary>
    private static GameObject CreateGridRoot()
    {
        GameObject gridGO = new GameObject(TILEMAP_PARENT_NAME);
        Grid grid = gridGO.AddComponent<Grid>();
        grid.cellSize = Vector3.one;
        return gridGO;
    }

    /// <summary>
    /// 바닥 타일맵을 생성한다.
    /// </summary>
    private static void GenerateFloorTilemap(Transform parent, Vector2 areaSize, Vector3Int offset, TileBase floorTile)
    {
        int width = Mathf.RoundToInt(areaSize.x);
        int height = Mathf.RoundToInt(areaSize.y);

        GameObject floorGO = new GameObject("FloorTilemap");
        floorGO.transform.parent = parent;
        Tilemap tilemap = floorGO.AddComponent<Tilemap>();
        floorGO.AddComponent<TilemapRenderer>().sortingOrder = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0) + offset;
                tilemap.SetTile(pos, floorTile);
            }
        }
    }

/// <summary>
/// 벽 타일맵을 생성하고 Collider 설정까지 수행한다.
/// </summary>
private static void GenerateWallTilemap(Transform parent, Vector2 areaSize, Vector3Int offset, TileBase wallTile)
{
    int width = Mathf.RoundToInt(areaSize.x);
    int height = Mathf.RoundToInt(areaSize.y);

    GameObject wallGO = new GameObject("WallTilemap");
    wallGO.transform.parent = parent;
    Tilemap wallTilemap = wallGO.AddComponent<Tilemap>();
    wallGO.AddComponent<TilemapRenderer>().sortingOrder = 1;

    // Collider 설정
    TilemapCollider2D tilemapCollider = wallGO.AddComponent<TilemapCollider2D>();
    tilemapCollider.compositeOperation = Collider2D.CompositeOperation.Merge;

    // Rigidbody2D가 이미 존재할 수 있으므로 중복 방지
    Rigidbody2D rb = wallGO.GetComponent<Rigidbody2D>();
    if (rb == null)
        rb = wallGO.AddComponent<Rigidbody2D>();
    rb.bodyType = RigidbodyType2D.Static;

    // CompositeCollider2D도 중복 방지
    if (wallGO.GetComponent<CompositeCollider2D>() == null)
        wallGO.AddComponent<CompositeCollider2D>();

    wallGO.layer = LayerMask.NameToLayer("Wall");

    for (int x = -WALL_THICKNESS; x < width + WALL_THICKNESS; x++)
    {
        for (int y = -WALL_THICKNESS; y < height + WALL_THICKNESS; y++)
        {
            if (x >= 0 && x < width && y >= 0 && y < height) continue;
            Vector3Int wallPos = new Vector3Int(x, y, 0) + offset;
            wallTilemap.SetTile(wallPos, wallTile);
        }
    }
}

    /// <summary>
    /// 바닥 기준 중심을 맞추기 위한 오프셋 계산.
    /// </summary>
    private static Vector3Int CalculateOffset(Vector2 areaSize)
    {
        int width = Mathf.RoundToInt(areaSize.x);
        int height = Mathf.RoundToInt(areaSize.y);
        return new Vector3Int(
            Mathf.FloorToInt(-width / 2f),
            Mathf.FloorToInt(-height / 2f),
            0
        );
    }

    // /// <summary>
    // /// 카메라를 맵 중심으로 이동시킨다.
    // /// </summary>
    // private static void CenterCamera()
    // {
    //     Camera cam = Camera.main;
    //     if (cam != null)
    //     {
    //         cam.transform.position = new Vector3(0, 0, -10);
    //     }
    // }
}
