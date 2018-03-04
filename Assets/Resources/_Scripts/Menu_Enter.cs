using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu_Enter : MonoBehaviour {

    public Canvas shopHUD;
    public Canvas inventoryHUD;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown("q") && GetComponent<CheckCollision>().incomingCollision.tag == "Shop" 
            && !inventoryHUD.enabled)
        {
            shopHUD.enabled = !shopHUD.enabled;
            Time.timeScale = (Time.timeScale == 1.0f) ? 0.0f : 1.0f;
        }
        else if(Input.GetKeyDown("i") && !shopHUD.enabled)
        {
            inventoryHUD.enabled = !inventoryHUD.enabled;
            inventoryHUD.transform.Find("Currency_UI").GetComponentInChildren<Text>().text = "Currency: $" + GetComponent<PlayerController>().currency;
        }
        shopHUD.transform.Find("Currency_UI").GetComponentInChildren<Text>().text = "Currency: $" + GetComponent<PlayerController>().currency;

    }
}
