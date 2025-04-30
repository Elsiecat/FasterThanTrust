using UnityEngine;

/// <summary>
/// 게임 내에서 사용하는 상수, 열거형 등을 통합 정의하는 클래스.
/// </summary>
public static class Define
{
    // 레이어 이름
    public const string LAYER_HUMAN = "Human";
    public const string LAYER_ZOMBIE = "Zombie";

    // 태그
    public const string TAG_HUMAN = "Human";
    public const string TAG_ZOMBIE = "Zombie";

    // 리소스 경로
    public const string PATH_CITIZEN_PREFAB = "Prefabs/Citizen";
    public const string PATH_ZOMBIE_PREFAB = "Prefabs/Zombie";
    public const string PATH_TILE_SPRITE = "Sprites/Map/FloorTile";
    public const string PATH_TILEMAP_TILE = "Tiles/Tile_Floor";
    public const string PATH_DAMAGE_TEXT = "VFX/t_damage";


    public const int WALL_THICKNESS = 3; 
    /// <summary>
    /// 게임 상태를 나타내는 열거형.
    /// </summary>
    public enum GameState
    {
        Init,
        WaitingForInfection,
        Playing,
        GameOver
    }
    public enum SceneType
    {
        Unknown,
        Title,
        Game,
        Result
    }

    public enum HumanState
    {
        Peaceful,       // 평화로움 (아무 위협 없음)
        Suspicious,     // 긴장 상태 (아직 좀비는 안 보임, 경계)
        ZombieNearby    // 주변에 좀비 있음 (회피 모드)
    }
    public enum ZombieState
    {
        Peaceful,       // 주변에 시민이 없음
        Suspicious,     // 긴장 상태 (아직 좀비는 안 보임, 경계)
        HumanNearby    // 주변에 사람 있음 (공격 모드)
    }

    public enum InfectedState
    {
        Clean,
        Infected
    }

}
