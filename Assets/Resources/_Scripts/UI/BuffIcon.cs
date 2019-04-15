using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
	[HideInInspector]
	public Buff buff;
	[HideInInspector]
	public Image icon;
	[HideInInspector]
	public TextMeshProUGUI timer;

	public void Initialize(Buff buff)
	{
		icon = GetComponent<Image>();
		timer = GetComponentInChildren<TextMeshProUGUI>();
		this.buff = buff;
		icon.sprite = buff.icon;
		if (buff.timeInSeconds > 0)
		{
			StartCoroutine(BeginCountdown(buff.timeInSeconds));
		}
	}

	/// <summary>
	/// Wait for time. Every second, update the time display underneath the given index.
	/// </summary>
	/// <param name="index"></param>
	/// <param name="time"></param>
	/// <returns></returns>
	private IEnumerator BeginCountdown(int time)
	{
		int remainingTime = time;
		timer.enabled = true;
		while (remainingTime > 0)
		{
			timer.text = GetTimeString(remainingTime);
			yield return new WaitForSeconds(1);
			remainingTime--;
		}

		BuffManager.current.RemoveBuff(buff);
	}

	/// <summary>
	/// Get time string for a given integer time.
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	private string GetTimeString(int time)
	{
		int hours = time / 3600;
		int remaining = (time - 3600 * hours);
		int minutes = remaining / 60;
		int seconds = remaining - (60 * minutes);

		string result = "";
		if (hours > 0)
			result += hours.ToString("##") + "h ";
		if (minutes > 0)
			result += minutes.ToString("##") + "m";
		if (hours == 0 && minutes == 0 && seconds > 0)
			result += seconds.ToString("##") + "s";

		return result;
	}

	void OnMouseOver()
	{
		if (icon.enabled)
		{
			InfoScreen.current.SetInfo(buff);
			InfoScreen.current.Show();
		}
	}

	void OnMouseExit()
	{
		InfoScreen.current.Hide();
	}
}
