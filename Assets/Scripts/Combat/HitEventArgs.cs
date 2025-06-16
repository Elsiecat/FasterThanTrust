using UnityEngine;
using Combat;

/// <summary>
/// 피격 이벤트 정보. 위치, 피해량, 사용된 무기를 포함.
/// </summary>
namespace Combat
{
    public class HitEventArgs
    {
        public Vector3 HitPosition;
        public Weapon WeaponUsed;
    }
}