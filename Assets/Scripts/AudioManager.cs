using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] 
    private AudioMixer mixer;

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

    //public void PlayMusic(MusicTrack track, float fadeTime = 1.5f)
    //{
    //    Debug.Log("FORCED MUSIC SWITCH");

    //    currentMusic.Stop();
    //    currentMusic.clip = track.loopClip;
    //    currentMusic.loop = true;
    //    currentMusic.Play();
    //}

    public void PlayMusic(MusicTrack track, float fadeTime = 1.9f)
    {
        Debug.Log("PlayMusic called: " + track);

        if (track == null) return;
        if (currentTrack != null) {
            if (currentTrack.introClip == track.introClip)
                return;
        }
            

        currentTrack = track;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(CrossfadeMusic(track, fadeTime));
    }

    private IEnumerator CrossfadeMusic(MusicTrack track, float fadeTime)
    {

        AudioSource oldSource = currentMusic;
        AudioSource newSource = nextMusic;

        AudioClip clip =
            track.introClip != null ? track.introClip : track.loopClip;
        Debug.Log("Playing clip: " + clip);
        newSource.clip = clip;
        newSource.loop = false;
        newSource.volume = 0f;
        newSource.Play();

        Debug.Log("New source playing: " + newSource.isPlaying);
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            float lerp = t / fadeTime;

            oldSource.volume = Mathf.Lerp(1f, 0f, lerp);
            newSource.volume = Mathf.Lerp(0f, 1f, lerp);

            yield return null;
        }

        oldSource.Stop();
        oldSource.volume = 1f;

        currentMusic = newSource;
        nextMusic = oldSource;

        if (track.introClip != null && track.loopClip != null)
        {
            StartCoroutine(WaitForIntro(track));
        }
    }

    private IEnumerator WaitForIntro(MusicTrack track)
    {
        float crossfadeTime = 0.1f;

        while (currentMusic.clip.length - currentMusic.time > crossfadeTime)
            yield return null;

        if (currentTrack != track)
            yield break;


        AudioSource loopSource = nextMusic;

        loopSource.clip = track.loopClip;
        loopSource.loop = true;
        loopSource.volume = 0f;
        loopSource.Play();


        float t = 0f;

        while (t < crossfadeTime)
        {
            t += Time.deltaTime;

            float lerp = t / crossfadeTime;

            currentMusic.volume = Mathf.Lerp(1f, 0f, lerp);
            loopSource.volume = Mathf.Lerp(0f, 1f, lerp);

            yield return null;
        }


        currentMusic.Stop();

        AudioSource old = currentMusic;
        currentMusic = loopSource;
        nextMusic = old;

        currentMusic.volume = 1f;
    }

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

    #region SFX

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