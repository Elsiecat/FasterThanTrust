using UnityEngine;

/// <summary>
/// 모든 매니저를 코드상에서 동적으로 생성하고 초기화 순서를 제어하는 전역 싱글톤 컨트롤러.
/// </summary>
public class Managers : MonoBehaviour
{
    public static Managers Instance { get; private set; }

    public static GameManager Game => Instance._game;
    public static StageManager Stage => Instance._stage;
    public static UIManager UI => Instance._ui;
    public static SpawnManager Spawn => Instance._spawn;
    public static ResourcesManager Resource => Instance._resource;
    public static ObjectPoolManager Pool => Instance._pool;
    public static SkillCardManager SkillCard => Instance._skillCard;
    public static DamageIndicatorManager DamageIndicator => Instance._damageIndicator;
    public static VFXManager VFXManager => Instance._vfx;

    private GameManager _game;
    private StageManager _stage;
    private UIManager _ui;
    private SpawnManager _spawn;
    private ResourcesManager _resource;
    private ObjectPoolManager _pool;
    private SkillCardManager _skillCard;
    private DamageIndicatorManager _damageIndicator;
    public PlayerRuntimeStat _playerRuntimeStat;
    public VFXManager _vfx;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateAllManagers();
        InitAllManagers();
    }

    /// <summary>
    /// 모든 매니저들을 코드상에서 동적으로 생성하고 AddComponent 한다.
    /// </summary>
    private void CreateAllManagers()
    {
        _resource   = new GameObject("ResourcesManager").AddComponent<ResourcesManager>();
        _pool       = new GameObject("ObjectPoolManager").AddComponent<ObjectPoolManager>();
        _stage      = new GameObject("StageManager").AddComponent<StageManager>();
        _spawn      = new GameObject("SpawnManager").AddComponent<SpawnManager>();
        _game       = new GameObject("GameManager").AddComponent<GameManager>();
        _ui         = new GameObject("UIManager").AddComponent<UIManager>();
        _skillCard  = new GameObject("SkillCardManager").AddComponent<SkillCardManager>();
        _damageIndicator = new GameObject("DamageIndicatorManager").AddComponent<DamageIndicatorManager>();
        _playerRuntimeStat = new PlayerRuntimeStat();

        _resource.transform.parent = transform;
        _pool.transform.parent = transform;
        _stage.transform.parent = transform;
        _spawn.transform.parent = transform;
        _game.transform.parent = transform;
        _ui.transform.parent = transform;
        _skillCard.transform.parent = transform;
        _damageIndicator.transform.parent = transform; // 추가
    }

    /// <summary>
    /// 모든 매니저를 순서대로 초기화한다.
    /// </summary>
    private void InitAllManagers()
    {
        _resource?.Init();
        _pool?.Init();
        _spawn?.Init();
        _stage?.Init();
        _game?.Init();
        _ui?.Init();
        _skillCard?.Init();
        _damageIndicator.Init();
    }
}
