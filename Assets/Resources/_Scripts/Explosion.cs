using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    public Sprite[] explosionSprites;
    public float playspeed = 0.1f;
    private float changeSprite = 0.0f;
    private int index = 0;
	
	// Update is called once per frame
	void Update ()
    {
	    if(Time.time > changeSprite)
        {
            if (index >= explosionSprites.Length) { Destroy(gameObject); }
            else
            {
                changeSprite = Time.time + playspeed;
                GetComponentInChildren<SpriteRenderer>().sprite = explosionSprites[index];
                index++;
            }     
        }
	}
}
