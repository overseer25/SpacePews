using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Methods that are helpful for dealing with players.
/// </summary>
public class PlayerUtils : MonoBehaviour
{
    /// <summary>
    /// Gets the player object closest to the target.
    /// </summary>
    /// <param name="target"> The target to measure distance from. </param>
    /// <returns></returns>
    public static GameObject GetClosestPlayer(GameObject target)
    {
        // The closest player.
        GameObject result = null;

        var minDistance = float.MaxValue;
        foreach(var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            var distance = Vector2.Distance(target.transform.position, player.transform.position);
            if (distance < minDistance)
            {
                result = player;
                minDistance = distance;
            }
        }

        return result;
    }
	
}
