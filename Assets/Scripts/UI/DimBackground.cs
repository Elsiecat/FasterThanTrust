using UnityEngine;

/// <summary>
/// 스킬 카드가 등장할 때 화면에 어두운 딤 배경을 깔기 위한 컴포넌트.
/// Show/Hide 메서드로 투명도 조절이 가능하다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DimBackground : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 배경을 보여줌 (알파 = 1, 입력 허용)
    /// </summary>
    public void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    /// <summary>
    /// 배경을 숨김 (알파 = 0, 입력 차단)
    /// </summary>
    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}
