using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpText : MonoBehaviour {

    private TextMesh textMesh;
    private float fadeTime = 0.0f;
    private float fadeSpeed = 0.2f;
    private int delay = 0;
    private int timeBeforeFade = 0; // A counter that will count up to a certain number. Once it reaches
                                    // this number, the sprite will begin to fade.

    private new MeshRenderer renderer;

    /// <summary>
    /// Initializes the pop-up text by allowing the callee to specify text and fade time.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="fadeTime"></param>
    public void Initialize(GameObject target, string text)
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = text;
        renderer = GetComponent<MeshRenderer>();
        Debug.Log("Renderer: " + renderer);
        
        transform.position = new Vector3(target.transform.position.x, 
            target.transform.position.y, target.transform.position.z); // Set this once
    }
	
	// Update is called once per frame
	void Update()
    {
        if (Time.time > fadeTime)
        {
            fadeTime = Time.deltaTime + fadeSpeed;

            if(delay == 100)
            {
                float colorAlpha = renderer.material.color.a;
                renderer.material.color = new Color(1f, 1f, 1f, (colorAlpha - 0.01f)); // Slowly fade the sprite


                // If the sprite is invisible, destroy the PopUpText object.
                if (colorAlpha <= 0)
                {
                    Destroy(gameObject);
                }
            }
            else { delay++; }


            // Move the sprite upward
            transform.position = new Vector3(transform.position.x,
                transform.position.y + (0.01f), transform.position.z); // Set this once

        }



    }
}
