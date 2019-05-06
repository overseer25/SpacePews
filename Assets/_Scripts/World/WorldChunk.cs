using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A world chunk is a grid square in the game world that keeps track of the world objects within it. When a world chunk is not visible to the player,
/// all objects that chunk is assigned to are disabled until the player gets close enough. This helps ensure that the world can efficient enabled and
/// disable objects efficiently, without each object having to individually check for player proximity.
/// </summary>
public class WorldChunk : MonoBehaviour
{
    public WorldObjectData[] chunkObjects;
    public int spawnCount;
    [HideInInspector]
    public Vector2[] spawnPositions;
    public bool visualize;

    private bool generated = false;
    private const int CHUNK_SIZE = 128;

    private void Start()
    {
        RandomizeSpawnPositions();
        StartCoroutine(Generate());
    }

    public void RandomizeSpawnPositions()
    {
        spawnPositions = new Vector2[spawnCount];
        for(int i = 0; i < spawnPositions.Length; i++)
        {
            spawnPositions[i] = new Vector2(Random.Range(-CHUNK_SIZE / 2, CHUNK_SIZE / 2), Random.Range(-CHUNK_SIZE / 2, CHUNK_SIZE / 2));
        }
    }

    private IEnumerator Generate()
    {
        if (generated)
            yield return null;

        foreach(var pos in spawnPositions)
        {
            var chance = Random.Range(0f, 1f);
            foreach(var obj in chunkObjects)
            {
                if (chance >= obj.rangeMin && chance <= obj.rangeMax)
                {
                    Instantiate(obj.worldObject, pos, Quaternion.identity);
                    break;
                }
            }
        }

        generated = true;
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualize)
            return;

        foreach(var pos in spawnPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pos, 1f);
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector2(CHUNK_SIZE, CHUNK_SIZE));
    }
}
