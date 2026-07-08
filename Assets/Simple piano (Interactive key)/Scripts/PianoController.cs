using UnityEngine;

namespace JIYUMA.Piano
{
    public class PianoController : MonoBehaviour
    {
        [Header("Audio Settings")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;

        [Header("Input Settings")]
        [Tooltip("Enable mouse click input.")]
        public bool enableClickInput = true;

        [Tooltip("Enable physics-based input such as VR hands or finger colliders.")]
        public bool enablePhysicsInput = true;
    }
}