using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ParticleEffect))]
[CanEditMultipleObjects]
public class ParticleEffectEditor : Editor
{
	private double changeSprite = 0;
	private int index = 0;
	private bool pause = false;
	private bool startAnimation = false;
	private double animationStartTime;

	public override void OnInspectorGUI()
	{
		var effect = serializedObject.targetObject as ParticleEffect;
		base.OnInspectorGUI();
		if (effect != null && effect.particleSprites != null && !startAnimation)
		{
			startAnimation = true;
			animationStartTime = EditorApplication.timeSinceStartup;
			EditorApplication.update += Animate;
		}
	}

	void OnDisable()
	{
		if(startAnimation)
		{
			startAnimation = false;
			EditorApplication.update -= Animate;
		}
	}

	/// <summary>
	/// Animate the sprite in the window.
	/// </summary>
	private void Animate()
	{
		var effect = serializedObject.targetObject as ParticleEffect;
		if (effect == null || effect.particleSprites == null)
		{
			return;
		}
		if (EditorApplication.timeSinceStartup > changeSprite + animationStartTime)
		{
			if(pause)
				pause = false;

			var sprite = effect.particleSprites.GetFrame(index);
			// Pause before repeating animation, if it is not suppose to loop normally.
			if (!pause)
			{
				index = (index + 1) % effect.particleSprites.frames.Length;
				if (index == 0 && !effect.particleSprites.looping)
				{
					pause = true;
					effect.GetComponentInChildren<SpriteRenderer>().sprite = null;
					changeSprite += 1.0f;
				}
				else
				{
					effect.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
					changeSprite += effect.particleSprites.playSpeed;
				}
			}				
		}
	}
}
