using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NoiseEvent : MonoBehaviour
{
    [Header("Timing")]
    public float minTimeBetweenChecks = 20f;
    public float maxTimeBetweenChecks = 40f;

    [Range(0f, 1f)]
    public float eventChance = 0.25f;

    [Header("Prompt")]
    public KeyCode stopKey = KeyCode.F;
    public float reactionTime = 3f;

    [Header("Audio")]
    public AudioClip noiseClip;

    [Header("UI")]
    public GameObject promptUI;
    public TMP_Text promptText;

    [Header("Gameplay")]
    public float noiseRadius = 15f;

    private bool eventActive;
    private Coroutine eventLoop;
    private EnemyAI enemy;

    void Start()
    {
        enemy = FindAnyObjectByType<EnemyAI>();
    }

    void OnEnable()
    {
        if (promptUI != null)
            promptUI.SetActive(false);

        eventLoop = StartCoroutine(EventLoop());
    }

    void OnDisable()
    {
        if (eventLoop != null)
        {
            StopCoroutine(eventLoop);
            eventLoop = null;
        }

        eventActive = false;

        if (promptUI != null)
            promptUI.SetActive(false);
    }



    IEnumerator EventLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenChecks, maxTimeBetweenChecks));

            if (eventActive)
                continue;

            if (Random.value <= eventChance && !EnemyAI.GetPlayerSafetyStatus())
                StartCoroutine(StartNoiseEvent());
        }
    }

    IEnumerator StartNoiseEvent()
    {
        eventActive = true;

        AudioManager.Instance.PlaySFX(noiseClip, 0.2f);

        promptUI.SetActive(true);
        promptText.text = $"Press {stopKey} to silence";

        float timer = reactionTime;

        while (timer > 0)
        {
            if (Input.GetKeyDown(stopKey))
            {
                EndEvent(false);
                yield break;
            }

            timer -= Time.deltaTime;
            yield return null;
        }


        Debug.Log("Noise attracted the monster!");
        enemy.HearNoise(transform.position, noiseRadius);

        EndEvent(true);
    }

    void EndEvent(bool failed)
    {
        if (failed == false) AudioManager.Instance.StopSFX();

        promptUI.SetActive(false);

        eventActive = false;
    }
}