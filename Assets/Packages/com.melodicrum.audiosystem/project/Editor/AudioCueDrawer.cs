using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AudioCueAttribute))]
public class AudioCueDrawer : PropertyDrawer
{
    private static AudioCueRegistry cachedRegistry;

    private static AudioCueRegistry GetRegistry()
    {
        if (cachedRegistry != null)
            return cachedRegistry;

        string[] guids = AssetDatabase.FindAssets("t:AudioCueRegistry");

        if (guids.Length == 0)
            return null;

        if (guids.Length > 1)
            Debug.LogWarning("[AudioCueDrawer] Multiple AudioCueRegistry assets found in the " +
                              "project — using the first one located. There should only be one.");

        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        cachedRegistry = AssetDatabase.LoadAssetAtPath<AudioCueRegistry>(path);
        return cachedRegistry;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.LabelField(position, label.text, "[AudioCue] only works on string fields.");
            return;
        }

        AudioCueRegistry registry = GetRegistry();

        EditorGUI.BeginProperty(position, label, property);

        if (registry == null)
        {
            property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
            EditorGUI.EndProperty();
            return;
        }

        // values[i] is the real string written back to the property;
        // displayLabels[i] is what's shown in the popup. Kept in lockstep
        // so the popup can show an annotated label without corrupting
        // the underlying value.
        var values = new List<string>(registry.Cues);
        values.Insert(0, string.Empty);

        var displayLabels = new List<string>(registry.Cues);
        displayLabels.Insert(0, "(None)");

        int currentIndex = values.IndexOf(property.stringValue);

        if (currentIndex < 0)
        {
            // The stored value doesn't match anything currently in the registry —
            // e.g. a cue was renamed or deleted, or this was a hand-typed value.
            // OnGUI runs on every repaint, not just on user interaction, so if we
            // fell back to index 0 here (like before), the very next repaint would
            // silently overwrite this field to empty with no click and no warning.
            // Instead, show the existing value as its own "(unlisted)" entry so it
            // round-trips untouched until someone deliberately picks something else.
            values.Insert(1, property.stringValue);
            displayLabels.Insert(1, $"{property.stringValue} (unlisted)");
            currentIndex = 1;
        }

        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, displayLabels.ToArray());
        property.stringValue = values[newIndex];

        EditorGUI.EndProperty();
    }
}

//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//[CustomPropertyDrawer(typeof(AudioCueAttribute))]
//public class AudioCueDrawer : PropertyDrawer
//{
//    private static AudioCueRegistry cachedRegistry;

//    private static AudioCueRegistry GetRegistry()
//    {
//        if (cachedRegistry != null)
//            return cachedRegistry;

//        string[] guids = AssetDatabase.FindAssets("t:AudioCueRegistry");

//        if (guids.Length == 0)
//            return null;

//        if (guids.Length > 1)
//            Debug.LogWarning("[AudioCueDrawer] Multiple AudioCueRegistry assets found in the " +
//                              "project — using the first one located. There should only be one.");

//        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
//        cachedRegistry = AssetDatabase.LoadAssetAtPath<AudioCueRegistry>(path);
//        return cachedRegistry;
//    }

//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        if (property.propertyType != SerializedPropertyType.String)
//        {
//            EditorGUI.LabelField(position, label.text, "[AudioCue] only works on string fields.");
//            return;
//        }

//        AudioCueRegistry registry = GetRegistry();

//        EditorGUI.BeginProperty(position, label, property);

//        if (registry == null)
//        {
//            property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
//            EditorGUI.EndProperty();
//            return;
//        }

//        var options = new List<string>(registry.Cues);
//        options.Insert(0, "(None)");

//        int currentIndex = options.IndexOf(property.stringValue);
//        if (currentIndex < 0)
//            currentIndex = 0;

//        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());
//        property.stringValue = newIndex == 0 ? string.Empty : options[newIndex];

//        EditorGUI.EndProperty();
//    }
//}
