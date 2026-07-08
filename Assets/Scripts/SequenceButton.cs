using System;
using System.Collections;
using UnityEngine;

// Assumes you already have:
// public interface IInteractable { void Interact(); }

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

    // The puzzle controller subscribes to this instead of buttons
    // needing any reference back to the controller.
    public event Action<int> OnButtonPressed;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (targetRenderer != null)
            baseColor = targetRenderer.material.color;
    }

    /// <summary>
    /// Called by the puzzle controller to lock/unlock player input
    /// (e.g. disabled while the sequence is being demonstrated).
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        inputEnabled = enabled;
    }

    /// <summary>
    /// Your existing interaction entry point (e.g. from a raycast/interact system).
    /// </summary>
    public void Interact()
    {
        if (!inputEnabled) return;

        Activate();
        OnButtonPressed?.Invoke(buttonId);
    }

    /// <summary>
    /// Plays the note + flash. Called both by player input and by
    /// the controller when demonstrating the sequence.
    /// </summary>
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