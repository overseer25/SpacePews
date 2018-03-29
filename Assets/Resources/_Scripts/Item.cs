using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {


    public Sprite[] spriteAnim; // For animation
    public Sprite sprite; // For no animation
    public SpriteRenderer hoverTextSprite; // When the mouse hovers over the object, display this text.
    public GameObject collectSprite;
    public new string name;
    public int quantity = 1;
    public int value;
    public string description;
    public string type;
    private float changeSprite = 0.0f;
    public float playspeed = 0.5f;
    private int index = 0;

    // Variables for bobbing
    private bool bobbingUp = true; // Starts with bobbing upward
    private float bobSpeed = 0.005f;
    private float changeDirection = 0.0f;
    private float bobAmount = 0.05f;
    private int counter = 0;

    // Creates the object to play the collection sprite, and destroys the item.
    public void CreateCollectItemSprite()
    {
        Instantiate(collectSprite, transform.position, Quaternion.identity);
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
        // If no sprite animation, it will just bob up and down.
        if(Time.time > changeDirection)
        {
            changeDirection = Time.time + bobAmount;
            // Change direction after 5 iterations
            if(counter > 15)
            {
                bobSpeed = -bobSpeed;
                counter = 0;
            }
            transform.position = new Vector3(transform.position.x, transform.position.y + bobSpeed, transform.position.z);
            counter++;
        }
    }

    void OnMouseOver()
    {

        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y + 20.0f, 0));
        mouse.z = 0;

        hoverTextSprite.enabled = true;
        hoverTextSprite.transform.position = mouse;
    }

    // Remove highlight on image when no longer hovering
    void OnMouseExit()
    {
        hoverTextSprite.enabled = false;
    }
}
