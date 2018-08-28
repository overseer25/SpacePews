using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop_Menu : MonoBehaviour {

    public GameObject player;
    public Button repairButton;

	// Use this for initialization
	void Start () {

        player = GameObject.FindGameObjectWithTag("Player");

        // Create listeners for buttons
        repairButton.onClick.AddListener(RepairShip);
	}
	
	// Update is called once per frame
	void Update () {

        if(player != null)
        {
            repairButton.GetComponentInChildren<Text>().text = "Repair: $" + (player.GetComponent<PlayerController>().maxHealth - player.GetComponent<PlayerController>().GetHealth());
        }
        
	}

    /// <summary>
    /// The action of the repair button. Heals the player ship.
    /// </summary>
    void RepairShip()
    {
        player.GetComponent<PlayerController>().currency -= player.GetComponent<PlayerController>().maxHealth - player.GetComponent<PlayerController>().GetHealth();
    }
}
