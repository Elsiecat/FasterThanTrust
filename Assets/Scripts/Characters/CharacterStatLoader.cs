using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 캐릭터 스탯과 무기를 Resources에서 불러와 캐싱하는 유틸리티.
/// </summary>
public static class CharacterStatLoader
{
    private static Dictionary<string, CharacterStat> _statCache = new();
    private static Dictionary<string, Weapon> _weaponCache = new();

    /// <summary>
    /// 캐릭터 스탯을 불러온다. 경로는 "Stats/{key}"
    /// </summary>
    public static CharacterStat LoadStat(string key)
    {
        if (_statCache.TryGetValue(key, out var stat))
            return stat;

        return LoadFromResources("Stats/" + key, _statCache);
    }

    /// <summary>
    /// 무기 정보를 불러온다. 경로는 "Weapons/{key}"
    /// </summary>
    public static Weapon LoadWeapon(string key)
    {
        if (_weaponCache.TryGetValue(key, out var weapon))
            return weapon;

        return LoadFromResources("Weapons/" + key, _weaponCache);
    }

    /// <summary>
    /// 내부 공통 로직: Resources에서 ScriptableObject를 불러와 캐싱한다.
    /// </summary>
    private static T LoadFromResources<T>(string path, Dictionary<string, T> cache) where T : ScriptableObject
    {
        T loaded = Resources.Load<T>(path);

        if (loaded == null)
        {
            Debug.LogError($"[CharacterStatLoader] '{path}' 리소스를 찾을 수 없습니다.");
            return null;
        }

        T clone = Object.Instantiate(loaded);
        cache[path] = clone;
        return clone;
    }
}
