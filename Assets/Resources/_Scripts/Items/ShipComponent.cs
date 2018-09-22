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
    // Whether or not the component is visible on the ship.
    [SerializeField]
    private bool visible = true;

    // Is this item mounted to the ship?
    internal bool mounted = false;

    // The type of component.
    internal ItemType itemType;

    // The amount of health the component currently has.
    internal int currentHealth;

    /// <summary>
    /// Set the mounted status of the component.
    /// </summary>
    /// <param name="val"></param>
    public virtual void SetMounted(bool val)
    {
        mounted = val;
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
    /// Is the component visible on the ship or not?
    /// </summary>
    /// <returns></returns>
    public bool IsVisible()
    {
        return visible;
    }

    /// <summary>
    /// Gets the type of this component.
    /// </summary>
    /// <returns></returns>
    public ItemType GetItemType()
    {
        return itemType;
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

    /// <summary>
    /// Checks that this ship component is the same as another, comparing
    /// their type, tier, and class.
    /// </summary>
    /// <returns></returns>
    public bool IsSameComponentType(ShipComponent other)
    {
        return itemType == other.GetItemType() && itemTier == other.GetComponentTier() && componentClass == other.GetComponentClass();
    }
}
