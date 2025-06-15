using System;
using UnityEngine;

namespace Combat
{
    /// <summary>
    /// 전투 중 발생하는 이벤트를 델리게이트로 관리하는 허브 클래스.
    /// </summary>
    public static class CombatEventHub
    {
        public static event Action<HitEventArgs> OnHit;

        public static void RaiseHit(HitEventArgs args)
        {
            OnHit?.Invoke(args);
        }
    }
}