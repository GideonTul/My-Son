using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [Header("Footsteps")]
    [SerializeField] private AudioClip[] footstepAudio;
    [SerializeField] private float footstepVolume = 0.2f;
    private AudioSource footstepSource;
    private static bool playerSafe = false;

    enum State
    {
        Roam, Aggro
    }

    State currentState;
    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    [Header("Animator")]
    public Animator anim;

    [Header("Behavior")]
    public AudioClip[] aggroSound;
    public AudioClip musicSting;
    public MusicTrack chaseMusic;
    public MusicTrack normalMusic;
    public AudioClip atkSound;

    private GameObject player;
    private FirstPersonController playerAudio;

    private bool isMad = false;

    private int currentPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerAudio = player.GetComponent<FirstPersonController>();
        footstepSource = GetComponent<AudioSource>();

        currentState = State.Roam;

        if (patrolPoints.Length > 0)
        {
            agent.destination = patrolPoints[0].position;
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0)
            return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        CheckAggro(distance);

        float speed = agent.velocity.magnitude;

        Debug.Log("Enemy Speed: " +  speed);
        anim.SetFloat("Speed", speed);

        if (!isMad)
        {
            Roam();
        }
        else
        {
            ChasePlayer(distance);
        }
    }
    public static void PlayerReachedSafeZone()
    {
        playerSafe = true;
    }

    public static void PlayerLeftSafeZone()
    {
        playerSafe = false;
    }

    public static bool GetPlayerSafetyStatus()
    {
        return playerSafe;
    }


    bool atk = false;
    private void Roam()
    {
        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPoint++;

            if (currentPoint >= patrolPoints.Length)
                currentPoint = 0;

            agent.SetDestination(patrolPoints[currentPoint].position);
        }
    }

    private void CheckAggro(float distance)
    {
        if (playerSafe)
        {
            CalmDown();
            return;
        }

        bool playerHeard = distance <= playerAudio.CurrentNoiseRadius;


        if (!isMad && playerHeard)
        {
            BecomeAggro();
        }
        else if (isMad && distance > 55f)
        {
            CalmDown();
        }
    }

    public void HearNoise(Vector3 noisePosition, float noiseRadius)
    {
        if (playerSafe || isMad)
            return;

        float distance = Vector3.Distance(transform.position, noisePosition);

        if (distance > noiseRadius)
            return;

        BecomeAggro();
    }

    private void BecomeAggro()
    {
        isMad = true;
        atk = false;

        agent.speed = 11f;

        AudioManager.Instance.PlaySFX(
            aggroSound[Random.Range(0, aggroSound.Length)],
            0.5f);

        AudioManager.Instance.PlayMusic(chaseMusic, 0.1f, 0.5f);
    }

    private void ChasePlayer(float distance)
    {
        agent.SetDestination(player.transform.position);

        if (distance < 5f && !atk)
        {
            atk = true;
            AudioManager.Instance.PlaySFX(atkSound, 0.5f);
        }
    }

    private void CalmDown()
    {
        if (!isMad)
            return;

        isMad = false;
        atk = false;

        agent.speed = 5f;

        agent.SetDestination(patrolPoints[currentPoint].position);

        AudioManager.Instance.PlayMusic(normalMusic, 1f, 0.2f);
    }

    public void PlayFootstep()
    {
        if (footstepAudio.Length == 0)
            return;

        AudioClip clip = footstepAudio[Random.Range(0, footstepAudio.Length)];

        footstepSource.PlayOneShot(clip, footstepVolume);
    }
}