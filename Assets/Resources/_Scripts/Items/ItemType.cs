using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Item = 0,
    Weapon,
    MiningLaser,
    Utility,
    Storage,
    Shield,
    Thruster,
    Upgrade
}

/// <summary>
/// Defines extension methods for the ItemType enumeration.
/// </summary>
public static class ItemTypeExtensions
{
    /// <summary>
    /// Returns a representation of the ItemType for display
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToDisplayString(this ItemType type)
    {
        switch(type)
        {
            case ItemType.MiningLaser:
                return "Mining Laser";
        }

        return type.ToString();
    }
}