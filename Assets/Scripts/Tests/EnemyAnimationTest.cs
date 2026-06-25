using UnityEngine;

public class EnemyAnimationTest : MonoBehaviour
{
    public Animator anim;

    private float timer;

    void Start()
    {
       // anim = GetComponent<Animator>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer < 3f)
        {
            anim.SetFloat("Speed", 0f); // Idle
        }
        else if (timer < 6f)
        {
            anim.SetFloat("Speed", 0.9f); // Walk
        }
        else if (timer < 9f)
        {
            anim.SetFloat("Speed", 5f); // Run
        }
        else
        {
            timer = 0f;
        }
    }
}