using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopUpText : MonoBehaviour
{
	public AudioClip defaultSound;
	public TextMeshPro textMesh;

    private float timeUntilFade = 0.5f;
	private static System.Random random;

    // Rate at which the text lifts upward.
	private const float LIFT_SPEED = 0.5f;
    // Number of times to wait until fading.
    private const int WAIT_COUNT = 50;

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
        StopCoroutine(Fade());

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

        StartCoroutine(Fade());
	}

    /// <summary>
    /// Lifts the text upward and fades over time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Fade()
    {
        bool beginFade = false;
        int count = 0;

        while(true)
        {
            if (count == WAIT_COUNT)
                beginFade = true;

            // Move the sprite upward
            transform.position = Vector2.Lerp(transform.position, new Vector3(transform.position.x,
                transform.position.y + (0.2f), transform.position.z), LIFT_SPEED); // Set this once
            count++;

            if(beginFade)
            {
                textMesh.alpha -= 0.05f; // Slowly fade the sprite
                // If the text is invisible, deactivate the PopUpText object.
                if (textMesh.alpha <= 0.0f)
                {
                    gameObject.SetActive(false);
                    break;
                }
            }
            yield return new WaitForSeconds(timeUntilFade / WAIT_COUNT);
        }

        yield return null;
    }
}
