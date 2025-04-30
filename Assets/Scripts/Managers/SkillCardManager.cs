using UnityEngine;

/// <summary>
/// 스킬 카드 UI를 관리하며, 플레이어가 선택한 스킬을 런타임 스탯에 반영한다.
/// </summary>
public class SkillCardManager : MonoBehaviour
{
    private SkillCard[] _cards;
    public void Init()
    {
        // 현재는 카드 프리셋 로딩 등의 확장용
    }
    private void Awake()
    {
        _cards = GetComponentsInChildren<SkillCard>(true);
    }

    /// <summary>
    /// 카드 선택 시 호출되는 메서드. 스킬을 플레이어 런타임 스탯에 등록하고 UI를 종료한다.
    /// </summary>
    /// <param name="skillId">선택된 스킬 ID</param>
    public void OnSkillSelected(string skillId)
    {
        if (string.IsNullOrEmpty(skillId))
        {
            Debug.LogWarning("[SkillCardManager] 스킬 ID가 유효하지 않습니다.");
            return;
        }

        // 플레이어에 스킬 등록
        Managers.Game.PlayerStat.AddSkill(skillId);

        // UI 닫기
        Managers.UI.HideDim();
        Managers.UI.HideSkillCards();

        // 감염파워 증가 예시 (필요시)
        // Managers.Game.PlayerStat.IncreaseInfectionPower();

        Debug.Log($"🃏 스킬 획득: {skillId}");
    }
}
