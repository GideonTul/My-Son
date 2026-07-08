using UnityEngine;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

namespace JIYUMA.Piano
{
    public class PianoKeyByName_Audio : MonoBehaviour
    {
        [Header("Controller")]
        public PianoController controller;

        [Header("Object Name Settings")]
        public string whitePrefix = "key_white";
        public string blackPrefix = "key_black";

        [Header("Index Offset (use -1 if names start from .001)")]
        public int whiteIndexOffset = -1;
        public int blackIndexOffset = -1;

        [Header("Direction")]
        public bool reverseWhiteOrder = false;
        public bool reverseBlackOrder = false;

        [Header("Fade Settings")]
        public float fadeDelay = 0.05f;
        public float fadeTime = 0.12f;

        [Header("Physics")]
        public string triggerTag = "Finger";

        private AudioSource audioSource;
        private AudioClip[] clips;
        private Coroutine fadeCoroutine;
        private bool isPressedByPhysics = false;

        private static readonly string[] WhiteNotes = new string[]
        {
            "A0","B0",
            "C1","D1","E1","F1","G1","A1","B1",
            "C2","D2","E2","F2","G2","A2","B2",
            "C3","D3","E3","F3","G3","A3","B3",
            "C4","D4","E4","F4","G4","A4","B4",
            "C5","D5","E5","F5","G5","A5","B5",
            "C6","D6","E6","F6","G6","A6","B6",
            "C7","D7","E7","F7","G7","A7","B7",
            "C8"
        };

        private static readonly string[] BlackNotes = new string[]
        {
            "A#0",
            "C#1","D#1","F#1","G#1","A#1",
            "C#2","D#2","F#2","G#2","A#2",
            "C#3","D#3","F#3","G#3","A#3",
            "C#4","D#4","F#4","G#4","A#4",
            "C#5","D#5","F#5","G#5","A#5",
            "C#6","D#6","F#6","G#6","A#6",
            "C#7","D#7","F#7","G#7","A#7"
        };

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = false;
            clips = Resources.LoadAll<AudioClip>("Piano");
        }

        void OnMouseDown()
        {
            if (!CanUseClick()) return;
            PlayNote();
        }

        void OnMouseUp()
        {
            if (!CanUseClick()) return;
            StartFadeOut();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!CanUseTrigger()) return;
            if (!other.CompareTag(triggerTag)) return;
            if (isPressedByPhysics) return;

            isPressedByPhysics = true;
            PlayNote();
        }

        void OnTriggerExit(Collider other)
        {
            if (!CanUseTrigger()) return;
            if (!other.CompareTag(triggerTag)) return;

            isPressedByPhysics = false;
            StartFadeOut();
        }

        bool CanUseClick()
        {
            return controller == null ? true : controller.enableClickInput;
        }

        bool CanUseTrigger()
        {
            return controller == null ? true : controller.enablePhysicsInput;
        }

        void PlayNote()
        {
            string noteName = GetNoteNameFromObjectName(gameObject.name);

            var clip = clips.FirstOrDefault(c => c.name == noteName || c.name.Contains(noteName));

            if (clip == null)
            {
                Debug.LogWarning($"Audio clip not found: {noteName}");
                return;
            }

            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.volume = controller != null ? controller.masterVolume : 1f;
            audioSource.pitch = 1f;
            audioSource.Play();
        }
        public void PressExternally()
        {
            PlayNote();
        }

        public void ReleaseExternally()
        {
            StartFadeOut();
        }
        void StartFadeOut()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            yield return new WaitForSeconds(fadeDelay);

            float startVolume = audioSource.volume;
            float t = 0f;

            while (t < fadeTime)
            {
                t += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeTime);
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = controller != null ? controller.masterVolume : 1f;
        }

        string GetNoteNameFromObjectName(string objectName)
        {
            string lowerName = objectName.ToLower();

            if (lowerName.StartsWith(whitePrefix))
            {
                int index = ExtractBlenderStyleIndex(objectName) + whiteIndexOffset;
                if (reverseWhiteOrder)
                    index = WhiteNotes.Length - 1 - index;

                return WhiteNotes[index];
            }

            if (lowerName.StartsWith(blackPrefix))
            {
                int index = ExtractBlenderStyleIndex(objectName) + blackIndexOffset;
                if (reverseBlackOrder)
                    index = BlackNotes.Length - 1 - index;

                return BlackNotes[index];
            }

            return null;
        }

        int ExtractBlenderStyleIndex(string objectName)
        {
            Match match = Regex.Match(objectName, @"\.(\d+)$");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }
    }
}