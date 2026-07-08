using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SequenceButton : MonoBehaviour, IInteractable, IPuzzleButton
{
    [Header("Identity")]
    [SerializeField] private int buttonId;
    [SerializeField] private NoteName noteName;

    [Header("Audio")]
    [SerializeField] private AudioClip note;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private float flashDuration = 0.4f;

    private AudioSource audioSource;
    private Color baseColor;
    private bool inputEnabled = false;

    public int ButtonId => buttonId;
    public NoteName Note => noteName;


    public event Action<int> OnButtonPressed;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (targetRenderer != null)
            baseColor = targetRenderer.material.color;
    }


    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    public void Interact()
    {
        if (!inputEnabled) return;

        Activate();
        OnButtonPressed?.Invoke(buttonId);
    }
    public void Activate()
    {
        if (note != null)
            audioSource.PlayOneShot(note);

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        if (targetRenderer != null)
            targetRenderer.material.color = highlightColor;

        yield return new WaitForSeconds(flashDuration);

        if (targetRenderer != null)
            targetRenderer.material.color = baseColor;
    }
}