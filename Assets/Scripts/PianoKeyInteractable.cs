using System;
using UnityEngine;

namespace JIYUMA.Piano
{
    // Put this on the same GameObject as PianoKeyByName_Audio (and PianoKeyPress,
    // if you want the visual dip/tilt too). It adapts the piano asset to your
    // IInteractable input system and to IPuzzleButton, so PianoController-driven
    // keys can be used as puzzle steps exactly like SequenceButton.
    [RequireComponent(typeof(PianoKeyByName_Audio))]
    public class PianoKeyInteractable : MonoBehaviour, IInteractable, IPuzzleButton
    {
        [Header("Identity")]
        [SerializeField] private int buttonId;

        [Tooltip("Which note this key represents for puzzle-matching purposes. " +
                 "This is independent of the piano asset's own note lookup — " +
                 "set it to whatever letter this key actually plays.")]
        [SerializeField] private NoteName noteName;

        [Header("Piano References")]
        [SerializeField] private PianoKeyByName_Audio keyAudio;
        [Tooltip("Optional — only needed if you want the key to visually dip/tilt.")]
        [SerializeField] private PianoKeyPress keyPress;

        [Tooltip("How long the key stays 'pressed' before auto-releasing, " +
                 "for both player input and sequence playback.")]
        [SerializeField] private float releaseDelay = 0.2f;

        private bool inputEnabled = false;

        public int ButtonId => buttonId;
        public NoteName Note => noteName;
        public event Action<int> OnButtonPressed;

        private void Awake()
        {
            if (keyAudio == null) keyAudio = GetComponent<PianoKeyByName_Audio>();
            if (keyPress == null) keyPress = GetComponent<PianoKeyPress>();
        }

        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
        }

        public void Interact()
        {
            Debug.Log($"{name} Interact");

            if (!inputEnabled)
                return;

            Activate();

            Debug.Log("Invoking event");

            OnButtonPressed?.Invoke(buttonId);
        }

        public void Activate()
        {
            keyAudio.PressExternally();
            keyPress?.PressExternally();

            CancelInvoke(nameof(Release));
            Invoke(nameof(Release), releaseDelay);
        }

        private void Release()
        {
            keyAudio.ReleaseExternally();
            keyPress?.ReleaseExternally();
        }
    }
}
