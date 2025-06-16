using System.Collections;
using UnityEngine;

/// <summary>
/// 감염 DOT(데미지 오버 타임) 정보를 담고 실행하는 클래스.
/// 감염되면 일정 시간 동안 일정 주기로 피해를 입힌다.
/// </summary>
public class InfectionDOT
{
    public float duration;
    public float tickInterval;
    public float damagePerTick;

    public InfectionDOT(float duration, float tickInterval, float damagePerTick)
    {
        this.duration = duration;
        this.tickInterval = tickInterval;
        this.damagePerTick = damagePerTick;
    }

    /// <summary>
    /// DOT 시작. CoroutineRunner에서 돌려야 함.
    /// </summary>
    public IEnumerator StartDOT(CharacterBase target, CharacterBase attacker)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (target == null || !target.IsAlive()) yield break;

            target.TakeDamage(damagePerTick, attacker);
            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }
    }
}
