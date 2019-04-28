using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents an ability charge bar.
/// </summary>
public class ChargeBar : MonoBehaviour
{
	public GameObject bg, fg;
	private float length;
	private void Start()
	{
		length = bg.GetComponent<RectTransform>().sizeDelta.x;
	}

	/// <summary>
	/// Set the percentage of the background the foreground fills up.
	/// </summary>
	/// <param name="amount"></param>
	public void SetFillPercentage(float amount)
	{
		fg.GetComponent<RectTransform>().sizeDelta = new Vector2(length * amount, fg.GetComponent<RectTransform>().sizeDelta.y);
	}
}
