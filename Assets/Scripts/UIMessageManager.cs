using System.Collections;
using TMPro;
using UnityEngine;

public class UIMessageManager : MonoBehaviour
{
    public static UIMessageManager Instance;

    [SerializeField] private TMP_Text messageText;

    [SerializeField] private CanvasGroup canv;

    private Coroutine messageRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowMessage(string message, float duration = 5f)
    {
        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(DisplayMessage(message, duration));
    }

    private IEnumerator DisplayMessage(string message, float duration)
    {
        messageText.text = message;

        yield return StartCoroutine(ScreenFader.Instance.FadeToBlack(canv, 0.3f));
        

        yield return new WaitForSeconds(duration);


        yield return StartCoroutine(ScreenFader.Instance.FadeFromBlack(canv, 0.3f));
    }
}
