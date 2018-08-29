using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpText : MonoBehaviour {

    private new MeshRenderer renderer;
    private TextMesh textMesh;
    private float fadeTime = 0.0f;
    private float fadeSpeed = 0.2f;
    private int delay = 0;

    /// <summary>
    /// Initializes the pop-up text by allowing the callee to specify text and fade time.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="text"></param>
    /// <param name="itemTier"></param>
    public void Initialize(GameObject target, string text, ItemColorSelector itemTier, AudioClip sound = null)
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.color = ItemColors.colors[(int)itemTier];
        renderer = GetComponent<MeshRenderer>();
        renderer.material.color = new Color(1f, 1f, 1f, 1f); // Reset the alpha.

        transform.position = new Vector3(target.transform.position.x, 
            target.transform.position.y, target.transform.position.z); // Set this once

        gameObject.SetActive(true);

        // If a sound was provided, play it.
        if(sound != null)
        {
            GetComponent<AudioSource>().clip = sound;
            GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// Deals with fading and disabling the text.
    /// </summary>
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
                    gameObject.SetActive(false);
                }
            }
            else { delay++; }

            // Move the sprite upward
            transform.position = new Vector3(transform.position.x,
                transform.position.y + (0.01f), transform.position.z); // Set this once

        }
    }

    /// <summary>
    /// Called when the object is set to active.
    /// </summary>
    void OnEnable()
    {
        fadeTime = 0.0f;
        delay = 0;

    }
}
