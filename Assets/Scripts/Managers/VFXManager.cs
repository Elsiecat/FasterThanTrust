using UnityEngine;
using Combat;
using System.Collections;

public class VFXManager : MonoBehaviour
{
    private void OnEnable()
    {
        CombatEventHub.OnHit += HandleHit;
    }

    private void OnDisable()
    {
        CombatEventHub.OnHit -= HandleHit;
    }

    public void Play(string effectName, Vector3 pos, Quaternion rot, float duration = 2f, Transform followTarget = null)
    {
        string path = $"VFX/{effectName}";
        GameObject instance = Managers.Pool.Spawn(path, pos, rot);
        if (instance == null) return;

        // ❗ 따라붙게 만들되, Parent로 붙이지 않고 위치만 따라가게 함
        if (followTarget != null)
            CoroutineRunner.Instance.StartCoroutine(FollowAndReturn(path, instance, followTarget, duration));
        else
            CoroutineRunner.Instance.StartCoroutine(ReturnAfter(path, instance, duration));
    }

        private IEnumerator FollowAndReturn(string path, GameObject go, Transform followTarget, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration && go != null && followTarget != null)
        {
            go.transform.position = followTarget.position;
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (go != null)
            Managers.Pool.Despawn(path, go);
    }

    private IEnumerator ReturnAfter(string path, GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        Managers.Pool.Despawn(path, go);
    }

    private void HandleHit(HitEventArgs args)
    {
        if (args.WeaponUsed == null) return;

        switch (args.WeaponUsed.type)
        {
            case WeaponType.Melee:
                Play("VFX_Hit_Blood", args.HitPosition, Quaternion.identity);
                break;
            case WeaponType.Ranged:
                Play("GunImpact", args.HitPosition, Quaternion.identity);
                break;
            case WeaponType.Explosive:
                Play("BluntHit", args.HitPosition, Quaternion.identity);
                break;
            default:
                Play("DefaultHit", args.HitPosition, Quaternion.identity);
                break;
        }
    }
}
