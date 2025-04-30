using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class HitFlasher
    {
        public static void Flash(SpriteRenderer spriteRenderer, Material flashMaterial, Material originalMaterial, MonoBehaviour owner, float duration = 0.05f)
        {
            if (spriteRenderer == null || flashMaterial == null || originalMaterial == null || owner == null)
                return;

            owner.StartCoroutine(FlashCoroutine(spriteRenderer, flashMaterial, originalMaterial, duration));
        }

        private static IEnumerator FlashCoroutine(SpriteRenderer spriteRenderer, Material flashMaterial, Material originalMaterial, float duration)
        {
            spriteRenderer.material = flashMaterial;
            flashMaterial.SetFloat("_FlashAmount", 1f);

            yield return new WaitForSeconds(duration);

            flashMaterial.SetFloat("_FlashAmount", 0f);
            spriteRenderer.material = originalMaterial;
        }
    }
}
