using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [AudioCue]
    [SerializeField]
    private string cue;

    [SerializeField]
    private float volume = 1f;

    [SerializeField]
    private float fadeTime = 0.17f;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(cue, fadeTime, volume);
    }
}
