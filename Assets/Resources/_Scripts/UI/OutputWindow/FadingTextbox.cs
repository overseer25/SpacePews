using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FadingTextbox : MonoBehaviour
{
	private TextMeshProUGUI textbox;

	private const float FADE_WAIT_TIME = 3.0f;
	private const float FADE_SPEED = 0.05f;
	private const float FADE_AMOUNT = 0.02f;
	private bool fading;
	private bool doneWaiting = false;

	private void Update()
	{
		if(!fading)
		{
			StartCoroutine(FadeText());
		}

		if(textbox.alpha == 0.0f)
		{
			Destroy(gameObject);
		}
	}

	private void OnDisable()
	{
		fading = false;
		StopAllCoroutines();
	}

	/// <summary>
	/// Set the text of the textbox and the color of the text.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="color"></param>
	public void Initialize(string text)
	{
		textbox = GetComponent<TextMeshProUGUI>();
		textbox.text = text;
	}

	/// <summary>
	/// Coroutine for fading the text once it has been displayed.
	/// </summary>
	/// <returns></returns>
	private IEnumerator FadeText()
	{
		fading = true;
		// Allow the text to display for an amount of time before beginning fade.
		if(!doneWaiting)
			yield return new WaitForSeconds(FADE_WAIT_TIME);
		fading = false;

		doneWaiting = true;
		while(textbox.alpha != 0.0f)
		{
			fading = true;
			textbox.alpha -= FADE_AMOUNT;
			yield return new WaitForSeconds(FADE_SPEED);
			fading = false;
		}

		yield break;
	}
}
