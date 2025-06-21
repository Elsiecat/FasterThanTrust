using UnityEngine;

/// <summary>
/// 시민 캐릭터를 제어하는 클래스.
/// 좀비가 없을 땐 배회, 있을 땐 좀비로부터 멀어지는 방향으로 도망.
/// </summary>
public class CitizenController : CharacterBase
{
    [Header("AI 설정")]
    [SerializeField] private float _wanderCooldown = 2f;
    [SerializeField] private float _escapeRadius = 8f;
    [SerializeField] private int _escapeSampleCount = 16;
    [SerializeField] private LayerMask _zombieMask;
    [SerializeField] private LayerMask _obstacleMask;
   

    private float _wanderTimer;
    private Vector2 _currentTarget;
    [SerializeField]private Define.HumanState _humanState = Define.HumanState.Peaceful;

    /// <summary>현재 시민의 상태를 반환</summary>
    public Define.HumanState CurrentHumanState => _humanState;

    // 좀비가 주변에 있는지 없는지 판별
    private Transform _lastSeenZombie = null;
    private bool _needsNewTarget = true;

    private void Start()
    {
        _wanderTimer = Random.Range(0f, _wanderCooldown);
        Vector2 offset = Random.insideUnitCircle.normalized * _escapeRadius;
        _currentTarget = (Vector2)transform.position + offset;
    }

    private void Update()
    {
        if (!IsAlive()) return;

        UpdateState();

        switch (_humanState)
        {
            case Define.HumanState.Peaceful:
                Wander();
                break;

            case Define.HumanState.ZombieNearby:
                // 현재 타겟까지 거의 도달했거나, 새로운 좀비가 감지되었을 때만 갱신
                float dist = Vector2.Distance(transform.position, _currentTarget);
                bool closeToTarget = dist < 0.2f;

                if (_needsNewTarget || closeToTarget)
                {
                    DecideEscapeTarget();
                    _needsNewTarget = false;
                }
                break;

            case Define.HumanState.Suspicious:
                // 추후 추가
                break;
        }

        MoveTo(_currentTarget);
    }

    /// <summary>
    /// 주변 좀비를 탐색하고 시민 상태를 갱신한다.
    /// </summary>
    private void UpdateState()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _characterStat.sightRadius, _zombieMask);

        Transform nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null || hit.transform == null) continue;

            RaycastHit2D check = Physics2D.Linecast(transform.position, hit.transform.position, _obstacleMask);
            if (check.collider != null) continue;

            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < nearestDist)
            {
                nearest = hit.transform;
                nearestDist = dist;
            }
        }
        if (nearest != null)
        {
            if (_lastSeenZombie == null || nearest != _lastSeenZombie)
            {
                _needsNewTarget = true; // 새로운 좀비 발견
            }

            _lastSeenZombie = nearest;
        }
        else
        {
            _lastSeenZombie = null;
        }
        _humanState = nearest != null ? Define.HumanState.ZombieNearby : Define.HumanState.Peaceful;
    }

    /// <summary>
    /// 좀비와 가장 멀어지는 방향을 탐색하여 이동 타겟을 설정한다.
    /// </summary>
    private void DecideEscapeTarget()
    {
        Vector2 origin = transform.position;
        Vector2 bestPoint = origin;
        float bestScore = float.MinValue;

        for (int i = 0; i < _escapeSampleCount; i++)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            Vector2 candidate = origin + dir * _escapeRadius;

            // 시야 내 좀비와 거리 측정
            Collider2D[] zombies = Physics2D.OverlapCircleAll(candidate, _characterStat.sightRadius, _zombieMask);
            float minDist = float.MaxValue;

            foreach (var z in zombies)
            {
                float dist = Vector2.Distance(candidate, z.transform.position);
                if (dist < minDist) minDist = dist;
            }

            // 가중치 계산: 좀비로부터 멀수록 점수 증가
            float score = minDist;

            // 장애물 확인
            RaycastHit2D hit = Physics2D.Linecast(origin, candidate, _obstacleMask);
            if (hit.collider != null) score -= 10f;

            if (score > bestScore)
            {
                bestScore = score;
                bestPoint = candidate;
            }
        }

        _currentTarget = bestPoint;
        _wanderTimer = _wanderCooldown; // 이동 리셋
    }

    /// <summary>
    /// 지정된 위치로 이동을 수행한다.
    /// </summary>
    /// <param name="target">이동 목표 지점</param>
    public override void MoveTo(Vector2 target)
    {
        Vector2 current = _rigid.position;
        Vector2 dir = (target - current).normalized;
        Vector2 next = current + dir * _characterStat.moveSpeed * Time.deltaTime;

        _rigid.MovePosition(next);
    }
    /// <summary>
    /// 주변을 무작위로 배회한다.
    /// </summary>
    private void Wander()
    {
        _wanderTimer -= Time.deltaTime;
        if (_wanderTimer > 0f) return;

        _wanderTimer = _wanderCooldown;
        Vector2 offset = Random.insideUnitCircle.normalized * _escapeRadius;
        _currentTarget = (Vector2)transform.position + offset;
    }


    private void OnMouseDown()
    {
        if (Managers.Game.PlayerStat.ClickInfectionCount <= 0)
        {
            //클릭감염 횟수가 없을 경우에는 그냥 바로 리턴 시킴
            Debug.Log("💉 클릭감염 횟수가 없을 경우에는 그냥 바로 리턴 시킴");
            return;
        }

        if (_state != Define.CharacterState.Alive)
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
        if (_state !=  Define.CharacterState.Alive) return;

        _state = Define.CharacterState.Infected;
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
        if (_state == Define.CharacterState.Dead) return;

        if (_state == Define.CharacterState.Infected)
        {
            Managers.Spawn.SpawnZombie(transform.position);
        }

        _state = Define.CharacterState.Dead;

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

#if UNITY_EDITOR
    /// <summary>
    /// 디버깅용 시야 및 타겟 시각화
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _characterStat != null ? _characterStat.sightRadius : 5f);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, _currentTarget);
        Gizmos.DrawWireSphere(_currentTarget, 0.2f);
    }
#endif
}
