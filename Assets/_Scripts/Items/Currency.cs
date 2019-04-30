using UnityEngine;

/// <summary>
/// A Currency object that the player collects to add money to their character. 
/// Currency objects can have varying quantities, and the sprite should reflect that.
/// </summary>
public class Currency : MonoBehaviour
{
    private const int FOLLOWSPEED = 50;
    private const float MAXDISTANCE = 10.0f;

	public Sprite sprite;
    public long amount;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        gameObject.tag = "Currency";
    }

    private void Update()
    {
        HoverTowardPlayer();
    }

    /// <summary>
    /// Deactivate the currency object.
    /// </summary>
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Activate the currency object.
    /// </summary>
    public void Activate()
    {
        gameObject.SetActive(true);
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
	/// Copy the values of the incoming currency to this one.
	/// </summary>
	/// <param name="other"></param>
	public void Copy(Currency other)
	{
		amount = other.amount;
		sprite = other.sprite;
		GetComponent<SpriteRenderer>().sprite = sprite;
	}
}
