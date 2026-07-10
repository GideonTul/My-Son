using AudioSystem;
using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private SoundData intro;
    [SerializeField] private SoundData loop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MusicManager.Instance.IntroToLoop(intro, loop);
    }


}
