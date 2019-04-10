using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Item types in the game. Used by inventory system to determine what items can go into what slots.
/// </summary>
public enum ItemType
{
    Item = 0,
    Turret,
    Utility,
    Storage,
    Shield,
    Thruster,
    Upgrade
}