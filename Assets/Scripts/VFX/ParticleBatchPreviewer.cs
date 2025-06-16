#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class EditorParticleAutoPlayer : MonoBehaviour
{
    private void OnEnable()
    {
        if (Application.isPlaying) return;

        // 강제로 Scene 뷰에 다시 그리도록 요청
        SceneView.RepaintAll();

        // 모든 자식 파티클 재생 시도
        var systems = GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in systems)
        {
            ps.Clear(true); // 혹시 멈춰있던 거 초기화
            ps.Play(true);  // 재생
        }
    }
}
#endif
