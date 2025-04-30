using UnityEngine;

/// <summary>
/// 플레이어의 장기 계정 데이터를 관리한다.
/// 레벨, 경험치, 통계 등을 포함하며, 게임 종료 시 정산된다.
/// </summary>
[System.Serializable]
public class PlayerAccountData
{
    // 최대 레벨 (기본 10, 추후 확장 가능)
    private const int MAX_LEVEL = 10;
    private const int EXP_PER_LEVEL = 10;

    /// <summary>현재 계정 레벨</summary>
    public int Level { get; private set; } = 1;

    /// <summary>현재 누적 경험치</summary>
    public int CurrentExp { get; private set; } = 0;

    /// <summary>최고 기록: 도달한 최대 스테이지</summary>
    public int MaxStageReached { get; private set; } = 0;

    /// <summary>레벨업 시 해금 조건 판별 용도</summary>
    public bool IsLevelMax => Level >= MAX_LEVEL;

    /// <summary>현재 레벨업에 필요한 경험치</summary>
    public int ExpToNextLevel => IsLevelMax ? 0 : EXP_PER_LEVEL;

    /// <summary>
    /// 경험치를 추가하고 필요 시 자동 레벨업을 수행한다.
    /// </summary>
    public void AddExp(int amount)
    {
        if (IsLevelMax) return;

        CurrentExp += amount;

        while (CurrentExp >= EXP_PER_LEVEL && Level < MAX_LEVEL)
        {
            CurrentExp -= EXP_PER_LEVEL;
            Level++;
        }

        if (Level >= MAX_LEVEL)
        {
            CurrentExp = 0;
        }
    }

    /// <summary>
    /// 스테이지 종료 시 최고 기록을 갱신한다.
    /// </summary>
    public void RecordStage(int stageNumber)
    {
        if (stageNumber > MaxStageReached)
            MaxStageReached = stageNumber;
    }

    /// <summary>
    /// 계정 데이터 초기화 (디버그용 또는 리셋 기능)
    /// </summary>
    public void Reset()
    {
        Level = 1;
        CurrentExp = 0;
        MaxStageReached = 0;
    }
}
