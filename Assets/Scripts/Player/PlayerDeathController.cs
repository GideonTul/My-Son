using StarterAssets;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerDeathController : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    private FirstPersonController playerController;
    private NoiseEvent playerNoiseEvent;

    private PlayableDirector activeCutscene;

    private void Awake()
    {
        playerController = player.GetComponent<FirstPersonController>();
        playerNoiseEvent = player.GetComponent<NoiseEvent>();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerKilled += HandlePlayerKilled;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerKilled -= HandlePlayerKilled;

        if (activeCutscene != null)
            activeCutscene.stopped -= HandleCutsceneStopped;
    }

    private void HandlePlayerKilled(PlayableDirector cutscene)
    {
        if (cutscene == null)
        {
            Debug.LogWarning("[PlayerDeathController] OnPlayerKilled fired with no cutscene assigned.");
            return;
        }

        playerController.enabled = false;
        playerNoiseEvent.enabled = false;
        
        activeCutscene = cutscene;
        activeCutscene.stopped += HandleCutsceneStopped;

        Debug.Log($"TimeScale: {Time.timeScale}");
        activeCutscene.Play();

        GameManager.Instance.RespawnPlayer(player);
    }

    private void HandleCutsceneStopped(PlayableDirector director)
    {
        director.stopped -= HandleCutsceneStopped;
        activeCutscene = null;

        playerController.enabled = true;
        playerNoiseEvent.enabled = true;

        GameEvents.OnPlayerRespawned?.Invoke();
    }
}