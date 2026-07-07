using UnityEngine;

public class SpawnJumpscare : MonoBehaviour
{
    [SerializeField] private GameObject toSpawn;

    [SerializeField] private AudioClip Sfx;

    private void OnDestroy()
    {
        AudioManager.Instance.PlaySFX(Sfx, 0.2f);
        toSpawn.SetActive(true);
    }
}
