using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A loot table that can be attached to a game object to define the items that object may spawn upon destroying it.
/// </summary>
[CreateAssetMenu(menuName = "New Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField]
    public ResourceData[] lootList;
}
