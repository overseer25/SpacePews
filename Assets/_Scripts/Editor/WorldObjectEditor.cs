using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(WorldChunk))]
public class WorldObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WorldChunk chunk = target as WorldChunk;
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Spawn Positions"))
        {
            chunk.RandomizeSpawnPositions();
        }
    }
}
