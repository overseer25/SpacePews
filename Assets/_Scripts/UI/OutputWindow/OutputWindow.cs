using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays information such as item collection and world events.
/// </summary>
public class OutputWindow : MonoBehaviour
{
	/// <summary>
	/// This text element is created and destroyed as new lines are needed.
	/// </summary>
	public FadingTextbox fadingText;

	/// <summary>
	/// The gameobject where the text elements are displayed.
	/// </summary>
	public GameObject textArea;

	/// <summary>
	/// The instance of the Output window.
	/// </summary>
	[HideInInspector]
	public static OutputWindow current;

	[HideInInspector]
	public bool isOpen;

	// The max number of text boxes that can exist.
	private const int MAX_NUMBER_OF_TEXT_BOXES = 15;

	private void Start()
	{
		if (current == null)
			current = this;
		else
			Debug.Log("An output window already exists.");
	}

	/// <summary>
	/// Display the Output Window.
	/// </summary>
	public void Show()
	{
		for (int i = 0; i < this.transform.childCount; i++)
		{
			this.transform.GetChild(i).gameObject.SetActive(true);
		}
		isOpen = true;
	}

	/// <summary>
	/// Hide the Output Window.
	/// </summary>
	public void Hide()
	{
		for (int i = 0; i < this.transform.childCount; i++)
		{
			this.transform.GetChild(i).gameObject.SetActive(false);
		}
		isOpen = false;
	}

	/// <summary>
	/// Displays a new bit of text in the menu.
	/// </summary>
	public void DisplayText(string text)
	{
		if (current == null)
		{
			Debug.Log("Trying to display text to output window, but output window is null.");
			return;
		}

		var children = textArea.GetComponentsInChildren<FadingTextbox>(true);
		if (children.Length > MAX_NUMBER_OF_TEXT_BOXES)
			return;
		else if(children.Length == MAX_NUMBER_OF_TEXT_BOXES)
		{
			Destroy(children[0].gameObject);
		}

		var textbox = Instantiate(fadingText, textArea.transform);
		textbox.GetComponent<FadingTextbox>().Initialize(text);
	}
}
