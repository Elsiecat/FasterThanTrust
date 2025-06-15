using UnityEngine;
using Utils;
using Combat;

/// <summary>
/// 캐릭터의 공통 기능을 담당하는 베이스 클래스.
/// 체력, 상태, 무기, 스탯 등을 포함하며, NavMesh 없이 직접 이동 처리한다.
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] protected int _level = 1; // 캐릭터 레벨
    [SerializeField] protected CharacterStat _characterStat; // 캐릭터 능력치
    [SerializeField] protected Weapon _weapon; // 무기 정보 (ScriptableObject)

    [Header("상태")]
    [SerializeField] protected float _currentHP; // 현재 체력
    [SerializeField] protected CharacterState _state = CharacterState.Alive; // 현재 캐릭터 상태
    [SerializeField] protected InfectionDOT _infectionDOT; // 감염 DOT 효과
    protected DamageIndicatorRoot _damageIndicatorRoot;

    protected Rigidbody2D _rigid;
    protected Collider2D _col; // 충돌 판정용
    protected SpriteRenderer _spriteRenderer; //처맞았을때 깜빡!하게 할 용도

    // 자식 클래스에서 오버라이딩할 Stat/Weapon 키값
    protected virtual string StatKey => null;
    protected virtual string WeaponKey => null;

    // 플래시용 머티리얼 관리
    private Material _originalMaterial;
    private Material _flashMaterial;

    /// <summary>
    /// 기본 Awake: Collider 초기화, 스탯/무기 로드 및 체력 설정
    /// </summary>
    protected virtual void Awake()
    {
        _col = GetComponent<Collider2D>();
        _rigid = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flashMaterial = Resources.Load<Material>("Materials/SpriteFlashMaterial");


        if (_spriteRenderer != null)
            _originalMaterial = _spriteRenderer.material;

        if (_characterStat == null && !string.IsNullOrEmpty(StatKey))
            _characterStat = CharacterStatLoader.LoadStat(StatKey);

        if (_weapon == null && !string.IsNullOrEmpty(WeaponKey))
            _weapon = CharacterStatLoader.LoadWeapon(WeaponKey);

        if (_characterStat != null)
            _currentHP = _characterStat.maxHp;
    }


    /// <summary>
    /// 피해를 입으면 체력을 감소시키고 죽음 여부를 판단한다.
    /// </summary>
    public virtual void TakeDamage(float rawDamage, CharacterBase attacker)
    {

        
        if (_state == CharacterState.Dead)
            return;

        if (_characterStat != null && DamageCalculator.TryEvade(_characterStat.evasion))
            return;

        float defense = _characterStat != null ? _characterStat.defense : 0;

        float critRate = 0f;
        if (attacker != null && attacker._weapon != null)
            critRate = attacker._weapon.criticalChance;

        var (finalDamage, isCritical) = DamageCalculator.CalculateFinalDamage(rawDamage, (int)defense, _level, critRate);

        //현재체력 - 마지막 데미지
        _currentHP -= finalDamage;
        //입은 데미지를 표현해주는 VFX (텍스트 위로 떠오르는거
        Managers.DamageIndicator.SpawnDamageIndicator(transform.position, Mathf.RoundToInt(finalDamage), isCritical);

        //피해를 입었을 때의 이펙트를 띄워주기 위함
        CombatEventHub.RaiseHit(new HitEventArgs
        {
            HitPosition = transform.position,
            WeaponUsed = attacker._weapon
        });

        HitFlasher.Flash(_spriteRenderer, _flashMaterial, _originalMaterial, this);

        if (attacker != null && attacker._weapon != null)
        {
            //무기에 맞았으면 무조건 '감염여부'판단
            ApplyInfection(attacker._weapon);
        }

        if (_currentHP <= 0)
            Die();
    }



    /// <summary>
    /// 감염 DOT 효과를 적용한다.
    /// </summary>
    public virtual void ApplyInfection(Weapon attackerWeapon)
    {
        if (_state != CharacterState.Alive) return;

        if (Random.value <= attackerWeapon.infectionChance)
        {
          //  Debug.Log("감염시도됨");
            _state = CharacterState.Infected;
            _infectionDOT = attackerWeapon.CreateDOT();
            StartCoroutine(_infectionDOT.StartDOT(this));
        }
    }



    /// <summary>
    /// 캐릭터가 죽었을 때의 처리: DOT 중지, 충돌 해제, 후속 행동 호출
    /// </summary>
    public virtual void Die()
    {
        if (_state == CharacterState.Dead) return;

        _state = CharacterState.Dead;

        if (_infectionDOT != null)
            StopCoroutine(_infectionDOT.StartDOT(this));

        if (_col != null)
            _col.enabled = false;
        
        HandleDeathOutcome();
    }

    /// <summary>
    /// 외부에서 목적지를 지정해 이동시키는 메서드
    /// </summary>
    public virtual void MoveTo(Vector2 target)
    {
        if (_characterStat == null || _rigid == null) return;

        // 경계 Clamp 처리
        Vector2 clampedTarget = new Vector2(
            Mathf.Clamp(target.x, TilemapFloorGenerator.PlayableAreaBounds.min.x, TilemapFloorGenerator.PlayableAreaBounds.max.x),
            Mathf.Clamp(target.y, TilemapFloorGenerator.PlayableAreaBounds.min.y, TilemapFloorGenerator.PlayableAreaBounds.max.y)
        );

        transform.position = Vector2.MoveTowards(
            _rigid.position,
            clampedTarget,
            _characterStat.moveSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// 캐릭터가 살아있는 상태인지 확인
    /// </summary>
    public bool IsAlive() => _state != CharacterState.Dead;

    /// <summary>
    /// 죽었을 때 처리할 캐릭터별 개별 행동 (상속 클래스에서 정의)
    /// </summary>
    protected abstract void HandleDeathOutcome();

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_characterStat == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _characterStat.sightRadius);
    }
#endif
}
