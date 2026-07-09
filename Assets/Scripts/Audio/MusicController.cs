using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Header("Cue List")]
    [AudioCue]
    [SerializeField] private string chaseMusic;
    [AudioCue]
    [SerializeField] private string normalMusic;

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
        AudioManager.Instance.PlayMusic(chaseMusic, 0.1f, 0.4f);
    }

    private void StartNormalMusic()
    {
        AudioManager.Instance.PlayMusic(normalMusic, 2f, 0.2f);
    }
    private void StartNormalMusic(bool safe)
    {
        if (safe) AudioManager.Instance.PlayMusic(normalMusic, 2f, 0.2f);
    }
}