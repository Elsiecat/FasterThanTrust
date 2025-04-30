using UnityEngine;

/// <summary>
/// 캐릭터가 사용하는 무기 데이터. 감염 확률, 공격력, 사거리 등 전투 속성을 포함한다.
/// Resources/Weapons 경로에서 ScriptableObject 형태로 관리된다.
/// </summary>
[CreateAssetMenu(menuName = "Data/Weapon", fileName = "Weapon_")]
public class Weapon : ScriptableObject
{
    public WeaponType type;

    [Header("전투 관련 수치")]
    public float attackPower;        // 기본 공격력
    public float infectionChance;   // 감염 확률 (0 ~ 1)
    public float attackRange;       // 공격 사거리
    public float attackSpeed;       // 초당 공격 횟수
    public float criticalChance;    // 치명타 확률 (0 ~ 1)

    [Header("탄약/재장전")]
    public int ammoCapacity;        // 탄창 크기 (0이면 무제한)
    public float reloadTime;        // 재장전 시간

    [Header("감염 DOT")]
    public float dotDuration;       // 감염 지속 시간
    public float dotTickInterval;   // 감염 틱 간격
    public float dotDamagePerTick;  // 감염 틱당 데미지

    /// <summary>
    /// 무기 데이터에 따라 감염 DOT 효과를 생성한다.
    /// </summary>
    public InfectionDOT CreateDOT()
    {
        return new InfectionDOT(dotDuration, dotTickInterval, dotDamagePerTick);
    }
}
