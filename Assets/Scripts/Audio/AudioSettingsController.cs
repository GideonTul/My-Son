using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    /// <summary>
    /// Bridges linear 0-1 UI sliders to AudioMixer exposed parameters (decibels) and
    /// persists them via PlayerPrefs. The mixer must expose float parameters named
    /// MasterVolume, MusicVolume, SFXVolume, UIVolume, AmbienceVolume and VoiceVolume
    /// (right-click a group's Volume in the Audio Mixer window > "Expose to script").
    /// </summary>
    public class AudioSettingsController : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;

        private const string MasterKey = "Audio_MasterVolume";
        private const string MusicKey = "Audio_MusicVolume";
        private const string SFXKey = "Audio_SFXVolume";
        private const string UIKey = "Audio_UIVolume";
        private const string AmbienceKey = "Audio_AmbienceVolume";
        private const string VoiceKey = "Audio_VoiceVolume";

        private void Start()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(MasterKey, 1f));
            SetMusicVolume(PlayerPrefs.GetFloat(MusicKey, 1f));
            SetSFXVolume(PlayerPrefs.GetFloat(SFXKey, 1f));
            SetUIVolume(PlayerPrefs.GetFloat(UIKey, 1f));
            SetAmbienceVolume(PlayerPrefs.GetFloat(AmbienceKey, 1f));
            SetVoiceVolume(PlayerPrefs.GetFloat(VoiceKey, 1f));
        }

        public void SetMasterVolume(float linear) => SetVolume("MasterVolume", MasterKey, linear);
        public void SetMusicVolume(float linear) => SetVolume("MusicVolume", MusicKey, linear);
        public void SetSFXVolume(float linear) => SetVolume("SFXVolume", SFXKey, linear);
        public void SetUIVolume(float linear) => SetVolume("UIVolume", UIKey, linear);
        public void SetAmbienceVolume(float linear) => SetVolume("AmbienceVolume", AmbienceKey, linear);
        public void SetVoiceVolume(float linear) => SetVolume("VoiceVolume", VoiceKey, linear);

        public float GetSavedVolume(string prefsKey) => PlayerPrefs.GetFloat(prefsKey, 1f);

        private void SetVolume(string exposedParam, string prefsKey, float linear)
        {
            linear = Mathf.Clamp(linear, 0.0001f, 1f); // avoid log10(0) = -infinity
            float dB = Mathf.Log10(linear) * 20f;
            mixer.SetFloat(exposedParam, dB);
            PlayerPrefs.SetFloat(prefsKey, linear);
        }
    }
}
