using UnityEngine;

public class AnimationFootstepProxy : MonoBehaviour
{
    private EnemyAI parent;

    void Awake()
    {
        parent = GetComponentInParent<EnemyAI>();
    }
    public void PlayFootstep()
    {
        if (parent != null)
        {
            parent.PlayFootstep();
        }
    }
}