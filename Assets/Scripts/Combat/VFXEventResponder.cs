using UnityEngine;
using Combat;

/// <summary>
/// 피격 이벤트에 반응해 무기에 따라 이펙트를 재생하는 리스너.
/// </summary>
public class VFXEventResponder : MonoBehaviour
{
    private void OnEnable()
    {
        CombatEventHub.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        CombatEventHub.OnHit -= HandleHit;
    }

    private void HandleHit(HitEventArgs args)
    {
        if (args.WeaponUsed == null) return;

        switch (args.WeaponUsed.type)

        //    None,       // 무기 없음 (시민 등)
        // Melee,      // 근접 공격 (좀비 손톱 등)
        // Ranged,     // 원거리 무기 (총 등)
        // Explosive,  // 폭발형 (수류탄, 폭탄 등)
        // Special 
        {
            case WeaponType.Melee:
                Managers.vFXManager.Play("BloodSplash", args.HitPosition, Quaternion.identity);
                break;
            case WeaponType.Ranged:
                Managers.vFXManager.Play("GunImpact", args.HitPosition, Quaternion.identity);
                break;
            case WeaponType.Explosive:
                Managers.vFXManager.Play("BluntHit", args.HitPosition, Quaternion.identity);
                break;
            default:
                Managers.vFXManager.Play("DefaultHit", args.HitPosition, Quaternion.identity);
                break;
        }
    }
}