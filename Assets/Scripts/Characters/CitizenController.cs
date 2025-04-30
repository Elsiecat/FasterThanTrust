using UnityEngine;

/// <summary>
/// 기본 시민 캐릭터를 제어하는 컨트롤러.
/// 좀비가 시야에 들어오면 회피하고, 그렇지 않으면 맵 내에서 랜덤 배회.
/// NavMesh 없이, 직접 맵 좌표 기준으로 이동.
/// </summary>
public class CitizenController : CharacterBase
{
    [Header("AI 설정")]
    //[SerializeField] private float _wanderCooldown = 2f;
    [SerializeField] private LayerMask _zombieMask;
    [SerializeField] private LayerMask _obstacleMask;
    
    [SerializeField] private float _fleeMaintainDuration = 1.5f; // 좀비 안 보여도 도망 유지 시간

    private float _fleeMaintainTimer = 0f; // 도망 유지 타이머
    private float _wanderTimer;
    private Vector2 _lastWanderTarget;
    private bool _hasFirstWanderTarget = false;

    private Rigidbody2D _rb;
    private Define.HumanState _humanState = Define.HumanState.Peaceful;

    protected override string StatKey => "Stat_Citizen";
    protected override string WeaponKey => "Weapon_Citizen";

    /// <summary>
    /// 기본 초기화: Rigidbody 설정, 레이어 설정 등
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        if (_rb == null)
            _rb = gameObject.AddComponent<Rigidbody2D>();

        _rb.gravityScale = 0;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        _zombieMask = 1 << LayerMask.NameToLayer(Define.LAYER_ZOMBIE);
        _obstacleMask = 1 << LayerMask.NameToLayer("Wall");

        _wanderTimer = Random.Range(0.5f, 1.5f);

