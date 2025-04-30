using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 인게임 도중 실시간으로 변화하는 플레이어의 상태를 관리한다.
/// 감염 파워, 보유 스킬, 적용된 효과 등을 포함한다.
/// </summary>
public class PlayerRuntimeStat
{
    /// <summary>현재 감염 파워 (감염 시도 시 면역력과 비교)</summary>
    public int InfectionPower { get; private set; } = 0;

    /// <summary>마우스  </summary>
    public int ClickInfectionCount { get; private set; } = 1;

    /// <summary>플레이어가 현재 보유 중인 스킬 키 목록</summary>
    private List<string> _skillIds = new();

    /// <summary>보유 중인 스킬 키들을 반환</summary>
    public IReadOnlyList<string> SkillIds => _skillIds;

    /// <summary>현재 인게임 세션을 초기화한다.</summary>
    public void Init()
    {
        InfectionPower = 1;
        ClickInfectionCount = 1;
        _skillIds.Clear();
    }

    /// <summary>감염 파워를 증가시킨다.</summary>
    public void IncreaseInfectionPower(int amount = 1)
    {
        InfectionPower += amount;
    }

    public void IncreaseClickInfectionCount(int amount = 1)
    {
        ClickInfectionCount += amount;
    }

    public void DecreaseClickInfectionCount(int amount = 1)
    {
        ClickInfectionCount = Mathf.Max(ClickInfectionCount - amount, 0);
    }

    /// <summary>스킬을 획득한다.</summary>
    public void AddSkill(string skillId)
    {
        if (!_skillIds.Contains(skillId))
            _skillIds.Add(skillId);
    }

    /// <summary>특정 스킬을 보유하고 있는지 여부</summary>
    public bool HasSkill(string skillId)
    {
        return _skillIds.Contains(skillId);
    }

}
