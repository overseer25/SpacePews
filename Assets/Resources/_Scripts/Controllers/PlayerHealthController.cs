using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the player health.
/// </summary>
public class PlayerHealthController : MonoBehaviour
{
	public RectTransform healthBar;
	public RectTransform healthBarBG;
	public TextMeshProUGUI counter;

	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip damageSound;

	private int currentHealth;
	private PlayerActor actor;
	private float healthbarLength;

	private const float REGEN_WAIT_TIME = 3.0f;
	private bool regeneratingHealth;

	private Coroutine regenerationCoroutine;

	private void Start()
	{
		actor = GetComponent<PlayerActor>();
		if (actor == null)
			Debug.LogError("Failed to find player actor for " + this);
		currentHealth = actor.health;

		healthBar.sizeDelta = new Vector2((float)currentHealth / actor.health * healthbarLength, healthBar.sizeDelta.y);
		healthBarBG.sizeDelta = new Vector2(healthbarLength, healthBarBG.sizeDelta.y);
	}

	private void Update()
	{
		UpdateHealthbar();
		if (currentHealth < actor.health && !regeneratingHealth && !actor.disableHealthRegen && !IsDead())
		{
			regenerationCoroutine = StartCoroutine("RegenHealth");
		}
		else if (regeneratingHealth && actor.disableHealthRegen)
		{
			regeneratingHealth = false;
			StopCoroutine(regenerationCoroutine);
		}
	}

	/// <summary>
	/// Damage the player's health by the given amount.
	/// </summary>
	/// <param name="amount">Insert negative values to reduce health, positive values to increase.</param>
	public void TakeDamage(int amount)
	{
		if (regenerationCoroutine != null)
		{
			StopCoroutine(regenerationCoroutine);
			regeneratingHealth = false;
		}

		if (currentHealth - amount <= 0)
			currentHealth = 0;
		else
		{
			currentHealth -= amount;
			audioSource.PlayOneShot(damageSound);
		}

		if (currentHealth < actor.health && actor.healthRegenAmount > 0 && !actor.disableHealthRegen && !IsDead())
		{
			regenerationCoroutine = StartCoroutine("RegenHealth");
		}
	}

	/// <summary>
	/// Heal the player by the given amount;
	/// </summary>
	/// <param name="amount"></param>
	public void Heal(int amount)
	{
		if (currentHealth + amount > actor.health)
			currentHealth = actor.health;
		else
			currentHealth += amount;
	}

	/// <summary>
	/// Coroutine for regenerating health.
	/// </summary>
	/// <returns></returns>
	public IEnumerator RegenHealth()
	{
		regeneratingHealth = true;
		yield return new WaitForSeconds(REGEN_WAIT_TIME);

		while (currentHealth < actor.health)
		{
			Heal(actor.healthRegenAmount);
			float waitTime = (actor.healthRegenSpeed < 0) ? 0 : actor.healthRegenSpeed;
			yield return new WaitForSeconds(waitTime);
		}
		regeneratingHealth = false;
		yield break;
	}

	/// <summary>
	/// Update the player's health bar.
	/// </summary>
	public void UpdateHealthbar()
	{
		healthbarLength = actor.health;
		if (currentHealth > actor.health)
			ResetHealth();
		healthBar.sizeDelta = Vector2.Lerp(healthBar.sizeDelta, new Vector2((float)currentHealth / actor.health * healthbarLength, healthBar.sizeDelta.y), 10.0f * Time.deltaTime);
		healthBarBG.sizeDelta = Vector2.Lerp(healthBarBG.sizeDelta, new Vector2(healthbarLength, healthBarBG.sizeDelta.y), 10.0f * Time.deltaTime);
		counter.text = currentHealth.ToString();
	}

	/// <summary>
	/// Reset the player's health to full.
	/// </summary>
	public void ResetHealth(int? amount = null)
	{
		if (regenerationCoroutine != null)
		{
			StopCoroutine(regenerationCoroutine);
			regeneratingHealth = false;
		}
		if (amount == null)
			currentHealth = actor.health;
		else
			currentHealth = (int)amount;
	}

	/// <summary>
	/// Instantly kill the player.
	/// </summary>
	public void Kill()
	{
		TakeDamage(currentHealth);
	}

	/// <summary>
	/// Is the player dead?
	/// </summary>
	/// <returns></returns>
	public bool IsDead()
	{
		return currentHealth <= 0;
	}
}
