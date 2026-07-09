using UnityEngine;

/// <summary>
/// A playable piece of music: an optional intro clip that plays once,
/// followed by a loop clip. If introClip is null, loopClip plays
/// (and loops) immediately.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Music Track", fileName = "MusicTrack")]
public class MusicTrack : ScriptableObject
{
    public AudioClip introClip;
    public AudioClip loopClip;

    //[Min(0f)]
    //[Tooltip("How long to crossfade from introClip into loopClip, in seconds. " +
    //         "Only used when both introClip and loopClip are assigned. This is " +
    //         "separate from the fadeTime passed to PlayMusic(), which controls " +
    //         "the transition BETWEEN tracks rather than within one.")]
    //public float introToLoopFadeTime = 0.1f;
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (introClip == null && loopClip == null)
            Debug.LogWarning($"[MusicTrack] '{name}' has no introClip or loopClip assigned — " +
                              "it will play silently.");
    }
#endif
}

//using UnityEngine;

///// <summary>
///// A playable piece of music: an optional intro clip that plays once,
///// followed by a loop clip. If introClip is null, loopClip plays
///// (and loops) immediately.
///// </summary>
//[CreateAssetMenu(menuName = "Audio/Music Track", fileName = "MusicTrack")]
//public class MusicTrack : ScriptableObject
//{
//    public AudioClip introClip;
//    public AudioClip loopClip;
//}
