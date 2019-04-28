using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtils {

    public static float AngleBetweenTwoVectors(Vector3 a, Vector3 b)
    {
        return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Converts a float to its percentage equivalent.
    /// </summary>
    /// <returns></returns>
    public static string ConvertToPercent(float val)
    {
        return (val * 100).ToString("N0") + "%";
    }
}
