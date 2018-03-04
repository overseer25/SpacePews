using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public Sprite[] healthSprites;
    public GameObject explosion;

    public Image heartUI;

    private PlayerController player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    void Update()
    {
        // If killed
        if (player.health <= 0)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            heartUI.sprite = healthSprites[0];
            Destroy(gameObject);
        }
        else
        {
            heartUI.sprite = healthSprites[player.health];
        }

        
    }
}
