using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MusicCue
{
    public string name;

    [AudioCue]
    public string cue;
}
