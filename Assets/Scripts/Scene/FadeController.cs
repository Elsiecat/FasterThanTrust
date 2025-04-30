using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 페이드 인/아웃 효과를 제어하는 UI 컴포넌트.
/// </summary>
public class FadeController : MonoBehaviour
{
    [SerializeField] private Image _fadeImage; // 알파 조절용 이미지
    [SerializeField] private float _fadeSpeed = 2.0f; // 알파 변화 속도

    /// <summary>페이드 아웃 후 콜백 호출</summary>
    public void FadeOut(Action onComplete)
    {
        StartCoroutine(FadeRoutine(0f, 1f, onComplete));
    }

    /// <summary>페이드 인 후 콜백 호출</summary>
    public void FadeIn(Action onComplete)
    {
        StartCoroutine(FadeRoutine(1f, 0f, onComplete));
    }

    /// <summary>페이드 실행 코루틴</summary>
    private System.Collections.IEnumerator FadeRoutine(float from, float to, Action onComplete)
    {
        float t = 0f;
        Color color = _fadeImage.color;
        color.a = from;
        _fadeImage.color = color;

        while (t < 1f)
        {
            t += Time.deltaTime * _fadeSpeed;
            color.a = Mathf.Lerp(from, to, t);
            _fadeImage.color = color;
            yield return null;
        }

        color.a = to;
        _fadeImage.color = color;
        onComplete?.Invoke();
    }
}
