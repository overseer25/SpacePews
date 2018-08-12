using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour {

    public Sprite[] thrusterAnim;
    public Sprite[] afterburnerAnim;

    public float playspeed = 0.1f;
    private float changeSprite = 0.0f;
    private int index = 0;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Boost"))
        {
            Debug.Log("pressing shift");
            if (Time.time > changeSprite)
            {
                changeSprite = Time.time + playspeed;
                index++;
                if (index >= afterburnerAnim.Length) { index = 0; } // Restart animation
                GetComponent<SpriteRenderer>().sprite = afterburnerAnim[index];
            }
        }
        else
        {
            Debug.Log("Not pressing shift");
            if (Time.time > changeSprite)
            {
                changeSprite = Time.time + playspeed;
                index++;
                if (index >= thrusterAnim.Length) { index = 0; } // Restart animation
                GetComponent<SpriteRenderer>().sprite = thrusterAnim[index];
            }
        }

        
    }
}
