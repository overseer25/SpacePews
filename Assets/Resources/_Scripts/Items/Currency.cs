using UnityEngine;

/// <summary>
/// A Currency object that the player collects to add money to their character. 
/// Currency objects can have varying quantities, and the sprite should reflect that.
/// </summary>
public class Currency : MonoBehaviour
{
    private const int FOLLOWSPEED = 50;
    private const float MAXDISTANCE = 10.0f;

    public long amount;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HoverTowardPlayer();
    }

    /// <summary>
    /// The logic to move toward the player.
    /// </summary>
    private void HoverTowardPlayer()
    {
        var player = PlayerUtils.GetClosestPlayer(gameObject);

        // Don't hover toward the player if the currency would overflow the player's currency value.
        if (player.GetComponent<PlayerController>().inventory.CheckCurrency(amount))
            return;

        var distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= MAXDISTANCE)
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, FOLLOWSPEED * Time.deltaTime);
    }

    /// <summary>
    /// Deals with object collisions.
    /// </summary>
    /// <param name="collider"></param>
    void OnTriggerEnter2D(Collider2D collider)
    {
        var obj = collider.GetComponentInParent<Rigidbody2D>();

        if (obj == null || !gameObject.activeInHierarchy)
            return;
        switch (obj.gameObject.tag)
        {
            case "Player":
                if(obj.GetComponent<PlayerController>().inventory.AddCurrency(amount))
                    gameObject.SetActive(false);
                break;
            default:
                return;
        }
    }
}
