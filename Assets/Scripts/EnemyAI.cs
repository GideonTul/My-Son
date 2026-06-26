using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    public Animator anim;

    private int currentPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();


        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0)
            return;

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
}