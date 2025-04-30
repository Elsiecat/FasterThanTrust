using UnityEngine;

/// <summary>
/// 게임 전체의 상태를 관리하는 매니저.
/// 게임 흐름, 감염 시작/종료, 감염파워, 계정 경험치 정산 등을 담당한다.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("게임 상태")]
    [SerializeField] private Define.GameState _currentState = Define.GameState.Init;

    /// <summary>현재 게임 상태 (Init, 대기, 진행중, 종료)</summary>
    public Define.GameState CurrentState => _currentState;

    /// <summary>플레이어의 인게임 런타임 스탯</summary>
    public PlayerRuntimeStat PlayerStat { get; private set; } = new();

    /// <summary>플레이어의 장기 계정 데이터</summary>
    public PlayerAccountData AccountData { get; private set; } = new();

    /// <summary>게임 상태 및 스탯 초기화</summary>
    public void Init()
    {
        _currentState = Define.GameState.WaitingForInfection;
        PlayerStat.Init();
    }

    /// <summary>감염 시작 시 호출됨. UI 닫기, 상태 전환 처리.</summary>
    public void StartInfection()
    {
        _currentState = Define.GameState.Playing;
        Managers.UI.HideSkillCards();
    }

    /// <summary>감염 파워 수동 증가 (예: 스킬로 인한 증가)</summary>
    public void IncreaseInfectionPower()
    {
        PlayerStat.IncreaseInfectionPower();
    }

    /// <summary>게임 종료 시 호출. 계정 경험치 정산 포함.</summary>
    public void EndGame()
    {
        _currentState = Define.GameState.GameOver;

        int clearedStage = Managers.Stage.CurrentStage.stageId + 1;

        // 계정 경험치 및 통계 정산
        AccountData.AddExp(clearedStage);
        AccountData.RecordStage(clearedStage);

        Debug.Log($"✅ 게임 종료: 클리어 스테이지 = {clearedStage}, 계정레벨 = {AccountData.Level}, 감염파워 = {PlayerStat.InfectionPower}");
    }
}
