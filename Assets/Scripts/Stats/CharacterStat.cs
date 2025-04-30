using UnityEngine;

[CreateAssetMenu(menuName = "Stat/CharacterStat")]
public class CharacterStat : ScriptableObject
{
    public float maxHp = 100f;
    public float moveSpeed = 2.5f;
    public int defense = 0;     // 0~10
    public float evasion = 0f;  // 0~1
    public float sightRadius = 5f; // 시야 거리
}
