using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    /// <summary>
    /// Wraps a single AudioSource so it can live in a pool. Handles configuring itself
    /// from a SoundData, optionally following a moving emitter, fading out, and
    /// returning itself to the pool automatically when a one-shot finishes.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PooledAudioSource : MonoBehaviour
    {
        public AudioSource Source { get; private set; }
        public float StartTime { get; private set; }

        private Transform _followTarget;
        private Action<PooledAudioSource> _releaseCallback;
        private Coroutine _watchRoutine;
        private Coroutine _fadeRoutine;

        private void Awake()
        {
            Source = GetComponent<AudioSource>();
            Source.playOnAwake = false;
        }

        public void Play(SoundData data, AudioMixerGroup mixerGroup, Action<PooledAudioSource> releaseCallback,
            Transform followTarget = null, Vector3? worldPosition = null)
        {
            var clip = data.GetClip();
            if (clip == null)
            {
                releaseCallback?.Invoke(this);
                return;
            }

            _releaseCallback = releaseCallback;
            _followTarget = followTarget;

            if (worldPosition.HasValue) transform.position = worldPosition.Value;
            else if (followTarget != null) transform.position = followTarget.position;

            Source.clip = clip;
            Source.outputAudioMixerGroup = mixerGroup;
            Source.volume = data.GetVolume();
            Source.pitch = data.GetPitch();
            Source.loop = data.loop;
            Source.spatialBlend = data.spatialBlend;
            Source.minDistance = data.minDistance;
            Source.maxDistance = data.maxDistance;

            StartTime = Time.unscaledTime;
            Source.Play();

            if (_watchRoutine != null) StopCoroutine(_watchRoutine);
            _watchRoutine = data.loop
                ? null
                : StartCoroutine(ReleaseWhenFinished(clip.length / Mathf.Max(0.01f, Mathf.Abs(Source.pitch))));
        }

        private void LateUpdate()
        {
            if (_followTarget != null)
            {
                transform.position = _followTarget.position;
            }
        }

        private IEnumerator ReleaseWhenFinished(float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            Stop();
        }

        /// <summary>Stops playback immediately and returns this source to the pool.</summary>
        public void Stop()
        {
            if (_watchRoutine != null) { StopCoroutine(_watchRoutine); _watchRoutine = null; }
            if (_fadeRoutine != null) { StopCoroutine(_fadeRoutine); _fadeRoutine = null; }

            Source.Stop();
            Source.clip = null;
            _followTarget = null;

            var cb = _releaseCallback;
            _releaseCallback = null;
            cb?.Invoke(this);
        }

        /// <summary>Fades volume to zero over `duration` seconds, then stops and returns to the pool.</summary>
        public void FadeOutAndStop(float duration)
        {
            if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FadeOutRoutine(duration));
        }

        private IEnumerator FadeOutRoutine(float duration)
        {
            float startVol = Source.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                Source.volume = Mathf.Lerp(startVol, 0f, t / duration);
                yield return null;
            }
            Stop();
        }
    }
}
