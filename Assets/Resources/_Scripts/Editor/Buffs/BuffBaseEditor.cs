using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Contains methods useful for custom buff editors.
/// </summary>
public abstract class BuffBaseEditor : Editor
{

    /// <summary>
    /// Display the description.
    /// </summary>
    /// <param name="buff"></param>
    public void DisplayDescription(Buff buff)
    {
        string tip = "A general overview of what the buff does.";
        EditorGUILayout.LabelField(new GUIContent(buff.BuildDescription(), tip), EditorStyles.boldLabel);
    }
}
