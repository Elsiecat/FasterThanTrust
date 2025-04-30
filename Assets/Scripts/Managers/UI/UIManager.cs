using UnityEngine;
using TMPro;

/// <summary>
/// UI 관련 기능(딤 배경, 스킬 카드 UI, 데미지 텍스트)을 관리하는 매니저.
/// Managers에서 Init()을 통해 호출되며, 모든 UI는 Resources에서 직접 로드된다.
/// </summary>
public class UIManager : MonoBehaviour
{
    private DimBackground _dim;
    private SkillCardGroup _skillCardGroup;

    private const string DIM_PATH = "Prefabs/UI/DimBackground";
    private const string CARD_PATH = "Prefabs/UI/SkillCardGroup";
    private const string DAMAGE_CANVAS_PATH = "Prefabs/UI/DamageTextCanvas"; // 수정: DamageTextCanvas로 변경

    private Canvas _damageTextCanvas; // 데미지 텍스트 전용 Canvas

    [Header("상태 UI")]
    [SerializeField] private TextMeshProUGUI _infectionPowerText; // 감염파워 표시 텍스트 (선택)

    /// <summary>
    /// UI 오브젝트를 Resources에서 로드 및 초기화한다.
    /// </summary>
    public void Init()
    {
        // 스크린용 UI 프리팹 로드
        GameObject dimPrefab = Resources.Load<GameObject>(DIM_PATH);
        GameObject cardPrefab = Resources.Load<GameObject>(CARD_PATH);

        // 데미지 텍스트용 Canvas 프리팹 로드
        GameObject damageCanvasPrefab = Resources.Load<GameObject>(DAMAGE_CANVAS_PATH);

        // 프리팹 로드 실패 체크
        if (dimPrefab == null)
        {
            Debug.LogError("[UIManager] DimBackground 프리팹을 찾을 수 없습니다.");
            return;
        }
        if (cardPrefab == null)
        {
            Debug.LogError("[UIManager] SkillCardGroup 프리팹을 찾을 수 없습니다.");
            return;
        }
        if (damageCanvasPrefab == null)
        {
            Debug.LogError("[UIManager] DamageTextCanvas 프리팹을 찾을 수 없습니다.");
            return;
        }

        // 스크린용 UI 세팅 (ScreenSpace-Overlay Canvas 하위)
        GameObject dimObj = Instantiate(dimPrefab, transform);
        GameObject cardObj = Instantiate(cardPrefab, transform);

        _dim = dimObj.GetComponent<DimBackground>();
        _skillCardGroup = cardObj.GetComponent<SkillCardGroup>();

        // 데미지 텍스트용 Canvas 생성
        GameObject damageCanvasObj = Instantiate(damageCanvasPrefab);
        _damageTextCanvas = damageCanvasObj.GetComponent<Canvas>();

        if (_damageTextCanvas == null)
        {
            Debug.LogError("[UIManager] 생성된 DamageTextCanvas에 Canvas 컴포넌트가 없습니다.");
            return;
        }

        // 생성된 Canvas에 Main Camera 연결
        if (_damageTextCanvas.renderMode == RenderMode.ScreenSpaceCamera && _damageTextCanvas.worldCamera == null)
        {
            _damageTextCanvas.worldCamera = Camera.main;
        }

    }

    /// <summary>
    /// 딤 배경을 보여준다.
    /// </summary>
    public void ShowDim() => _dim?.Show();

    /// <summary>
    /// 딤 배경을 숨긴다.
    /// </summary>
    public void HideDim() => _dim?.Hide();

    /// <summary>
    /// 스킬 카드 UI를 보여준다.
    /// </summary>
    public void ShowSkillCards() => _skillCardGroup?.Show();

    /// <summary>
    /// 스킬 카드 UI를 숨긴다.
    /// </summary>
    public void HideSkillCards() => _skillCardGroup?.Hide();

    /// <summary>
    /// 감염 파워 수치를 UI에 표시한다.
    /// </summary>
    public void UpdateInfectionPowerUI()
    {
        if (_infectionPowerText != null)
        {
            _infectionPowerText.text = $"💉 감염파워: {Managers.Game.PlayerStat.InfectionPower}";
        }
    }

    /// <summary>
    /// 피해를 입었을 때 데미지 텍스트를 스폰한다.
    /// </summary>

}
