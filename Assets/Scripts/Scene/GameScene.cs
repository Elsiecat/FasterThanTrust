using UnityEngine;

/// <summary>
/// 인게임 씬을 초기화하고, 시민 스폰 및 카드 UI 등을 띄우는 컨트롤러.
/// </summary>
public class GameScene : BaseScene
{
    /// <summary>
    /// 게임 씬 초기화 로직
    /// </summary>
    public override void InitScene()
    {
       // Debug.Log("🎮 게임 씬 초기화됨");

        // 게임 상태 초기화 (감염 상태, 감염파워 등)
        Managers.Game.Init();

        // 시민 스폰
        Managers.Spawn.SpawnCitizens();

        // UI 초기 표시
        Managers.UI.ShowDim();
        Managers.UI.ShowSkillCards();
    }

    /// <summary>
    /// 게임 씬 전환 시 리소스 해제 처리
    /// </summary>
    public override void ClearScene()
    {
        Debug.Log("🎮 게임 씬 종료됨");
        // 현재 리소스 해제 로직 없음 (필요시 추가 가능)
    }

    /// <summary>
    /// 이 씬의 타입을 반환
    /// </summary>
    public override Define.SceneType SceneType => Define.SceneType.Game;
}
