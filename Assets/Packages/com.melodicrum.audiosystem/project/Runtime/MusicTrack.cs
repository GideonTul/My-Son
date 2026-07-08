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
}
