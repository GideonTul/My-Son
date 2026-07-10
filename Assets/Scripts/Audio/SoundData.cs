using UnityEngine;

namespace AudioSystem
{
    /// <summary>
    /// The set of mixer buses a sound can be routed through.
    /// Add new entries here (and a matching AudioMixerGroup slot on AudioManager)
    /// if your project needs more categories.
    /// </summary>
    public enum SoundCategory
    {
        Music,
        SFX,
        UI,
        Ambience,
        Voice
    }

    /// <summary>
    /// Data-driven definition of a sound. Designers create and tweak these as assets
    /// (Create > Audio > Sound Data) without ever touching code.
    /// </summary>
    [CreateAssetMenu(fileName = "New Sound", menuName = "Audio/Sound Data")]
    public class SoundData : ScriptableObject
    {
        [Header("Clips")]
        [Tooltip("One clip is picked at random each time this sound plays, for variation.")]
        public AudioClip[] clips;

        [Header("Routing")]
        public SoundCategory category = SoundCategory.SFX;

        [Header("Playback")]
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0f, 0.5f)] public float volumeVariance = 0f;
        [Range(0.1f, 3f)] public float pitch = 1f;
        [Range(0f, 0.5f)] public float pitchVariance = 0f;
        public bool loop = false;

        [Header("Spatial (0 = 2D, 1 = fully 3D)")]
        [Range(0f, 1f)] public float spatialBlend = 0f;
        public float minDistance = 1f;
        public float maxDistance = 25f;

        [Header("Throttling")]
        [Tooltip("Minimum seconds between plays, so rapid-fire sounds like footsteps don't stack.")]
        public float cooldown = 0f;

        public AudioClip GetClip()
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }

        public float GetVolume() => Mathf.Clamp01(volume + Random.Range(-volumeVariance, volumeVariance));
        public float GetPitch() => pitch + Random.Range(-pitchVariance, pitchVariance);
    }
}
