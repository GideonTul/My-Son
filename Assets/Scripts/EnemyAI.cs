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
    public PlayableDirector deathCutscene;

    [Header("Behavior")]
    public AudioClip[] aggroSound;
    public AudioClip musicSting;
    public MusicTrack deathMusic;
    public MusicTrack chaseMusic;
    public MusicTrack normalMusic;
    public AudioClip atkSound;
    public AudioClip babyScream;

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
        currentState = State.Roam;
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

        if (isRespawning == false) AudioManager.Instance.PlayMusic(chaseMusic, 0.1f, 0.4f);
    }
    private bool cutsceneTriggered = false;
    private void ChasePlayer(float distance)
    {
        agent.SetDestination(player.transform.position);

        if (distance < 9f && !cutsceneTriggered)
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

    //private IEnumerator AttackCutscene()
    //{
    //    agent.isStopped = true;
    //    isMad = false;


    //    FirstPersonController controller = player.GetComponent<FirstPersonController>();
    //    controller.enabled = false;
    //    Vector3 targetLook = transform.position + Vector3.up * 2f;

    //    transform.LookAt(player.transform);
    //    player.transform.LookAt(targetLook);

    //    isRespawning = true;
    //    anim.SetTrigger("Attack");
    //    AudioManager.Instance.PlaySFX(atkSound, 0.5f);
    //    CameraShake.Instance.Shake(0.7f, 0.4f);
    //    // AudioManager.Instance.PlayMusic(deathMusic, 2f, 1f);
    //    AudioManager.Instance.PlaySFX(babyScream, 0.5f);
    //    yield return new WaitForSeconds(0.5f);

    //    yield return new WaitForSeconds(0.8f);

        
    //    StartCoroutine(RespawnRoutine());

    //}
    //private IEnumerator RespawnRoutine()
    //{
    //    FirstPersonController controller = player.GetComponent<FirstPersonController>();

    //    yield return StartCoroutine(ScreenFader.Instance.FadeToBlack());

    //    Time.timeScale = 0f;

    //    yield return new WaitForSecondsRealtime(0.5f);

    //    GameManager.Instance.RespawnPlayer(player);
        
    //    yield return new WaitForSecondsRealtime(0.5f);

    //    Time.timeScale = 1f;
    //    AudioManager.Instance.PlayMusic(normalMusic, 2f, 0.2f);
    //    yield return StartCoroutine(ScreenFader.Instance.FadeFromBlack());
    //    controller.enabled = true;
    //    player.transform.rotation = originalPlayerRot;
    //    yield return new WaitForSeconds(1f);
        
    //    isRespawning = false;
    //    cutsceneTriggered = false;
    //    var noise = player.GetComponent<NoiseEvent>();
    //    noise.enabled = true;
        
    //}
    private void CalmDownImmediate()
    {
        isMad = false;
        atk = false;

        agent.isStopped = false;
        agent.ResetPath();

        agent.speed = 5f;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPoint].position);

    }

    private void CalmDown()
    {
        if (!isMad)
            return;

        isMad = false;
        atk = false;

        agent.speed = 5f;

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