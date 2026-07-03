using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    public Animator anim;

    public AudioClip aggroSound;

    private GameObject player;

    private bool isMad = false;

    private int currentPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0)
            return;

        float distFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        ControlAggro(distFromPlayer);

        // Update animation
        anim.SetFloat("Speed", agent.velocity.magnitude);

        // Have we reached the patrol point?
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
                currentPoint = 0;

            agent.destination = patrolPoints[currentPoint].position;
        }
    }

    private void ControlAggro(float dist)
    {
        if (isMad) return;

        if (dist < 20) { 
            isMad = true;
            AudioManager.Instance.PlaySFX(aggroSound, 0.5f);
        }
        else
        {
            isMad = false;
        }
    }
}