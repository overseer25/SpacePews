using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serialized data for saving user audio preferences.
/// </summary>
[Serializable]
public class AudioData
{
    public float master;
    public float sfx;
    public float music;
    public float ui;
}
