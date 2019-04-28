using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimImagePulsate : MonoBehaviour {

    [Header("Parameters")]
    public float playspeed = 0.1f;
    public float rateOfChange = 0.002f;
    public int cycles = 10;

    private float changeSprite = 0.0f;
    private int counter = 0;
    private bool scaleUp = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time > changeSprite)
        {
            if(!scaleUp)
            {
                changeSprite = Time.time + playspeed;
                gameObject.transform.localScale *= 1 - rateOfChange;
                counter++;
            }
            else
            {
                changeSprite = Time.time + playspeed;
                gameObject.transform.localScale *= 1 + rateOfChange;
                counter--;
            }

            if (counter == 0 || counter == cycles)
                scaleUp = !scaleUp;

        }
    }
}
