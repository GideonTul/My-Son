using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;

    private void Awake()
    {
        if (director == null)
            director = GetComponent<PlayableDirector>();


        director.RebuildGraph();


        director.time = 0;


        director.Play();
    }
}