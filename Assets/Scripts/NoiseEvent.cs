using System.Collections;
using UnityEngine;
using TMPro;

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

    void Start()
    {
        promptUI.SetActive(false);
        StartCoroutine(EventLoop());
    }

    IEnumerator EventLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenChecks, maxTimeBetweenChecks));

            if (eventActive)
                continue;

            if (Random.value <= eventChance)
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

        // Player failed
        Debug.Log("Noise attracted the monster!");

        // TODO:
        // AlertEnemies(transform.position, noiseRadius);

        EndEvent(true);
    }

    void EndEvent(bool failed)
    {
        AudioManager.Instance.StopSFX();

        promptUI.SetActive(false);

        eventActive = false;
    }
}