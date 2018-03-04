using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySpriteAnim : MonoBehaviour {

    public Sprite[] spriteSheet;
    public float playspeed = 0.1f;
    private float changeSprite = 0.0f;
    private int index = 0;

    // Update is called once per frame
    void Update () {
        if (Time.time > changeSprite)
        {
            changeSprite = Time.time + playspeed;
            index++;
            if (index >= spriteSheet.Length) { index = 0; } // Restart animation
            GetComponent<SpriteRenderer>().sprite = spriteSheet[index];
        }
    }
}
