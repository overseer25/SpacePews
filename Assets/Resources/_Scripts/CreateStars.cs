using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateStars : MonoBehaviour {

    // For shooting stars
    public GameObject shootingStar;
    private float nextShootingStar = 0.0f;
    private float shootingStarRate = 5.0f;

    // For stars
    public GameObject star;
    private float nextStar = 0.0f;
    private float starRate = 0.2f;

    private System.Random rand;

    void Start()
    {
        rand = new System.Random(System.DateTime.Now.Millisecond);
    }
	
	// Update is called once per frame
	void Update () {

        GenerateShootingStar();
        GenerateStar();
	}

    /// <summary>
    /// Generates shooting stars
    /// </summary>
    void GenerateShootingStar()
    {
        Vector3 screenPoint = new Vector3(rand.Next(0, Screen.height), rand.Next(0, Screen.width), 0);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPos.z = 0;
        if (Time.time > nextShootingStar)
        {
            nextShootingStar = Time.time + shootingStarRate;
            Instantiate(shootingStar, worldPos, Quaternion.identity);
        }
    }


    /// <summary>
    /// Generates regular stars
    /// </summary>
    void GenerateStar()
    {
        Vector3 screenPoint = new Vector3(rand.Next(0, Screen.height), rand.Next(0, Screen.width), 0);
        var worldPos = Camera.main.ScreenToWorldPoint(screenPoint);
        worldPos.z = 0;
        worldPos.x += rand.Next(-5, 10); // Weird bug where only populates left side of screen, so shift the x value.
        if (Time.time > nextStar)
        {
            nextStar = Time.time + starRate;
            Instantiate(star, worldPos, Quaternion.identity);
        }
    }
}
