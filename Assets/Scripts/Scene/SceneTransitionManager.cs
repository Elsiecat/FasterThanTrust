using UnityEngine;

/// <summary>
/// 씬 전환 시 페이드 효과를 포함한 씬 로딩을 관리하는 매니저.
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private FadeController _fadeController; // 페이드 컨트롤러

    /// <summary>페이드 후 씬 전환</summary>
    public void TransitionToScene(Define.SceneType nextScene)
    {
        if (_fadeController != null)
        {
            _fadeController.FadeOut(() =>
            {
                SceneLoader.LoadScene(nextScene);
            });
        }
        else
        {
            SceneLoader.LoadScene(nextScene);
        }
    }
}
