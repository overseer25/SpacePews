using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour {


    public AudioSource hit;
    public AudioSource collisionSound;
    public Collider2D incomingCollision;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void OnTriggerEnter2D(Collider2D collision)
    {
        incomingCollision = collision;

        // If enemy hits player
        if (collision.gameObject.tag == "EnemyProjectile" && gameObject.tag == "Player" && GetComponent<PlayerController>().health > 0)
        {
            GetComponent<PlayerController>().health--;
            hit.Play();
        }

        // If player hits enemy
        if (collision.gameObject.tag == "PlayerProjectile" && gameObject.tag == "Enemy" && GetComponent<EnemyScript_Redwing>().health > 0)
        {
            GetComponent<EnemyScript_Redwing>().health--;
            hit.Play();
        }

        // If player entering shop
        if(collision.gameObject.tag == "Player" && gameObject.tag == "Shop")
        {
            gameObject.transform.Find("Shop_enter_text").GetComponent<SpriteRenderer>().enabled = true;
        }
        // If player collecting coin
        if(collision.gameObject.tag == "Item" && gameObject.tag == "Player")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            Inventory inventory = GameObject.Find("InventoryHUD").GetComponent<Inventory>();
            // Only add the item to the player's inventory list if there is an empty slot for it, or a slot contains the item already.
            if (inventory.IsEmptySlot() || inventory.ContainsItem(item))
            {
                GameObject.Find("InventoryHUD").GetComponent<Inventory>().AddItem(item); // Add the item to the player inventory.
                collision.gameObject.GetComponent<Item>().CreateCollectItemSprite();
            }
            
        }
        // If player collecting coin
        if (collision.gameObject.tag == "Upgrade" && gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Upgrade>().ApplyUpgrade();
            Destroy(collision.gameObject);
        }

        // If colliding with asteroid or other immovable object
        if ((collision.gameObject.tag == "Immovable" || collision.gameObject.tag == "Asteroid" || collision.gameObject.tag == "Mineable") 
            && (gameObject.tag == "Player" || gameObject.tag == "Enemy"))
        {
            Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
            Vector2 direction = Vector2.zero;
            // Direction of the collision, with the magnitude applied being the speed of the player.
            // If the player isn't moving, we need to move them away, as they would be able to clip into the immovable otherwise.
            if (rigidBody.velocity == Vector2.zero)
            {
                direction = (gameObject.transform.position - collision.gameObject.transform.position) * 20;
            }
            // The difference here is the player has a velocity, so we will propel them away from the immovable using their velocity magnitude.
            else
            {
                // If the entity collides with the object at a certain velocity, take one health away.
                if(rigidBody.velocity.magnitude >= 3 && rigidBody.velocity.magnitude < 5)
                {
                    gameObject.GetComponent<PlayerController>().health--;
                    collisionSound.Play();
                }
                else if(rigidBody.velocity.magnitude >= 5 && rigidBody.velocity.magnitude < 10)
                {

                    gameObject.GetComponent<PlayerController>().health -= 2;
                    collisionSound.Play();
                }
                else if(rigidBody.velocity.magnitude >= 10)
                {
                    gameObject.GetComponent<PlayerController>().health -= 3;
                    collisionSound.Play();
                }
                direction = (gameObject.transform.position - collision.gameObject.transform.position).normalized * (rigidBody.velocity.magnitude * 100);
            }
            rigidBody.AddForce(direction);
        }

    }

    void OnTriggerExit2D(Collider2D collision)
    {
        incomingCollision = null;

        if (collision.gameObject.tag == "Player" && gameObject.tag == "Shop")
        {
            gameObject.transform.Find("Shop_enter_text").GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
