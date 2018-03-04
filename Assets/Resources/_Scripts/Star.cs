using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private bool fadeIn = true;
    private float fadeVal = 0.0f;
    private float fadeRate;
    private int numOfFlickers = 0; // The number of times the star will flicker before disappearing.

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // Start invisible
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);
        fadeRate = (float)rand.NextDouble() * (0.008f - 0.001f) + 0.001f;
        numOfFlickers = rand.Next(1, 7);
    }

	// Update is called once per frame
	void Update () {

        if(fadeIn)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, fadeVal);
            fadeVal += fadeRate;
            if(fadeVal >= 1.0f)
            {
                fadeIn = false;
            }
        }
        else
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, fadeVal);
            fadeVal -= fadeRate;
            if(fadeVal <= 0.1f)
            {
                if(numOfFlickers == 0) { Destroy(gameObject); } // Destroy the star after the number of flickers have passed.
                fadeIn = true;
                numOfFlickers--;
            }
        }
	}
}
