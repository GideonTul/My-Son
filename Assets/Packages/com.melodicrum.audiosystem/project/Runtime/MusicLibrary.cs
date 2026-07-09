using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps a cue name (string, picked via the [AudioCue] dropdown — see
/// AudioCueDrawer and AudioCueRegistry) to the MusicTrack that should play for it.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Music Library", fileName = "MusicLibrary")]
public class MusicLibrary : ScriptableObject
{
    [System.Serializable]
    public struct Entry
    {
        [AudioCue]
        public string cue;
        public MusicTrack track;
    }

    [SerializeField]
    private Entry[] entries;

    private Dictionary<string, MusicTrack> lookup;

    private void BuildLookupIfNeeded()
    {
        if (lookup != null)
            return;

        lookup = new Dictionary<string, MusicTrack>();

        if (entries == null)
            return;

        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(entry.cue))
                continue;

            if (lookup.ContainsKey(entry.cue))
            {
                Debug.LogWarning($"[MusicLibrary] Duplicate entry for cue '{entry.cue}' — " +
                                  $"the first mapping will be used, check the Inspector list.");
                continue;
            }

            lookup.Add(entry.cue, entry.track);
        }
    }

    public MusicTrack Get(string cue)
    {
        if (string.IsNullOrEmpty(cue))
            return null;

        BuildLookupIfNeeded();

        if (!lookup.TryGetValue(cue, out MusicTrack track))
        {
            Debug.LogWarning($"[MusicLibrary] No track mapped for cue '{cue}'.");
            return null;
        }

        if (track == null)
        {
            Debug.LogWarning($"[MusicLibrary] Cue '{cue}' is mapped but has no MusicTrack " +
                              $"assigned in the Inspector — check the entry list.");
            return null;
        }

        return track;
    }

    private void OnEnable()
    {
        lookup = null;
    }
}

//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// Maps a cue name (string, picked via the [AudioCue] dropdown — see
///// </summary>
//[CreateAssetMenu(menuName = "Audio/Music Library", fileName = "MusicLibrary")]
//public class MusicLibrary : ScriptableObject
//{
//    [System.Serializable]
//    public struct Entry
//    {
//        [AudioCue]
//        public string cue;
//        public MusicTrack track;
//    }

//    [SerializeField]
//    private Entry[] entries;

//    private Dictionary<string, MusicTrack> lookup;

//    private void BuildLookupIfNeeded()
//    {
//        if (lookup != null)
//            return;

//        lookup = new Dictionary<string, MusicTrack>();

//        foreach (var entry in entries)
//        {
//            if (string.IsNullOrEmpty(entry.cue))
//                continue;

//            if (lookup.ContainsKey(entry.cue))
//            {
//                Debug.LogWarning($"[MusicLibrary] Duplicate entry for cue '{entry.cue}' — " +
//                                  $"the first mapping will be used, check the Inspector list.");
//                continue;
//            }

//            lookup.Add(entry.cue, entry.track);
//        }
//    }

//    public MusicTrack Get(string cue)
//    {
//        if (string.IsNullOrEmpty(cue))
//            return null;

//        BuildLookupIfNeeded();

//        if (lookup.TryGetValue(cue, out MusicTrack track))
//            return track;

//        Debug.LogWarning($"[MusicLibrary] No track mapped for cue '{cue}'.");
//        return null;
//    }

//    private void OnEnable()
//    {
//        lookup = null;
//    }
//}
