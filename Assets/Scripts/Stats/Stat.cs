using UnityEngine;

[CreateAssetMenu(fileName = "NewStat", menuName = "Data/Stat")]
public class Stat : ScriptableObject
{
    public float moveSpeed = 3.5f;
    public float sightRange = 6f;
    public float hp = 100f;
}
