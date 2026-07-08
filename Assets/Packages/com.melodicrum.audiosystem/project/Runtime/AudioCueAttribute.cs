using UnityEngine;

/// <summary>
/// Put this on a string field to have the Inspector render it as a
/// dropdown populated from the project's AudioCueRegistry, instead of
/// a free-text field. The field is still just a plain string at
/// runtime, e.g. AudioManager.Instance.PlayMusic("Chase", ...).
/// </summary>
public class AudioCueAttribute : PropertyAttribute
{
}
