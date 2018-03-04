using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpText : MonoBehaviour {

    public Sprite[] spriteList;

    private Sprite spriteText;
    public SpriteRenderer sRenderer;
    private float fadeTime = 0.0f;
    private float fadeSpeed;
    private int timeBeforeFade = 0; // A counter that will count up to a certain number. Once it reaches
                                    // this number, the sprite will begin to fade.

    private GameObject player;

    /// <summary>
    /// Initializes the pop-up text by allowing the callee to specify text and fade time.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fadeTime"></param>
    public void Initialize(string text, float fadeSpeed)
    {
        player = GameObject.FindGameObjectWithTag("Player");
        AssignSprite(text); // Assign the sprite of the associated text.
        this.fadeSpeed = fadeSpeed;
        
        transform.position = new Vector3(player.transform.position.x, 
            player.transform.position.y, player.transform.position.z); // Set this once
    }
	
	// Update is called once per frame
	void Update()
    {
        // Doesn't have to follow the player, so we don't do this.
        //transform.position = player.transform.position; 

        if(Time.time > fadeTime)
        {
            fadeTime = Time.time + fadeSpeed;
            // Delays the fade.
            if (timeBeforeFade == 100)
            {
                float colorAlpha = sRenderer.color.a;
                sRenderer.color = new Color(1f, 1f, 1f, (colorAlpha * 0.95f)); // Slowly fade the sprite

                // If the sprite is invisible, destroy the PopUpText object.
                if(colorAlpha <= 0)
                {
                    Destroy(gameObject);
                }

                // Move the sprite upward
                transform.position = new Vector3(transform.position.x,
                    transform.position.y + (0.01f), transform.position.z); // Set this once
            }
            else { timeBeforeFade++; }

        }



    }

    /// <summary>
    /// Assigns a sprite to the pop-up, given text.
    /// </summary>
    /// <param name="text"></param>
    void AssignSprite(string text)
    {
        if(text == "Stone")
        {
            sRenderer.sprite = spriteList[0];
        }
    }
}