        // 초기 랜덤 목표 위치 설정
        Vector2 randomOffset = Random.insideUnitCircle.normalized * 2f;
        _lastWanderTarget = _rb.position + randomOffset;
    }

    private void Update()
    {
        if (_state != CharacterState.Alive) return;

        if (!_hasFirstWanderTarget)
        {
            _hasFirstWanderTarget = true;
            SetRandomWanderTarget();
        }

        UpdateState();

        switch (_humanState)
        {
            case Define.HumanState.Peaceful:
                Wander();
                break;

            case Define.HumanState.ZombieNearby:
                Flee();
                break;
        }
    }

    /// <summary>
    /// 좀비 감지 및 상태 전환 관리
    /// </summary>
    private void UpdateState()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _characterStat.sightRadius, _zombieMask);
        bool zombieVisible = false;

        foreach (var hit in hits)
        {
            if (hit == null || hit.transform == null) continue;
            RaycastHit2D check = Physics2D.Linecast(transform.position, hit.transform.position, _obstacleMask);
            if (check.collider != null) continue;
            zombieVisible = true;
            break;
        }

        if (zombieVisible)
        {
            _humanState = Define.HumanState.ZombieNearby;
            _fleeMaintainTimer = _fleeMaintainDuration;
        }
        else
        {
            if (_fleeMaintainTimer > 0f)
            {
                _fleeMaintainTimer -= Time.deltaTime;
                _humanState = Define.HumanState.ZombieNearby;
            }
            else
            {
                _humanState = Define.HumanState.Peaceful;
            }
        }
    }

    /// <summary>
    /// 랜덤 배회 이동 처리
    /// </summary>
    private void Wander()
    {
        if (_wanderTimer > 0f)
        {
            _wanderTimer -= Time.deltaTime;
            MoveTo(_lastWanderTarget);
        }
        else
        {
            _wanderTimer = Random.Range(1.5f, 3f);
            Vector2 randomOffset = Random.insideUnitCircle.normalized * 2f;
            _lastWanderTarget = _rb.position + randomOffset;
        }
    }


    /// <summary>
    /// 좀비 발견 시 도망 이동 처리
    /// </summary>
    private void Flee()
    {
        Vector2 fleeDir = GetFleeDirection();
        if (fleeDir == Vector2.zero) return;

        Vector2 fleeTarget = _rb.position + fleeDir * 3f;
        MoveTo(fleeTarget);
    }

    /// <summary>
    /// 도망 방향 계산
    /// </summary>
    private Vector2 GetFleeDirection()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _characterStat.sightRadius, _zombieMask);
        Vector2 fleeDir = Vector2.zero;

        foreach (var hit in hits)
        {
            if (hit == null || hit.transform == null) continue;
            RaycastHit2D check = Physics2D.Linecast(transform.position, hit.transform.position, _obstacleMask);
            if (check.collider != null) continue;

            Vector2 toZombie = (Vector2)hit.transform.position - _rb.position;
            float dist = toZombie.magnitude;
            if (dist == 0) continue;

            Vector2 away = -toZombie.normalized / dist;
            fleeDir += away;
        }

        return fleeDir.normalized;
    }

    /// <summary>
    /// 타겟 좌표로 이동 처리
    /// </summary>
    public override void MoveTo(Vector2 target)
    {
        if (_characterStat == null || _rb == null) return;

        float moveDelta = _characterStat.moveSpeed * Time.fixedDeltaTime;

        Vector2 clampedTarget = new Vector2(
            Mathf.Clamp(target.x, TilemapFloorGenerator.PlayableAreaBounds.min.x, TilemapFloorGenerator.PlayableAreaBounds.max.x),
            Mathf.Clamp(target.y, TilemapFloorGenerator.PlayableAreaBounds.min.y, TilemapFloorGenerator.PlayableAreaBounds.max.y)
        );

        Vector2 newPos = Vector2.MoveTowards(_rb.position, clampedTarget, moveDelta);
        _rb.MovePosition(newPos);
    }

    private void OnMouseDown()
    {
        if (Managers.Game.PlayerStat.ClickInfectionCount <= 0)
        {
            //클릭감염 횟수가 없을 경우에는 그냥 바로 리턴 시킴
            Debug.Log("💉 클릭감염 횟수가 없을 경우에는 그냥 바로 리턴 시킴");
            return;
        }

        if (_state != CharacterState.Alive)
        {  
             Debug.Log("💉 살아있지 않은 시민의 경우 리턴 시킴");
            //살아있지 않은 시민의 경우 리턴 시킴
            return;
        }

        if (Managers.Game.PlayerStat.InfectionPower >= GetResistance())
        {
            Managers.Game.StartInfection();
            ClickInfection();
        }
        else
        {
            Debug.Log("💉 감염 실패 (면역력이 더 높음)");
        }
    }

   /// <summary>
    /// 클릭 감염 처리
    /// </summary>
    private void ClickInfection()
    {
        if (_state != CharacterState.Alive) return;

        _state = CharacterState.Infected;
        Die();
    }

    /// <summary>
    /// 기본 저항 수치 반환 (0)
    /// </summary>
    private int GetResistance() => 0;

    /// <summary>
    /// 시민 사망 처리
    /// </summary>
    public override void Die()
    {
        if (_state == CharacterState.Dead) return;

        if (_state == CharacterState.Infected)
        {
            Managers.Spawn.SpawnZombie(transform.position);
        }

        _state = CharacterState.Dead;

        if (_infectionDOT != null)
            StopCoroutine(_infectionDOT.StartDOT(this));

        if (_col != null)
            _col.enabled = false;

        HandleDeathOutcome();
    }
    /// <summary>
    /// 사망 후 처리 (오브젝트 제거)
    /// </summary>
    protected override void HandleDeathOutcome()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 랜덤 배회 목표 설정
    /// </summary>
    private void SetRandomWanderTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle.normalized * 2f;
        _lastWanderTarget = _rb.position + randomOffset;
    }

}
