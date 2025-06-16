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

    public void Play(string effectName, Vector3 pos, Quaternion rot, float duration = 2f)
    {
        string path = $"VFX/{effectName}";
        GameObject instance = Managers.Pool.Spawn(path, pos, rot);
        instance.transform.position = new Vector3(pos.x, pos.y, -5f);
        if (instance == null) return;
        // 🔽 여기서 ParticleSystemRenderer의 레이어 순서 조정
            var psRenderer = instance.GetComponent<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                psRenderer.sortingLayerName = "VFX_OnCharacter(forSorting)";  // 이 Sorting Layer는 Unity에서 직접 만들어둬야 함
                psRenderer.sortingOrder = 30;            // Sprite보다 위로 오게
            }


        StartCoroutine(ReturnAfter(path, instance, duration));
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
