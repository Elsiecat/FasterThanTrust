using UnityEngine;

/// <summary>
/// 인게임 씬을 초기화하고, 맵 및 UI 등을 띄우는 컨트롤러.
/// </summary>
public class GameScene : BaseScene
{
    /// <summary>
    /// 게임 씬 초기화 로직
    /// </summary>
    public override void InitScene()
    {
        // 게임 상태 초기화 (감염 상태, 감염파워 등)
        Managers.Game.Init();

        // 시민 스폰은 더 이상 사용하지 않음
        // 시민은 맵 프리팹에 미리 배치되어 있음
        // Managers.Spawn.SpawnCitizens();

        // ✅ 카메라 바운드 설정 (맵 프리팹에 포함된 CameraBounds 참조)
        GameObject map = GameObject.FindWithTag("Map"); // 또는 Managers.Stage에서 _mapInstance 직접 참조
        if (map != null)
        {
            BoxCollider2D camBound = map.transform.Find("CameraBounds")?.GetComponent<BoxCollider2D>();
            if (camBound != null)
            {
                Camera.main.GetComponent<CameraController>()?.SetCameraBounds(camBound);
            }
            else
            {
                Debug.LogWarning("CameraBounds 오브젝트를 찾을 수 없습니다.");
            }
        }

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
