using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour {

    public Sprite laser;
    public Sprite gun;

    public Texture2D gunCursor;
    public Texture2D miningCursor;

    public AudioSource switchSound;



    // Use this for initialization
    void Start () {
        transform.Find("Hotbar_item 1").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        transform.Find("Hotbar_item 2").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }
	
	// Update is called once per frame
	void Update () {
	
        if(Input.GetKeyDown("1"))
        {
            Cursor.SetCursor(gunCursor, new Vector2(gunCursor.width / 2, gunCursor.height / 2), CursorMode.Auto);

            transform.Find("Hotbar_item 1").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            transform.Find("Hotbar_item 2").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            //GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponController>().isMiningLaser = false;
            switchSound.Play();
        }
        else if (Input.GetKeyDown("2"))
        {
            Cursor.SetCursor(miningCursor, new Vector2(miningCursor.width / 2, miningCursor.height / 2), CursorMode.Auto);

            transform.Find("Hotbar_item 1").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            transform.Find("Hotbar_item 2").GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            //GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponController>().isMiningLaser = true;
            switchSound.Play();
        }
    }

}
