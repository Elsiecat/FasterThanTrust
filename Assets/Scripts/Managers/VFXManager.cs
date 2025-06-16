using System.Collections;
using UnityEngine;

/// <summary>
/// 이펙트를 ObjectPool에서 꺼내 재생하고, 일정 시간 후 반환하는 매니저.
/// MonoBehaviour로 만들어 자체 코루틴 사용.
/// </summary>
public class VFXManager : MonoBehaviour
{
    public void Play(string effectName, Vector3 pos, Quaternion rot, float duration = 2f)
    {
        string path = $"VFX/{effectName}";
        GameObject instance = Managers.Pool.Spawn(path, pos, rot);
        if (instance == null) return;

        StartCoroutine(ReturnAfter(path, instance, duration));
    }

    private IEnumerator ReturnAfter(string path, GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.Pool.Despawn(path, go);
    }
}
