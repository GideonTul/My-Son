using AudioSystem;
using Unity.VisualScripting;
using UnityEngine;

public class FootstepPlayerForTesting : MonoBehaviour
{
    [SerializeField] private SoundData foot;
    public void PlayFootstep()
    {
        AudioManager.Instance.Play(foot);
    }
}
