using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    [Header("Footsteps")]
    [SerializeField] private AudioClip[] footstepAudio;
    [SerializeField] private float footstepVolume = 0.2f;
    private AudioSource footstepSource;


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

    private bool isMad = false;

    private int currentPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
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

        float distFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        ControlAggro(distFromPlayer);

        float speed = agent.velocity.magnitude;

        Debug.Log("Enemy Speed: " +  speed);
        anim.SetFloat("Speed", speed);

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
    bool atk = false;
    private void ControlAggro(float dist)
    {
        if (dist > 35) { 
            isMad = false;
            agent.speed = 5f;
            agent.destination = patrolPoints[currentPoint].position;
            AudioManager.Instance.PlayMusic(normalMusic, 1f, 0.2f);
            atk = false;
        }
        if (dist < 5 && !atk)
        {
            AudioManager.Instance.PlaySFX(atkSound, 0.5f);
            atk = true;
        }

        if (isMad) {
            agent.destination = player.transform.position;
            return; 
        }

        if (dist < 30) { 
            isMad = true;
            agent.speed = 11f;
            AudioManager.Instance.PlaySFX(aggroSound[Random.Range(0, aggroSound.Length)], 0.5f);
            //AudioManager.Instance.PlaySFX(musicSting, 0.2f);
            AudioManager.Instance.PlayMusic(chaseMusic, 0.1f, 0.5f);
            agent.destination = player.transform.position;
        }
        else
        {
            isMad = false;
        }
    }

    public void PlayFootstep()
    {
        if (footstepAudio.Length == 0)
            return;

        AudioClip clip = footstepAudio[Random.Range(0, footstepAudio.Length)];

        footstepSource.PlayOneShot(clip, footstepVolume);
    }
}