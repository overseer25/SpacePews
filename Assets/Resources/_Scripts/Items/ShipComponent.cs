using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponent : Item
{
    [Header("Component")]
    [SerializeField]
    private ItemClass componentClass;
    [SerializeField]
    private int health;
    // Is this item mounted to the ship?
    internal bool mounted = false;

    // The type of component.
    internal ItemType ItemType;

    // The amount of health the component currently has.
    internal int currentHealth;

    /// <summary>
    /// Initialization.
    /// </summary>
    void Awake()
    {
        currentHealth = health;
    }

    /// <summary>
    /// The logic to move toward the player.
    /// </summary>
    internal override void HoverTowardPlayer()
    {
        if (!mounted)
            base.HoverTowardPlayer();
    }

    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    /// <returns></returns>
    public ItemType GetItemType()
    {
        return ItemType;
    }

    /// <summary>
    /// Gets the class of this component.
    /// </summary>
    /// <returns></returns>
    public ItemClass GetComponentClass()
    {
        return componentClass;
    }

    /// <summary>
    /// Gets the tier of this component.
    /// </summary>
    /// <returns></returns>
    public ItemTier GetComponentTier()
    {
        return itemTier;
    }

    /// <summary>
    /// Gets the current health of this component.
    /// </summary>
    /// <returns></returns>
    public int GetRemainingHealth()
    {
        return currentHealth;
    }
}
