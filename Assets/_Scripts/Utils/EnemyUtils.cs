using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUtils : MonoBehaviour
{
    /// <summary>
    /// Finds the closest enemy to the target game object.
    /// </summary>
    /// <param name="maxRange">The furthest distance from the target to look. Leave empty for infinite range.</param>
    /// <returns>The closest enemy to the target, or null if one cannot be found.</returns>
    public static GameObject FindNearestEnemy(GameObject target, float maxRange = float.MaxValue)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        var closest = float.MaxValue;

        // The closest enemy.
        GameObject result = null;

        foreach(var enemy in enemies)
        {
            var distance = Mathf.Abs((target.transform.position - enemy.transform.position).magnitude);
            if(distance <= maxRange && distance < closest)
            {
                result = enemy;
                closest = distance;
            }
        }

        return result;
    }
}
