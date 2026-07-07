using StarterAssets;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;


public class EnemyAI : MonoBehaviour
{

    [Header("Footsteps")]
    [SerializeField] private AudioClip[] footstepAudio;
    [SerializeField] private float footstepVolume = 0.2f;
    private AudioSource footstepSource;
    private static bool playerSafe = false;

    [Header("Patrol Points")]
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    [Header("Animator")]
    public Animator anim;
    public PlayableDirector deathCutscene;

    [Header("Behavior")]
    public AudioClip[] aggroSound;
    public AudioClip musicSting;
    public MusicTrack deathMusic;
    public MusicTrack chaseMusic;
    public MusicTrack normalMusic;
    public AudioClip atkSound;
    public AudioClip babyScream;
    public float aggroSpeed = 10f;
    public float walkSpeed = 5f;


    private GameObject player;
    private FirstPersonController playerAudio;
    Quaternion originalPlayerRot;
    private bool isMad = false;
    private bool isRespawning = false;


    private int currentPoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerAudio = player.GetComponent<FirstPersonController>();
        footstepSource = GetComponent<AudioSource>();
        originalPlayerRot = player.transform.rotation;
        agent.speed = walkSpeed;
        deathCutscene.stopped += OnCutsceneEnd;
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

        // Debug.Log("Enemy Speed: " +  speed);
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
        
        if (playerSafe || isRespawning)
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
        {
            agent.destination = noisePosition;
            return;
        }
        BecomeAggro();
    }

    private void BecomeAggro()
    {
        isMad = true;

        agent.speed = aggroSpeed;

        AudioManager.Instance.PlaySFX(
            aggroSound[Random.Range(0, aggroSound.Length)],
            0.5f);

        if (isRespawning == false) AudioManager.Instance.PlayMusic(chaseMusic, 0.1f, 0.4f);
    }
    private bool cutsceneTriggered = false;
    private void ChasePlayer(float distance)
    {
        agent.SetDestination(player.transform.position);

        if (distance < 3f && !cutsceneTriggered)
        {
            cutsceneTriggered = true;
            var noise = player.GetComponent<NoiseEvent>();
            noise.enabled = false;
            isRespawning = true;
            player.GetComponent<FirstPersonController>().enabled = false;
            PlayDeathCutscene();
            CalmDownImmediate();
            
        }
    }
    void OnCutsceneEnd(PlayableDirector pd)
    {
        Debug.Log("Death custscene end!");
        isRespawning = false;
        cutsceneTriggered = false;
        var noise = player.GetComponent<NoiseEvent>();
        noise.enabled = true;
        player.GetComponent<FirstPersonController>().enabled = true;

    }

    private void CalmDownImmediate()
    {
        isMad = false;

        agent.isStopped = false;
        agent.ResetPath();

        agent.speed = walkSpeed;
        currentPoint = Random.Range(0, patrolPoints.Length);

        agent.Warp(patrolPoints[currentPoint].position);
    }

    private void CalmDown()
    {
        if (!isMad)
            return;

        isMad = false;

        agent.speed = walkSpeed;

        agent.SetDestination(patrolPoints[currentPoint].position);

        if (isRespawning == false) AudioManager.Instance.PlayMusic(normalMusic, 2f, 0.2f);
    }

    public void PlayFootstep()
    {
        if (footstepAudio.Length == 0)
            return;

        AudioClip clip = footstepAudio[Random.Range(0, footstepAudio.Length)];

        footstepSource.PlayOneShot(clip, footstepVolume);
    }
    public void PlayDeathCutscene()
    {
        deathCutscene.Play();
        AudioManager.Instance.PlayMusic(normalMusic, 2f, 0.2f);
        GameManager.Instance.RespawnPlayer(player);
    }
}