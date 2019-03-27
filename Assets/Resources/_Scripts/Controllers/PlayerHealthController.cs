using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Handles the player health.
/// </summary>
public class PlayerHealthController : MonoBehaviour
{
	public RectTransform healthBar;
	public RectTransform healthBarBG;
	public TextMeshProUGUI counter;

	[Header("Audio")]
	public AudioClip damageSound;

	private int currentHealth;
	private PlayerActor actor;
	private float healthbarLength;
	private AudioSource audioSource;

	private const float REGEN_WAIT_TIME = 1.0f;
	private bool finishedWaiting;
	private bool regeneratingHealth;

	private void Start()
	{
		actor = GetComponent<PlayerActor>();
		if (actor == null)
			Debug.LogError("Failed to find player actor for " + this);
		currentHealth = actor.health;
		audioSource = GetComponent<AudioSource>();

		healthBar.sizeDelta = new Vector2((float)currentHealth / actor.health * healthbarLength, healthBar.sizeDelta.y);
		healthBarBG.sizeDelta = new Vector2(healthbarLength, healthBarBG.sizeDelta.y);
	}

	private void Update()
	{
		UpdateHealthbar();
		if (currentHealth < actor.health && actor.healthRegenAmount != 0)
			StartCoroutine(RegenHealth());
	}

	/// <summary>
	/// Damage the player's health by the given amount.
	/// </summary>
	/// <param name="amount">Insert negative values to reduce health, positive values to increase.</param>
	public void TakeDamage(int amount)
	{
		finishedWaiting = false;
		StopCoroutine(RegenHealth());

		if (currentHealth - amount < 0)
			currentHealth = 0;
		else
		{
			currentHealth -= amount;
			audioSource.PlayOneShot(damageSound);
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
		if(!regeneratingHealth)
		{
			regeneratingHealth = true;
			while(currentHealth < actor.health)
			{
				if(!finishedWaiting)
				{
					yield return new WaitForSeconds(REGEN_WAIT_TIME);
					finishedWaiting = true;
				}
				Heal(actor.healthRegenAmount);
				yield return new WaitForSeconds(actor.healthRegenSpeed);
			}
			regeneratingHealth = false;
			finishedWaiting = false;
			yield break;
		}
	}

	/// <summary>
	/// Update the player's health bar.
	/// </summary>
	public void UpdateHealthbar()
	{
		healthbarLength = actor.health;
		if (currentHealth > actor.health)
			ResetHealth();
		healthBar.sizeDelta = new Vector2((float)currentHealth / actor.health * healthbarLength, healthBar.sizeDelta.y);
		healthBarBG.sizeDelta = new Vector2(healthbarLength, healthBarBG.sizeDelta.y);

		counter.text = currentHealth.ToString();
	}

	/// <summary>
	/// Reset the player's health to full.
	/// </summary>
	public void ResetHealth()
	{
		currentHealth = actor.health;
	}

	/// <summary>
	/// Instantly kill the player.
	/// </summary>
	public void Kill()
	{
		currentHealth = 0;
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
