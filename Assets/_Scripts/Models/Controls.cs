using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serialized data for saving user control preferences.
/// </summary>
[Serializable]
public class Controls
{
    public KeyCode fire;
    public KeyCode forward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode inventory;
	public KeyCode ability;
    public readonly KeyCode submit = KeyCode.Return;
    public readonly KeyCode pause = KeyCode.Escape;
    public KeyCode cameraZoomIn;
    public KeyCode cameraZoomOut;
    public KeyCode suicide;

	public Controls()
	{
		fire = KeyCode.Mouse0;
		forward = KeyCode.W;
		left = KeyCode.A;
		right = KeyCode.D;
		inventory = KeyCode.Tab;
		ability = KeyCode.Space;
		cameraZoomIn = KeyCode.KeypadPlus;
		cameraZoomOut = KeyCode.KeypadMinus;
		suicide = KeyCode.Backspace;

		submit = KeyCode.Return;
		pause = KeyCode.Escape;
	}
}
