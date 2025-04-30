using UnityEngine;

/// <summary>
/// 스킬 카드 UI 그룹을 제어하는 컴포넌트.
/// Show/Hide를 통해 UI 등장 및 퇴장 애니메이션 등을 제어할 수 있다.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class SkillCardGroup : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 카드 그룹을 보여줌 (알파 = 1, 입력 허용)
    /// </summary>
    public void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    /// <summary>
    /// 카드 그룹을 숨김 (알파 = 0, 입력 차단)
    /// </summary>
    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}
