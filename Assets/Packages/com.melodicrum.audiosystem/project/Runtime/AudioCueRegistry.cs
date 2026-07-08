using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The single source of truth for what cue names exist in this project.
/// To add a new cue: open this asset in the Inspector and add a row.
/// </summary>
[CreateAssetMenu(menuName = "Audio/Cue Registry", fileName = "AudioCueRegistry")]
public class AudioCueRegistry : ScriptableObject
{
    [SerializeField]
    private List<string> cues = new List<string>();

    public IReadOnlyList<string> Cues => cues;

    public bool Contains(string cue) => !string.IsNullOrEmpty(cue) && cues.Contains(cue);

#if UNITY_EDITOR
    private void OnValidate()
    {
        var seen = new HashSet<string>();
        foreach (var c in cues)
        {
            if (string.IsNullOrWhiteSpace(c))
            {
                Debug.LogWarning("[AudioCueRegistry] List contains an empty cue name.");
                continue;
            }

            if (!seen.Add(c))
                Debug.LogWarning($"[AudioCueRegistry] Duplicate cue name: '{c}'.");
        }
    }
#endif
}
