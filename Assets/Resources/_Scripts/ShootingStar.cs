using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingStar : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidBody;
    private bool fadeIn = true;
    private float fadeVal = 0.01f;
    private float fadeRate;
    private float speed = 0.0f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        System.Random rand = new System.Random(System.DateTime.Now.Millisecond);

        speed = (float)rand.NextDouble() * (10.0f - 5.0f) + 5.0f;
        fadeRate = (float)rand.NextDouble() * (0.05f - 0.001f) + 0.001f;
        rigidBody.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
        rigidBody.velocity = transform.up * -speed;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = new Color(1f, 1f, 1f, fadeVal);
        if(fadeIn)
        {
            fadeVal += fadeRate;
            if(fadeVal >= 1.0f)
            {
                fadeIn = false;
            }
        }
        else
        {
            fadeVal -= fadeRate;
            if (fadeVal <= 0.0f)
            {
                Destroy(gameObject);
            }
        }

    }
}
