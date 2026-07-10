using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    /// <summary>
    /// Central entry point for one-shot and looped SFX/UI/Ambience/Voice playback.
    /// Music is handled separately by MusicManager, since its lifecycle (long-lived,
    /// crossfaded) is different enough from pooled one-shots to deserve its own system.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Mixer Routing")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioMixerGroup sfxGroup;
        [SerializeField] private AudioMixerGroup uiGroup;
        [SerializeField] private AudioMixerGroup ambienceGroup;
        [SerializeField] private AudioMixerGroup voiceGroup;
        [SerializeField] private AudioMixerGroup musicGroup;

        [Header("Pool")]
        [SerializeField] private int prewarmCount = 16;
        [SerializeField] private int maxPoolSize = 32;

        private AudioSourcePool _pool;
        private readonly Dictionary<SoundData, float> _lastPlayedTime = new Dictionary<SoundData, float>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _pool = new AudioSourcePool(transform, prewarmCount, maxPoolSize);
        }

        private AudioMixerGroup GetGroup(SoundCategory category)
        {
            switch (category)
            {
                case SoundCategory.Music: return musicGroup;
                case SoundCategory.SFX: return sfxGroup;
                case SoundCategory.UI: return uiGroup;
                case SoundCategory.Ambience: return ambienceGroup;
                case SoundCategory.Voice: return voiceGroup;
                default: return sfxGroup;
            }
        }

        private bool IsOffCooldown(SoundData data)
        {
            if (data.cooldown <= 0f) return true;
            return !_lastPlayedTime.TryGetValue(data, out var last) || Time.unscaledTime - last >= data.cooldown;
        }

        /// <summary>Play a non-positional sound (UI clicks, 2D stingers, etc).</summary>
        public PooledAudioSource Play(SoundData data) => PlayInternal(data, null, null);

        /// <summary>Play a sound at a fixed world position (impacts, explosions, pickups).</summary>
        public PooledAudioSource PlayAt(SoundData data, Vector3 position) => PlayInternal(data, position, null);

        /// <summary>Play a sound that follows a moving transform (footsteps, engine loops).</summary>
        public PooledAudioSource PlayAttached(SoundData data, Transform target) => PlayInternal(data, null, target);

        private PooledAudioSource PlayInternal(SoundData data, Vector3? position, Transform followTarget)
        {
            if (data == null || data.clips == null || data.clips.Length == 0)
            {
                Debug.LogWarning("AudioManager: SoundData is missing clips.", data);
                return null;
            }

            if (!IsOffCooldown(data)) return null;
            _lastPlayedTime[data] = Time.unscaledTime;

            var pooled = _pool.Get();
            pooled.Play(data, GetGroup(data.category), _pool.Release, followTarget, position);
            return pooled;
        }

        /// <summary>Immediately stop a sound started with Play/PlayAt/PlayAttached.</summary>
        public void Stop(PooledAudioSource handle) => handle?.Stop();

        /// <summary>Fade out and stop a sound started with Play/PlayAt/PlayAttached.</summary>
        public void FadeOutAndStop(PooledAudioSource handle, float duration) => handle?.FadeOutAndStop(duration);
    }
}
