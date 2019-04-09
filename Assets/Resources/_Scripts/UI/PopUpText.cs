using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
	public AudioClip defaultSound;
	public TextMeshPro textMesh;

	private float fadeTime = 0.0f;
	private float fadeSpeed = 0.2f;
	private int delay = 0;
	private static System.Random random;

	private const float LIFT_SPEED = 30.0f;

	void Awake()
	{
		random = new System.Random();
	}

	/// <summary>
	/// Initializes the pop-up text by allowing the callee to specify text and fade time.
	/// </summary>
	/// <param name="target">The game object to spawn this pop up text on top of.</param>
	/// <param name="text">The text to display.</param>
	/// <param name="size">Size of the text.</param>
	/// <param name="color">Color of the text.</param>
	/// <param name="sound">Sound to play when spawned.</param>
	/// <param name="radius">Radius around the origin point that the pop up text can spawn</param>
	/// <param name="playDefaultSound">Play the default sound of the pop up text.</param>
	public void Initialize(GameObject target, string text, float size, Color color, AudioClip sound = null, float radius = 0.0f, bool playDefaultSound = true)
	{
		textMesh.color = color;
		textMesh.text = text;
		textMesh.alpha = 1.0f;
		textMesh.fontSize = size;

		// Set this once.
		float x;

		// Make the value negative half the time.
		if (random.NextDouble() > 0.5f)
			x = target.transform.position.x + (float)(random.NextDouble()) * radius * -1;
		else
			x = target.transform.position.x + (float)(random.NextDouble() * radius);


		transform.position = new Vector3(x, target.transform.position.y, target.transform.position.z);

		gameObject.SetActive(true);

		// If a sound was provided, play it.
		if (sound != null)
		{
			GetComponent<AudioSource>().PlayOneShot(sound);
		}
		else if (playDefaultSound)
		{
			GetComponent<AudioSource>().PlayOneShot(defaultSound);
		}
	}

	/// <summary>
	/// Deals with fading and disabling the text.
	/// </summary>
	void Update()
	{
		if (Time.time > fadeTime)
		{
			fadeTime = Time.deltaTime + fadeSpeed;

			if (delay == 10)
			{
				textMesh.alpha -= 0.02f; // Slowly fade the sprite

				// If the text is invisible, deactivate the PopUpText object.
				if (textMesh.alpha <= 0)
				{
					gameObject.SetActive(false);
				}
			}
			else { delay++; }

			// Move the sprite upward
			transform.position = Vector2.Lerp(transform.position, new Vector3(transform.position.x,
				transform.position.y + (0.2f), transform.position.z), Time.deltaTime * LIFT_SPEED); // Set this once

		}
	}

	/// <summary>
	/// Called when the object is set to active.
	/// </summary>
	void OnEnable()
	{
		fadeTime = 0.0f;
		delay = 0;

	}
}
