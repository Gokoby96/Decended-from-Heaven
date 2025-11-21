using UnityEngine;
using TMPro;

public class AmmoTextEffect : MonoBehaviour
{
    public float punchScale = 1.2f;  
    public float duration = 0.15f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlayEffect()
    {
        StopAllCoroutines();
        StartCoroutine(ScaleEffect());
    }

    private System.Collections.IEnumerator ScaleEffect()
    {
        // büyüt
        transform.localScale = originalScale * punchScale;

        // geri küçül
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                originalScale,
                t / duration
            );
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
