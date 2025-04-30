using UnityEngine;

/// <summary>
/// 타이틀 화면 씬을 초기화하고, 시작 버튼 클릭 시 게임 씬으로 전환하는 컨트롤러.
/// </summary>
public class TitleScene : BaseScene
{
    [SerializeField] private SceneTransitionManager _sceneTransition; // 씬 전환 매니저

    /// <summary>
    /// 타이틀 화면 초기화 로직
    /// </summary>
    public override void InitScene()
    {
        Debug.Log("🏠 타이틀 씬 진입");
    }

    /// <summary>
    /// 타이틀 화면 정리 로직
    /// </summary>
    public override void ClearScene()
    {
        Debug.Log("🏠 타이틀 씬 종료");
    }

    /// <summary>
    /// 이 씬의 타입을 반환
    /// </summary>
    public override Define.SceneType SceneType => Define.SceneType.Title;

    /// <summary>
    /// 시작 버튼 클릭 시 게임 씬으로 전환
    /// </summary>
    public void OnClickStart()
    {
        _sceneTransition.TransitionToScene(Define.SceneType.Game);
    }
}
