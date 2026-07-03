using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public MusicTrack track;
    public float volume = 1f;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(track, 0.17f, volume);
    }
}