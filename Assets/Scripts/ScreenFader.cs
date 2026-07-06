using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 2f;

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeToBlack()
    {
        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadeSpeed;
            canvasGroup.alpha = t;
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeFromBlack()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.unscaledDeltaTime * fadeSpeed;
            canvasGroup.alpha = t;
            yield return null;
        }

        canvasGroup.alpha = 0f;
    }
}