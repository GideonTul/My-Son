using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Vector3 CurrentCheckpoint;

    private void Awake()
    {
        Instance = this;
    }

    public void SetCheckpoint(Vector3 position)
    {
        CurrentCheckpoint = position + Vector3.down;
    }

    public void RespawnPlayer(GameObject player)
    {
        CharacterController cc = player.GetComponent<CharacterController>();

        if (cc != null)
            cc.enabled = false;

        player.transform.position = CurrentCheckpoint;

        if (cc != null)
            cc.enabled = true;
    }
}