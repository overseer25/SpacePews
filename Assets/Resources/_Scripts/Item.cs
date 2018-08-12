﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {


    public Sprite[] spriteAnim; // For animation
    public Sprite sprite; // For no animation
    public GameObject collectSprite;
    public GameObject hoverText;
    public GameObject collectedText; // text displayed when the object is collected.
    public new string name;
    public int quantity = 1;
    public int value;
    public string description;
    public string type;
    public float playspeed = 0.5f;

    private float changeSprite = 0.0f;
    private int index = 0;
    private int followSpeed = 5;
    private const float MAXDISTANCE = 1.0f;

    // If the item has been dropped from player inventory. If it has been, then it must wait a certain amount of time before hovering toward the player that dropped it.
    private bool dropped = false;
    // The player who dropped the item.
    private GameObject dropper;
    private float waitTime = 5.0f;
    private float timer = 0.0f;

    /// <summary>
    /// Instantiate the object with additional parameters. Used when the item is dropped from an inventory.
    /// </summary>
    /// <param name="dropped"></param>
    /// <param name="dropper"></param>
    public void Initialize(bool dropped, GameObject dropper)
    {
        this.dropped = dropped;
        this.dropper = dropper;
    }

    // Creates the object to play the collection sprite, and destroys the item.
    public void CreateCollectItemSprite()
    {
        Instantiate(collectSprite, transform.position, Quaternion.identity);
        var collected = Instantiate(collectedText, transform.position, Quaternion.identity);
        collected.GetComponent<PopUpText>().Initialize(PlayerUtils.GetClosestPlayer(gameObject), name);
        Destroy(gameObject);
    }
	
	// Update is called once per frame
	void Update () {

        if(spriteAnim.Length > 0)
        {
            // Player sprite animation
            if (Time.time > changeSprite)
            {
                changeSprite = Time.time + playspeed;
                index++;
                if (index >= spriteAnim.Length) { index = 0; } // Restart animation
                GetComponentInChildren<SpriteRenderer>().sprite = spriteAnim[index];
            }
        }

        HoverTowardPlayer();
    }

    /// <summary>
    /// The logic to move toward the player.
    /// </summary>
    void HoverTowardPlayer()
    {
        var closestPlayer = PlayerUtils.GetClosestPlayer(gameObject);
        float distanceToPlayer = Vector2.Distance(transform.position, closestPlayer.transform.position);
        waitTime += Time.time;
        
        if (distanceToPlayer <= MAXDISTANCE)
        {
            if (dropped)
            {
                if(Time.time >= waitTime)
                    transform.position = Vector2.MoveTowards(transform.position, closestPlayer.transform.position, followSpeed * Time.deltaTime);
            }
            else
                transform.position = Vector2.MoveTowards(transform.position, closestPlayer.transform.position, followSpeed * Time.deltaTime);
        }
    }

    void OnMouseOver()
    {
        hoverText.SetActive(true);
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        hoverText.SetActive(false);
    }
}
