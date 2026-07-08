using UnityEngine;

public class StartPuzzle : MonoBehaviour
{

    [SerializeField] private AudioSource music;
    [SerializeField] private MusicSequencePuzzle musicSequencePuzzle;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (musicSequencePuzzle.enabled != false)
            {
                musicSequencePuzzle.StartPuzzle();
                music.enabled = false;
            }
        }
    }
}
