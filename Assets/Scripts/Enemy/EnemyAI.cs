using AudioSystem;
using StarterAssets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public enum EnemyState
{
    Patrol,
    Investigate,
    Chase,
    Respawning
}

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5;
    [SerializeField] private float chaseSpeed = 10;

    [Header("Detection")]
    [SerializeField] private float loseDistance = 55;
    [SerializeField] private float killDistance = 3;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Cutscene")]
    [SerializeField] private PlayableDirector deathCutscene;

    [Header("Audio")]
    [SerializeField] private SoundData aggroSounds;
    [SerializeField] private SoundData footsteps;



    private EnemyState state = EnemyState.Patrol;

    private NavMeshAgent agent;

    private GameObject player;

    private FirstPersonController playerController;

    private AudioSource footstepSource;

    private int patrolIndex;

    private bool playerSafe;


    private static bool playerIsDying;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        footstepSource = GetComponent<AudioSource>();

        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) { Debug.Log("EnemyAI could not find player."); return; }

        playerController = player.GetComponent<FirstPersonController>();
    }
    private void OnEnable()
    {
        GameEvents.OnPlayerSafeChanged += HandlePlayerSafeChanged;
        GameEvents.OnNoiseMade += HearNoise;
        GameEvents.OnPlayerRespawned += HandlePlayerRespawned;
    }
    private void OnDisable()
    {
        GameEvents.OnPlayerSafeChanged -= HandlePlayerSafeChanged;
        GameEvents.OnNoiseMade -= HearNoise;
        GameEvents.OnPlayerRespawned -= HandlePlayerRespawned;
    }
    private void Start()
    {
        agent.speed = walkSpeed;

        EnterPatrol();
    }
    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);

        switch (state)
        {
            case EnemyState.Patrol:
                PatrolUpdate();
                break;

            case EnemyState.Investigate:
                InvestigateUpdate();
                break;

            case EnemyState.Chase:
                ChaseUpdate();
                break;

            case EnemyState.Respawning:
                break;
        }
    }
    private void EnterPatrol()
    {
        state = EnemyState.Patrol;

        agent.speed = walkSpeed;

        patrolIndex = Random.Range(0, patrolPoints.Length);

        agent.SetDestination(patrolPoints[patrolIndex].position);
    }
    private void PatrolUpdate()
    {
        if (playerSafe)
            return;

        float distance =
            Vector3.Distance(transform.position, player.transform.position);

        if (distance <= playerController.CurrentNoiseRadius)
        {
            EnterChase();
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            patrolIndex++;

            if (patrolIndex >= patrolPoints.Length)
                patrolIndex = 0;

            agent.SetDestination(patrolPoints[patrolIndex].position);
        }
    }
    private void EnterChase()
    {
        state = EnemyState.Chase;

        agent.speed = chaseSpeed;

        AudioManager.Instance.PlayAttached(aggroSounds, transform);

        GameEvents.OnEnemyStartedChasing?.Invoke();
    }
    private void EnterInvestigate(Vector3 location)
    {
        state = EnemyState.Investigate;

        agent.speed = walkSpeed;

        agent.SetDestination(location);
    }
    private void InvestigateUpdate()
    {
        if (playerSafe)
        {
            EnterPatrol();
            return;
        }

        float distance =
            Vector3.Distance(transform.position, player.transform.position);

        if (distance <= playerController.CurrentNoiseRadius)
        {
            EnterChase();
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance)
        {
            EnterPatrol();
        }
    }
    private void ChaseUpdate()
    {
        if (playerSafe)
        {
            GameEvents.OnEnemyStoppedChasing?.Invoke();
            EnterPatrol();
            return;
        }

        agent.SetDestination(player.transform.position);

        float distance =
            Vector3.Distance(transform.position, player.transform.position);

        if (distance > loseDistance)
        {
            GameEvents.OnEnemyStoppedChasing?.Invoke();
            EnterPatrol();
            return;
        }

        if (distance <= killDistance)
        {
            KillPlayer();
        }
    }
    private void HearNoise(Vector3 noisePosition, float radius)
    {
        if (state == EnemyState.Respawning)
            return;

        if (playerSafe)
            return;

        if (state == EnemyState.Chase)
            return;

        float distance =
            Vector3.Distance(transform.position, noisePosition);

        if (distance <= radius)
        {
            EnterChase();
        }
        else
        {
            EnterInvestigate(noisePosition);
        }
    }
    private void KillPlayer()
    {
        if (state == EnemyState.Respawning)
            return;

        if (playerIsDying)
            return;

        playerIsDying = true;
        GameEvents.OnEnemyStoppedChasing?.Invoke();
        EnterRespawning();
    }
    private void EnterRespawning()
    {
        state = EnemyState.Respawning;

        agent.ResetPath();

        GameEvents.OnPlayerKilled?.Invoke(deathCutscene);
        //deathCutscene.Play();
    }
    private void HandlePlayerRespawned()
    {
        if (state != EnemyState.Respawning)
            return;

        patrolIndex = Random.Range(0, patrolPoints.Length);

        agent.Warp(patrolPoints[patrolIndex].position);

        playerIsDying = false;

        EnterPatrol();
    }
    private void HandlePlayerSafeChanged(bool safe)
    {
        playerSafe = safe;

        if (safe && state == EnemyState.Chase)
        {
            EnterPatrol();
        }
    }
    public void PlayFootstep()
    {

        AudioManager.Instance.PlayAttached(footsteps, transform);
    }
}