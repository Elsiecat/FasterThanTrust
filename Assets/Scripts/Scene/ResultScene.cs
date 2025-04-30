using UnityEngine;

/// <summary>
/// 결과 씬을 담당하는 클래스.
/// 게임 종료 후 통계 표시, 리스타트 UI 등을 관리한다.
/// </summary>
public class ResultScene : BaseScene
{
    /// <summary>
    /// 결과 화면 초기화
    /// </summary>
    public override void InitScene()
    {
        Debug.Log("📊 결과 씬 초기화됨");
    }

    /// <summary>
    /// 씬 해제 시 처리 로직
    /// </summary>
    public override void ClearScene()
    {
        // 예: 통계 패널 닫기
    }

    /// <summary>
    /// 이 씬이 결과 씬임을 명시
    /// </summary>
    public override Define.SceneType SceneType => Define.SceneType.Result;
}
