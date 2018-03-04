using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAsteroids : MonoBehaviour {

    // For stars
    public GameObject asteroid;

    private System.Random rand = new System.Random(System.DateTime.Now.Millisecond);

    void Update()
    {
        GenerateAsteroid();
    }


    /// <summary>
    /// Generates regular stars
    /// </summary>
    void GenerateAsteroid()
    {
        Vector3 screenPoint = new Vector3(rand.Next(0, Screen.height), rand.Next(0, Screen.width), 0);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPos.z = 0;
        Instantiate(asteroid, worldPos, Quaternion.identity);
    }
}
