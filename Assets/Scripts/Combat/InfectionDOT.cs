using System.Collections;
using UnityEngine;

/// <summary>
/// 감염 DOT(데미지 오버 타임) 정보를 담고 실행하는 클래스.
/// 감염되면 일정 시간 동안 일정 주기로 피해를 입힌다.
/// </summary>
public class InfectionDOT
{
    public float duration;        // DOT 지속 시간
    public float tickInterval;   // 피해 주기 (초당 n회)
    public float damagePerTick;  // 한 번에 입히는 피해량

    /// <summary>
    /// DOT 인스턴스를 생성한다.
    /// </summary>
    /// <param name="duration">전체 지속 시간</param>
    /// <param name="tickInterval">틱 간격</param>
    /// <param name="damagePerTick">틱당 피해량</param>
    public InfectionDOT(float duration, float tickInterval, float damagePerTick)
    {
        this.duration = duration;
        this.tickInterval = tickInterval;
        this.damagePerTick = damagePerTick;
    }

    /// <summary>
    /// DOT를 시작하여 일정 주기로 피해를 입힌다.
    /// </summary>
    /// <param name="target">피해를 입을 대상</param>
    public IEnumerator StartDOT(CharacterBase target)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            target.TakeDamage(damagePerTick, null);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
