using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Controls
{
    public KeyCode fire;
    public KeyCode forward;
    public KeyCode left;
    public KeyCode right;
    public KeyCode inventory;
    public KeyCode submit;
    public readonly KeyCode pause = KeyCode.Escape;
    public KeyCode cameraZoomIn;
    public KeyCode cameraZoomOut;
    public KeyCode suicide;
}
