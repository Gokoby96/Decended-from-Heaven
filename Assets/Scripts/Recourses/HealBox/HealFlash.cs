using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealFlash : MonoBehaviour
{
    public Image flashImage; // Canvas üzerindeki beyaz Image
    public float flashDuration = 0.2f; // Ekranın kaç saniye beyaz kalacağı
    public float maxAlpha = 0.5f; // Beyazın yoğunluğu

    public void Flash()
    {
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        // Başlangıç alpha
        flashImage.color = new Color(1f, 1f, 1f, maxAlpha);

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0, elapsed / flashDuration);
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        flashImage.color = new Color(1f, 1f, 1f, 0f);
    }
}