using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] 
    private AudioMixer mixer;

    [Header("Music Library")]
    [SerializeField]
    private MusicLibrary musicLibrary;

    [Header("Music Sources")]
    [SerializeField] 
    private AudioSource musicA;
    [SerializeField] 
    private AudioSource musicB;

    [Header("SFX Source")]
    [SerializeField] 
    private AudioSource sfxSource;

    [Header("Voice Source")]
    [SerializeField] 
    private AudioSource voiceSource;

    private AudioSource currentMusic;
    private AudioSource nextMusic;

    private Coroutine musicRoutine;

    private MusicTrack currentTrack;

    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string VOICE_KEY = "VoiceVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentMusic = musicA;
        nextMusic = musicB;
    }

    private void Start()
    {
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_KEY, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_KEY, 1f));
        SetVoiceVolume(PlayerPrefs.GetFloat(VOICE_KEY, 1f));
    }

    #region MUSIC

    public void PlayMusic(string cue, float fadeTime = 1.9f, float volume = 1f)
    {
        if (string.IsNullOrEmpty(cue))
            return;

        MusicTrack track = musicLibrary.Get(cue);
        if (track == null)
            return;


        if (currentTrack == track)
            return;

        currentTrack = track;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(PlayMusicRoutine(track, fadeTime, volume));
    }

    private IEnumerator PlayMusicRoutine(MusicTrack track, float fadeTime, float tvolume)
    {
        AudioSource oldSource = currentMusic;
        AudioSource newSource = nextMusic;

        AudioClip introOrLoop = track.introClip != null ? track.introClip : track.loopClip;

        newSource.clip = introOrLoop;
        newSource.loop = track.introClip == null;
        newSource.volume = 0f;
        newSource.Play();

        // crossfade old -> new
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float lerp = t / fadeTime;

            oldSource.volume = Mathf.Lerp(1f, 0f, lerp);
            newSource.volume = Mathf.Lerp(0f, tvolume, lerp);

            yield return null;
        }

        oldSource.Stop();
        oldSource.volume = tvolume;

        currentMusic = newSource;
        nextMusic = oldSource;

        if (track.introClip == null || track.loopClip == null)
        {
            musicRoutine = null;
            yield break;
        }

        // wait for intro to nearly finish, crossfade into loop ---
        const float crossfadeTime = 0.1f;

        while (currentMusic.clip.length - currentMusic.time > crossfadeTime)
            yield return null;

        AudioSource loopSource = nextMusic;
        loopSource.clip = track.loopClip;
        loopSource.loop = true;
        loopSource.volume = 0f;
        loopSource.Play();

        t = 0f;
        while (t < crossfadeTime)
        {
            t += Time.deltaTime;
            float lerp = t / crossfadeTime;

            currentMusic.volume = Mathf.Lerp(1f, 0f, lerp);
            loopSource.volume = Mathf.Lerp(0f, tvolume, lerp);

            yield return null;
        }

        currentMusic.Stop();

        AudioSource old = currentMusic;
        currentMusic = loopSource;
        nextMusic = old;
        currentMusic.volume = tvolume;

        musicRoutine = null;
    }

    #endregion

    #region VOICE

    public void PlayVoice(AudioClip clip)
    {
        if (clip == null)
            return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.Play();
    }

    public void PlayVoice(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        voiceSource.Stop();
        voiceSource.clip = clip;
        voiceSource.volume = volume;
        voiceSource.Play();
    }

    public void StopVoice()
    {
        voiceSource.Stop();
    }

    public bool IsVoicePlaying()
    {
        return voiceSource.isPlaying;
    }

    #endregion

    #region SFX (2D one-shots)

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        sfxSource.PlayOneShot(clip, volume);
    }   

    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public bool IsSFXPlaying()
    {
        return sfxSource.isPlaying;
    }

    #endregion

    #region VOLUME (Mixer only)

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Max(volume, 0.0001f);

        mixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat(MUSIC_KEY, volume);
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Max(volume, 0.0001f);

        mixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat(SFX_KEY, volume);
    }

    public void SetVoiceVolume(float volume)
    {
        volume = Mathf.Max(volume, 0.0001f);

        mixer.SetFloat("VoiceVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat(VOICE_KEY, volume);
    }

    #endregion
}
