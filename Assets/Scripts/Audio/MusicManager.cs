using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    /// <summary>
    /// Handles background music as a separate concern from pooled SFX: a small rotating
    /// pool of long-lived AudioSources that crossfade between tracks, a ducking helper
    /// for lowering music under important SFX or dialogue, and a sample-accurate
    /// intro-then-loop mode for tracks with a one-shot intro followed by a looping body.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private float defaultCrossfadeDuration = 1.5f;

        // 3 sources, not 2: a plain crossfade only ever needs an "outgoing" and an
        // "incoming" source, but IntroToLoop briefly needs a third — the outgoing
        // source is still fading out at the same time the intro is fading in AND the
        // loop clip is pre-scheduled and waiting silently to take over. Reusing the
        // outgoing source for the scheduled loop would cause its own fade-out to
        // Stop() (and cancel the schedule on) the very source waiting to play next.
        private const int SourceCount = 3;

        // Small buffer before a PlayScheduled call so it reliably lands in the future
        // relative to the audio thread, rather than racing the current audio buffer.
        private const double ScheduleLeadTime = 0.1;

        private AudioSource[] _sources;
        private AudioSource _active;
        private Coroutine _routine;

        // Tracks an intro-to-loop handoff in progress, so a later Play()/Stop()/
        // IntroToLoop() call can cleanly cancel it instead of leaving a stray
        // scheduled loop source waiting to kick in later.
        private Coroutine _introLoopRoutine;
        private AudioSource _pendingLoopSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _sources = new AudioSource[SourceCount];
            for (int i = 0; i < SourceCount; i++) _sources[i] = CreateSource();
            _active = _sources[0];
        }

        private AudioSource CreateSource()
        {
            var go = new GameObject("MusicSource");
            go.transform.SetParent(transform, false);
            var src = go.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = musicGroup;
            src.loop = true;
            src.playOnAwake = false;
            src.spatialBlend = 0f;
            src.volume = 0f;
            return src;
        }

        /// <summary>Returns a pooled source that isn't any of the given (currently reserved) ones.</summary>
        private AudioSource GetFreeSource(AudioSource exclude1, AudioSource exclude2 = null)
        {
            foreach (var s in _sources)
            {
                if (s != exclude1 && s != exclude2) return s;
            }
            return _sources[0]; // unreachable with SourceCount = 3 and at most 2 exclusions
        }

        /// <summary>Crossfade into a new music track. Calling Play with the already-playing track is a no-op.</summary>
        public void Play(SoundData track, float crossfadeDuration = -1f)
        {
            if (track == null) return;
            var clip = track.GetClip();
            if (clip == null) return;
            if (_active.clip == clip && _active.isPlaying) return;
            if (crossfadeDuration < 0f) crossfadeDuration = defaultCrossfadeDuration;

            CancelPendingIntroLoop();

            var incoming = GetFreeSource(_active);
            incoming.clip = clip;
            incoming.loop = true;
            incoming.pitch = track.pitch;
            incoming.volume = 0f;
            incoming.Play();

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Crossfade(_active, incoming, track.volume, crossfadeDuration));
            _active = incoming;
        }

        /// <summary>
        /// Play a track starting from silence and fade it up to its volume over
        /// `fadeDuration` seconds. This is Play() under a clearer name: Play already
        /// ramps the incoming track up from 0, which for a source that wasn't
        /// previously playing anything is exactly a fade-in. Use this when you want
        /// that intent to read clearly at the call site (e.g. music kicking in at a
        /// menu or level start) rather than implying a crossfade away from something.
        /// </summary>
        public void PlayWithFadeIn(SoundData track, float fadeDuration)
        {
            Play(track, fadeDuration);
        }

        /// <summary>Fade the current track out to silence.</summary>
        public void Stop(float fadeDuration = -1f)
        {
            if (fadeDuration < 0f) fadeDuration = defaultCrossfadeDuration;
            CancelPendingIntroLoop();
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(Fade(_active, 0f, fadeDuration, stopOnComplete: true));
        }

        /// <summary>Temporarily lower music volume (e.g. during dialogue or a big SFX moment), then restore it.</summary>
        public void Duck(float volumeMultiplier, float fadeDuration, float holdDuration)
        {
            StartCoroutine(DuckRoutine(volumeMultiplier, fadeDuration, holdDuration));
        }

        /// <summary>
        /// Plays `intro` once, then hands off to `loop` (which then loops forever) at
        /// the exact sample the intro ends — no gap, no overlap. Use for tracks with a
        /// distinct one-shot intro leading into a looping body. Both clips are queued
        /// with AudioSource.PlayScheduled against the audio DSP clock, so the handoff
        /// is sample-accurate regardless of frame rate.
        ///
        /// The loop picks up at `intro`'s volume rather than its own, so the seam is
        /// inaudible; give both SoundData assets the same volume to avoid a jump.
        /// `fadeDuration` only fades the intro in (against whatever was playing
        /// before, if anything) — the intro-to-loop handoff itself is never faded,
        /// since it's meant to sound like one continuous piece of music.
        /// </summary>
        public void IntroToLoop(SoundData intro, SoundData loop, float fadeDuration = -1f)
        {
            if (intro == null || loop == null) return;
            var introClip = intro.GetClip();
            var loopClip = loop.GetClip();
            if (introClip == null || loopClip == null) return;
            if (fadeDuration < 0f) fadeDuration = defaultCrossfadeDuration;

            CancelPendingIntroLoop();
            if (_routine != null) { StopCoroutine(_routine); _routine = null; }

            double introDuration = introClip.samples / (double)introClip.frequency
                                    / Mathf.Max(0.01f, Mathf.Abs(intro.pitch));
            fadeDuration = Mathf.Min(fadeDuration, (float)introDuration);

            var outgoing = _active;
            var introSource = GetFreeSource(outgoing);
            var loopSource = GetFreeSource(outgoing, introSource);

            double startDsp = AudioSettings.dspTime + ScheduleLeadTime;

            introSource.clip = introClip;
            introSource.loop = false;
            introSource.pitch = intro.pitch;
            introSource.volume = 0f;
            introSource.PlayScheduled(startDsp);

            loopSource.Stop(); // clear anything stale (and any leftover scheduling) on this source
            loopSource.clip = loopClip;
            loopSource.loop = true;
            loopSource.pitch = loop.pitch;
            loopSource.volume = intro.volume;
            loopSource.PlayScheduled(startDsp + introDuration);

            _pendingLoopSource = loopSource;
            _active = introSource;

            _introLoopRoutine = StartCoroutine(
                IntroToLoopSequence(outgoing, introSource, loopSource, intro.volume, fadeDuration, introDuration));
        }

        private IEnumerator IntroToLoopSequence(AudioSource outgoing, AudioSource introSource, AudioSource loopSource,
            float targetVolume, float fadeDuration, double introDuration)
        {
            yield return new WaitForSecondsRealtime((float)ScheduleLeadTime);
            yield return Crossfade(outgoing, introSource, targetVolume, fadeDuration);

            float remaining = (float)introDuration - fadeDuration;
            if (remaining > 0f) yield return new WaitForSecondsRealtime(remaining);

            _active = loopSource;
            _pendingLoopSource = null;
            _introLoopRoutine = null;
        }

        /// <summary>Cancels an in-progress IntroToLoop handoff, if any, so it doesn't fire later unexpectedly.</summary>
        private void CancelPendingIntroLoop()
        {
            if (_introLoopRoutine != null)
            {
                StopCoroutine(_introLoopRoutine);
                _introLoopRoutine = null;
            }
            if (_pendingLoopSource != null)
            {
                _pendingLoopSource.Stop();
                _pendingLoopSource = null;
            }
        }

        private IEnumerator DuckRoutine(float multiplier, float fadeDuration, float holdDuration)
        {
            float original = _active.volume;
            yield return Fade(_active, original * multiplier, fadeDuration, false);
            yield return new WaitForSecondsRealtime(holdDuration);
            yield return Fade(_active, original, fadeDuration, false);
        }

        private IEnumerator Crossfade(AudioSource from, AudioSource to, float targetVolume, float duration)
        {
            float fromStart = from.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float p = duration <= 0f ? 1f : t / duration;
                from.volume = Mathf.Lerp(fromStart, 0f, p);
                to.volume = Mathf.Lerp(0f, targetVolume, p);
                yield return null;
            }
            from.volume = 0f;
            from.Stop();
            to.volume = targetVolume;
        }

        private IEnumerator Fade(AudioSource src, float targetVolume, float duration, bool stopOnComplete)
        {
            float start = src.volume;
            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                src.volume = Mathf.Lerp(start, targetVolume, t / duration);
                yield return null;
            }
            src.volume = targetVolume;
            if (stopOnComplete) src.Stop();
        }
    }
}