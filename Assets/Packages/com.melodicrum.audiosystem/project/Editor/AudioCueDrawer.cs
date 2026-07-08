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

        var options = new List<string>(registry.Cues);
        options.Insert(0, "(None)");

        int currentIndex = options.IndexOf(property.stringValue);
        if (currentIndex < 0)
            currentIndex = 0;

        int newIndex = EditorGUI.Popup(position, label.text, currentIndex, options.ToArray());
        property.stringValue = newIndex == 0 ? string.Empty : options[newIndex];

        EditorGUI.EndProperty();
    }
}
