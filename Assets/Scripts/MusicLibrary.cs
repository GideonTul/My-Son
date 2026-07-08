using UnityEngine;

public enum MusicCue { None, Main_Menu, Forest_Ambient, Chase, Victory }

[CreateAssetMenu(menuName = "Resources/Audio/Music Library")]
public class MusicLibrary : ScriptableObject
{
    [System.Serializable]
    public struct Entry { public MusicCue cue; public MusicTrack track; }

    public Entry[] entries;

    public MusicTrack Get(MusicCue cue)
    {
        foreach (var e in entries)
            if (e.cue == cue) return e.track;

        Debug.LogWarning($"No track mapped for cue {cue}");
        return null;
    }
}