/// <summary>
/// 무기의 공격 타입을 정의하는 열거형.
/// </summary>
public enum WeaponType
{
    None,       // 무기 없음 (시민 등)
    Melee,      // 근접 공격 (좀비 손톱 등)
    Ranged,     // 원거리 무기 (총 등)
    Explosive,  // 폭발형 (수류탄, 폭탄 등)
    Special     // 특수 무기 (차저의 돌진 등 나중 확장 가능)
}
