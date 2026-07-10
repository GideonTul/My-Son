using AudioSystem;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private SoundData chaseMusic;
    [SerializeField] private SoundData normalMusic;

    private void Start()
    {
        StartNormalMusic();
    }

    private void OnEnable()
    {
        GameEvents.OnEnemyStartedChasing += StartChaseMusic;
        GameEvents.OnEnemyStoppedChasing += StartNormalMusic;
        GameEvents.OnPlayerSafeChanged += StartNormalMusic;
    }

    private void OnDisable()
    {
        GameEvents.OnEnemyStartedChasing -= StartChaseMusic;
        GameEvents.OnEnemyStoppedChasing -= StartNormalMusic;
        GameEvents.OnPlayerSafeChanged -= StartNormalMusic;
    }

    private void StartChaseMusic()
    {
        MusicManager.Instance.Play(chaseMusic, 0.1f);
    }

    private void StartNormalMusic()
    {
        MusicManager.Instance.Play(normalMusic, 2f);
    }
    private void StartNormalMusic(bool safe)
    {
        if (safe) MusicManager.Instance.Play(normalMusic, 2f);
    }
}