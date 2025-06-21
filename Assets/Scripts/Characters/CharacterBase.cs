using UnityEngine;
using Utils;
using Combat;

/// <summary>
/// 캐릭터의 기본 동작과 상태를 제어하는 베이스 클래스.
/// 시민과 좀비 모두 이 클래스를 상속받아 동작.
/// </summary>
public abstract class CharacterBase : MonoBehaviour
{
    [Header("기본 스탯")]
    [SerializeField] protected int _level = 1; // 캐릭터 레벨
    [SerializeField] protected CharacterStat _characterStat; // 캐릭터 스탯
    [SerializeField] protected Weapon _weapon;               // 장착 무기

    [Header("상태")]
    [SerializeField] protected float _currentHP; // 현재 체력
    [SerializeField] protected InfectionDOT _infectionDOT; // 감염 DOT 효과
    [SerializeField] protected Define.CharacterState _state = Define.CharacterState.Alive; // 현재 캐릭터 상태
    protected DamageIndicatorRoot _damageIndicatorRoot;

    protected Rigidbody2D _rigid;
    protected BoxCollider2D _col; // 충돌 판정용
    protected SpriteRenderer _spriteRenderer; //처맞았을때 깜빡!하게 할 용도


    // 자식 클래스에서 오버라이딩할 Stat/Weapon 키값
    protected virtual string StatKey => null;
    protected virtual string WeaponKey => null;

    // 플래시용 머티리얼 관리
    private Material _originalMaterial;
    private Material _flashMaterial;

    //감염이 발생했을 때 도트뎀 들어가게 할 용도로 쓰는 코루틴
    private Coroutine _infectionCoroutine;

    /// <summary>
    /// 캐릭터가 현재 생존 상태인지 여부
    /// </summary>
    public bool IsAlive() => _state == Define.CharacterState.Alive;

    /// <summary>
    /// 기본 Awake: Collider 초기화, 스탯/무기 로드 및 체력 설정
    /// </summary>
    protected virtual void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
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
    /// 이동 명령 처리. 지정 위치로 물리 이동.
    /// </summary>
    /// <param name="target">이동할 월드 좌표</param>
    public virtual void MoveTo(Vector2 target)
    {
        if (_characterStat == null || _rigid == null) return;

        Vector2 direction = (target - _rigid.position).normalized;
        Vector2 newPos = _rigid.position + direction * _characterStat.moveSpeed * Time.deltaTime;
        _rigid.MovePosition(newPos);
    }

    /// <summary>
    /// 데미지를 입는다. 방어력, 회피력 적용 포함.
    /// </summary>
    /// <param name="damage">기본 데미지</param>
    /// <param name="attacker">공격자</param>
    public virtual void TakeDamage(float rawDamage, CharacterBase attacker)
    {
        
        if (_state == Define.CharacterState.Dead)
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
            ApplyInfection(attacker._weapon, attacker);
        }

        if (_currentHP <= 0)
            Die();
    }

    /// <summary>
    /// 감염 DOT 효과를 적용한다.
    /// 
    /// </summary>
    public virtual void ApplyInfection(Weapon attackerWeapon, CharacterBase attacker)
    {
        if (_state != Define.CharacterState.Alive) return;

        // 이미 감염 중이면 무시
        if (_state == Define.CharacterState.Infected || _infectionDOT != null)
            return;

        if (Random.value <= attackerWeapon.infectionChance)
        {
            _state = Define.CharacterState.Infected;

        //    _infectionDOT = attackerWeapon.CreateDOT(attacker);
        //    if (_infectionDOT != null)
        //        _infectionCoroutine = CoroutineRunner.Instance.StartCoroutine(_infectionDOT.StartDOT(attacker));
        }
    }


    /// <summary>
    /// 캐릭터가 죽었을 때의 처리: DOT 중지, 충돌 해제, 후속 행동 호출
    /// </summary>
    public virtual void Die()
    {
        if (_state == Define.CharacterState.Dead) return;

        _state = Define.CharacterState.Dead;

        if (_infectionCoroutine != null) // ✅ 핸들로 중지
            CoroutineRunner.Instance.StopCoroutine(_infectionCoroutine);

        if (_col != null)
            _col.enabled = false;
        
        HandleDeathOutcome();
    }


    /// <summary>
    /// 사망 후 개별 처리 (예: 좀비 소환, 파괴 등). 파생 클래스에서 구현.
    /// </summary>
    protected abstract void HandleDeathOutcome();
}
