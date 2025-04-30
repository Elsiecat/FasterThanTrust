using UnityEngine;

/// <summary>
/// 스테이지 데이터를 로딩하고, 바닥 생성 및 스폰 영역 초기화까지 담당하는 매니저.
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private int _currentStageId = 1; // 현재 스테이지 ID
    private StageData _currentStage; // 현재 스테이지 데이터

    /// <summary>현재 스테이지 데이터</summary>
    public StageData CurrentStage => _currentStage;

    /// <summary>
    /// 시작 시 스테이지 데이터를 로드한다.
    /// </summary>
    /// 
    /// 
    public void Init()
    {
        // 현재는 특별히 초기화할 내용 없음
        LoadStage(_currentStageId);

    // ✅ 여기 추가
    CameraController cameraController = Camera.main.GetComponent<CameraController>();
    if (cameraController != null)
    {
        cameraController.InitializeCamera(_currentStage.spawnAreaSize);
    }
    }
    private void Awake()
    {
        
    }

    /// <summary>
    /// 지정된 ID에 해당하는 스테이지를 로드하고, 바닥 및 스폰 범위를 초기화한다.
    /// </summary>
    public void LoadStage(int stageId)
    {
        _currentStageId = stageId;
        _currentStage = Resources.Load<StageData>($"StageData/Stage_{stageId}");

        if (_currentStage == null)
        {
            Debug.LogError($"[StageManager] StageData/Stage_{stageId} not found.");
            return;
        }

        // 바닥 타일 자동 생성
        TilemapFloorGenerator.Generate(_currentStage.spawnAreaSize, _currentStage.spawnAreaCenter);

        // 시민 스폰 범위 설정
        Managers.Spawn.InitSpawnArea(_currentStage.spawnAreaCenter, _currentStage.spawnAreaSize);

        // ✅ 시민 스폰 실행 (정확히 _currentStage.citizenCount 사용)
        Managers.Spawn.SpawnCitizens();
    }


    /// <summary>
    /// 다음 스테이지로 넘어가며, 계정 통계 정산도 수행된다.
    /// </summary>
    public void NextStage()
    {
        // 통계: 최고 기록 갱신
        Managers.Game.AccountData.RecordStage(_currentStageId + 1);

        // 다음 스테이지 로드
        LoadStage(_currentStageId + 1);
    }
}
