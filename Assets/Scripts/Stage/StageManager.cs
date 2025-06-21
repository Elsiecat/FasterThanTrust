using UnityEngine;

/// <summary>
/// 스테이지 데이터를 로딩하고, 맵 프리팹을 불러오는 역할을 담당하는 매니저.
/// </summary>
public class StageManager : MonoBehaviour
{
    [SerializeField] private int _currentStageId = 1; // 현재 스테이지 ID
    private StageData _currentStage;                  // 현재 스테이지 데이터
    private GameObject _mapInstance;                  // 현재 활성화된 맵 프리팹 인스턴스

    /// <summary>
    /// 현재 로드된 스테이지 데이터 참조
    /// </summary>
    public StageData CurrentStage => _currentStage;

    /// <summary>
    /// 게임 시작 시 호출됨. 최초 스테이지를 로딩한다.
    /// </summary>
    public void Init()
    {
        LoadStage(_currentStageId);

        // ✅ 기존 랜덤맵 전용 카메라 설정 → 더 이상 사용 안 함 (남겨만 둠)
        /*
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.InitializeCamera(_currentStage.spawnAreaSize);
        }
        */
    }

    /// <summary>
    /// 주어진 스테이지 ID에 맞는 StageData를 불러오고, 맵을 생성한다.
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

        // ✅ 이전 맵 프리팹 인스턴스가 있으면 제거
        if (_mapInstance != null)
        {
            Destroy(_mapInstance);
            _mapInstance = null;
        }

        // ✅ 직접 만든 맵 프리팹 로드 및 인스턴스화
        if (!string.IsNullOrEmpty(_currentStage.mapPrefabPath))
        {
            GameObject prefab = Resources.Load<GameObject>(_currentStage.mapPrefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[StageManager] 맵 프리팹 로드 실패: {_currentStage.mapPrefabPath}");
                return;
            }

            _mapInstance = Instantiate(prefab);
        }

        // ✅ 랜덤맵 기반 로직은 더 이상 사용하지 않지만, 참고용으로 주석만 남겨둠
        /*
        // 바닥 타일 자동 생성
        TilemapFloorGenerator.Generate(_currentStage.spawnAreaSize, _currentStage.spawnAreaCenter);

        // 시민 스폰 범위 설정
        Managers.Spawn.InitSpawnArea(_currentStage.spawnAreaCenter, _currentStage.spawnAreaSize);

        // 시민 스폰 실행
        Managers.Spawn.SpawnCitizens();
        */
    }

    /// <summary>
    /// 다음 스테이지로 넘어갈 때 호출됨. 통계 정산 후 다음 스테이지 로드.
    /// </summary>
    public void NextStage()
    {
        // 통계 처리: 다음 스테이지 도달 기록 저장
        Managers.Game.AccountData.RecordStage(_currentStageId + 1);

        // 다음 스테이지 로드
        LoadStage(_currentStageId + 1);
    }
}
