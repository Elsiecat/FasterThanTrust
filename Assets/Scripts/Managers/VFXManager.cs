using UnityEngine;

public class VFXManager
{
    public void Play(string effectName, Vector3 pos, Quaternion rot)
    {
        var prefab = Resources.Load<GameObject>($"Effects/{effectName}");
        if (prefab == null) return;

        GameObject instance = GameObject.Instantiate(prefab, pos, rot);
        GameObject.Destroy(instance, 3f); // 재생 후 자동 삭제
    }
}