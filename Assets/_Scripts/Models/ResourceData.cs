using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data model for resources that can be mined from a <see cref="HarvestableObject"/>.
/// Data includes the item and its chance of spawning from the object.
/// </summary>
[Serializable]
public class ResourceData
{
    public Item item;
    public float chance;
}
