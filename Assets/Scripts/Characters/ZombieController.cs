using UnityEngine;

/// <summary>
/// 기본 좀비 캐릭터를 제어하는 컨트롤러.
/// 주변의 인간을 탐지하고, 없으면 배회하며, 인간이 근처에 있으면 추적 및 공격한다.
/// </summary>
public class ZombieController : CharacterBase
{
    [Header("AI 설정")]
    [SerializeField] private float _wanderCooldown = 2f;
    [SerializeField] private float _wanderRadius = 8f;
    [SerializeField] private LayerMask _humanMask;
    [SerializeField] private LayerMask _obstacleMask;

    private float _wanderTimer;
    private Vector2 _wanderTarget;

    private Transform _targetHuman;
    private Define.ZombieState _zombieState = Define.ZombieState.Peaceful;
    private float _lastAttackTime;

    private Rigidbody2D _rb;

    /// <summary>
    /// 외부 데이터 키
    /// </summary>
    protected override string StatKey => "Stat_Zombie";
    protected override string WeaponKey => "Weapon_Zombie";

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        _humanMask = 1 << LayerMask.NameToLayer(Define.LAYER_HUMAN);
        _obstacleMask = LayerMask.GetMask("Wall", "Obstacle");

        _wanderTimer = Random.Range(0.5f, 1.5f);
    }

    private void Update()
    {
        if (!IsAlive()) return;

        UpdateState();
        ActByState();
    }

    /// <summary>
    /// 좀비의 상태를 업데이트한다 (시야 내 시민 탐색)
    /// </summary>
    private void UpdateState()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _characterStat.sightRadius, _humanMask);

        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null || hit.transform == null) continue;

            if (Physics2D.Linecast(transform.position, hit.transform.position, _obstacleMask)) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closest = hit.transform;
                closestDist = dist;
            }
        }

        if (closest != null)
        {
            _targetHuman = closest;
            _zombieState = Define.ZombieState.HumanNearby;
        }
        else
        {
            _targetHuman = null;
            _zombieState = Define.ZombieState.Peaceful;
        }
    }

    /// <summary>
    /// 상태에 따른 행동 수행
    /// </summary>
    private void ActByState()
    {
        switch (_zombieState)
        {
            case Define.ZombieState.Peaceful:
                WanderRandomly();
                break;

            case Define.ZombieState.HumanNearby:
                if (_targetHuman != null)
                {
                    MoveTo(_targetHuman.position);
                    TryAttack();
                }
                break;

            case Define.ZombieState.Suspicious:
                // 경계 모드 (미구현)
                break;
        }
    }

    /// <summary>
    /// 시민 공격 시도
    /// </summary>
    private void TryAttack()
    {
        if (_targetHuman == null || _weapon == null) return;

        float dist = Vector2.Distance(transform.position, _targetHuman.position);
        if (dist > _weapon.attackRange) return;

        float cooldown = 1f / _weapon.attackSpeed;
        if (Time.time < _lastAttackTime + cooldown) return;

        var target = _targetHuman.GetComponent<CharacterBase>();
        if (target != null && target.IsAlive())
        {
            float damage = _weapon.attackPower;
            bool isCrit = Random.value < _weapon.criticalChance;
            if (isCrit) damage *= 2f;

            target.TakeDamage(damage, this);
            target.ApplyInfection(_weapon, this);

            _lastAttackTime = Time.time;
        }
    }

    /// <summary>
    /// 무작위 위치로 배회
    /// </summary>
    private void WanderRandomly()
    {
        _wanderTimer -= Time.deltaTime;
        if (_wanderTimer <= 0f)
        {
            _wanderTimer = _wanderCooldown;
            Vector2 random = Random.insideUnitCircle.normalized * _wanderRadius;
            _wanderTarget = (Vector2)transform.position + random;
        }

        MoveTo(_wanderTarget);
    }

    /// <summary>
    /// 대상 지점으로 이동
    /// </summary>
    public override void MoveTo(Vector2 target)
    {
        if (_characterStat == null || _rb == null) return;

        Vector2 direction = (target - _rb.position).normalized;
        Vector2 newPos = _rb.position + direction * _characterStat.moveSpeed * Time.deltaTime;

        _rb.MovePosition(newPos);
    }

    /// <summary>
    /// 사망 처리
    /// </summary>
    protected override void HandleDeathOutcome()
    {
        // TODO: 시체 비주얼 이펙트 처리 등 추가 가능
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_weapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _weapon.attackRange);
        }

        if (_characterStat != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _characterStat.sightRadius);
        }
    }
#endif
}
