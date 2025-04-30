using UnityEngine;

/// <summary>
/// 스킬 카드 정보를 담는 ScriptableObject. 스킬 효과 및 연동 ID를 포함한다.
/// </summary>
[CreateAssetMenu(menuName = "Data/SkillCardData")]
public class SkillCardData : ScriptableObject
{
    public string title;
    public string description;
    public Sprite icon;

    public int infectionPowerBonus; // 감염 파워 증가치
    public int levelRequirement;    // 계정 레벨 해금 조건
}
