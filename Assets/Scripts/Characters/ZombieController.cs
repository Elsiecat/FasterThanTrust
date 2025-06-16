using UnityEngine;

/// <summary>
/// 기본 좀비 캐릭터를 제어하는 컨트롤러.
/// 주변의 인간을 탐지하고, 없으면 배회하며, 인간이 근처에 있으면 추적 및 공격한다.
/// </summary>
public class ZombieController : CharacterBase
{
    [SerializeField] private LayerMask _humanMask;
    [SerializeField] private LayerMask _obstacleMask;

    private Transform _targetHuman;
    private Define.ZombieState _zombieState = Define.ZombieState.Peaceful;
    private float _lastAttackTime;

    [SerializeField] private float _wanderCooldown = 2f;  // 이동 타겟 갱신 주기
    [SerializeField] private float _wanderRadius = 10f;   // 배회 반경
    private float _wanderTimer;
    private Vector2 _currentWanderTarget;


    private void Update()
    {
        if (!IsAlive()) return;

        UpdateState();
        ActByState();
    }

    /// <summary>
    /// 시야 내 인간을 감지하고, 상태를 결정한다.
    /// </summary>
    private void UpdateState()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _characterStat.sightRadius, _humanMask);

        Transform closest = null;
        float closestDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null || hit.transform == null) continue;

            RaycastHit2D check = Physics2D.Linecast(transform.position, hit.transform.position, _obstacleMask);
            if (check.collider != null) continue;

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
    /// 좀비의 상태에 따라 행동을 수행한다.
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
                // 추후 경계 행동 추가 가능
                break;
        }
    }

    /// <summary>
    /// 일정 범위 내의 인간을 공격한다.
    /// </summary>
    private void TryAttack()
    {
        if (_targetHuman == null || _weapon == null) return;

        float dist = Vector2.Distance(transform.position, _targetHuman.position);
        if (dist > _weapon.attackRange) return;

        float attackCooldown = 1f / _weapon.attackSpeed;
        if (Time.time < _lastAttackTime + attackCooldown) return;

        var target = _targetHuman.GetComponent<CharacterBase>();
        if (target != null && target.IsAlive())
        {
            float finalDamage = _weapon.attackPower;

            // 치명타 처리
            bool isCrit = Random.value < _weapon.criticalChance;
            if (isCrit)
            {
                finalDamage *= 2f;
            //    Debug.Log("💥 치명타 발생!");
            }

            target.TakeDamage(finalDamage, this);
            target.ApplyInfection(_weapon, this); // ← 감염 시도 추가
            _lastAttackTime = Time.time;

          //  Debug.Log($"🧟 좀비가 {target.name}을 공격했습니다. 피해량: {finalDamage}");
        }
    }

    /// <summary>
    /// 주변을 무작위로 배회한다.
    /// </summary>
    private void WanderRandomly()
    {
    // 타겟 갱신 타이머
    _wanderTimer -= Time.deltaTime;
    if (_wanderTimer <= 0f)
    {
        _wanderTimer = _wanderCooldown;
        _currentWanderTarget = (Vector2)transform.position + Random.insideUnitCircle.normalized * _wanderRadius;
    }

    // 갱신된 타겟으로 이동
    MoveTo(_currentWanderTarget);
}
    /// <summary>
    /// 좀비 사망 처리.
    /// </summary>
    protected override void HandleDeathOutcome()
    {
        Debug.Log("좀비가 사망했습니다. 시체로 남습니다.");
        // TODO: 시체 애니메이션 등 추가 가능
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_weapon != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _weapon.attackRange);
        }
    }
#endif
}
