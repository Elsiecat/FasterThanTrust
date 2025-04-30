using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 전환을 담당하는 정적 유틸리티 클래스.
/// Define.SceneType 기반으로 씬 이름을 매핑하고 전환한다.
/// </summary>
public static class SceneLoader
{
    /// <summary>지정된 씬 타입을 로드</summary>
    public static void LoadScene(Define.SceneType type)
    {
        SceneManager.LoadScene(GetSceneName(type));
    }

    /// <summary>씬 타입에 따른 씬 이름 반환</summary>
    private static string GetSceneName(Define.SceneType type)
    {
        return type switch
        {
            Define.SceneType.Title => "TitleScene",
            Define.SceneType.Game => "GameScene",
            Define.SceneType.Result => "ResultScene",
            _ => "UnknownScene"
        };
    }
}
