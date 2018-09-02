using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour {

    public TextMeshPro textMesh;
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
        textMesh.text = text;
        textMesh.color = ItemColors.colors[(int)itemTier];
        textMesh.alpha = 1.0f;

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

            if(delay == 50)
            {
                textMesh.alpha -= 0.01f; // Slowly fade the sprite

                // If the text is invisible, deactivate the PopUpText object.
                if (textMesh.alpha <= 0)
                {
                    gameObject.SetActive(false);
                }
            }
            else { delay++; }

            // Move the sprite upward
            transform.position = new Vector3(transform.position.x,
                transform.position.y + (0.02f), transform.position.z); // Set this once

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
