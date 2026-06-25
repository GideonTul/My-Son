using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    public MusicTrack track;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(track, 0.17f);
    }
}