using System;

namespace Combat
{
    public static class CombatEventHub
    {
        public static event Action<HitEventArgs> OnHit;

        public static void RaiseHit(HitEventArgs args)
        {
            OnHit?.Invoke(args);
        }
    }
}
