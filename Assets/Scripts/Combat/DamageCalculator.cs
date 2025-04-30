using UnityEngine;

public static class DamageCalculator
{
    private const float _defenseFactor = 0.85f;

    public static (float finalDamage, bool isCritical) CalculateFinalDamage(float rawDamage, int defense, int level, float critRate)
    {
        bool isCritical = TryCrit(critRate);
        float critMultiplier = isCritical ? 1.5f : 1.0f;

        float defMultiplier = Mathf.Pow(_defenseFactor, defense);
        float levelFactor = 1 + (level - 1) * 0.05f; // 레벨 1 기준 100%

        float finalDamage = rawDamage * critMultiplier * defMultiplier / levelFactor;

        return (finalDamage, isCritical);
    }

    public static bool TryEvade(float evasionRate)
    {
        return Random.value < evasionRate;
    }

    public static bool TryCrit(float critRate)
    {
        return Random.value < critRate;
    }

}

