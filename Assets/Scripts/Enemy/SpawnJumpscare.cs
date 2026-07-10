using AudioSystem;
using UnityEngine;

public class SpawnJumpscare : MonoBehaviour
{
    [SerializeField] private GameObject toSpawn;

    [SerializeField] private SoundData Sfx;

    private void OnDestroy()
    {
        AudioManager.Instance.Play(Sfx);

        if (toSpawn != null) toSpawn.SetActive(true);
    }
}
