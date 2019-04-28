using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour {

    public SpriteAnimation thrusterAnim;
	public bool isActive;
	private Coroutine animate;

	/// <summary>
	/// Activate the thruster. This enables the animation.
	/// </summary>
	public void Activate()
	{
		thrusterAnim.ResetAnimation();
		isActive = true;
		animate = StartCoroutine(Animate());
	}

	public void Deactivate()
	{
		isActive = false;
		if(animate != null)
		{
			StopCoroutine(animate);
			animate = null;
		}
		GetComponent<SpriteRenderer>().sprite = null;
	}

	/// <summary>
	/// Animate the thruster.
	/// </summary>
	/// <returns></returns>
    public IEnumerator Animate()
	{
		var sprite = thrusterAnim.GetNextFrame();
		GetComponent<SpriteRenderer>().sprite = sprite;
		yield return new WaitForSeconds(thrusterAnim.playSpeed);
		animate = StartCoroutine(Animate());
	}
}
