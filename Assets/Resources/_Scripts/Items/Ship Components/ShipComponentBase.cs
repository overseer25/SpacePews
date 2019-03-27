using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all ship components.
/// </summary>
public abstract class ShipComponentBase : Item
{
    public ItemClass componentClass;
    // Whether or not the component is visible on the ship.
    public bool visible = true;

    // Is this item mounted to the ship?
    public bool mounted = false;

    protected override void Awake()
    {
        base.Awake();
        stackable = false;
        stackSize = 0;
    }

    /// <summary>
    /// Set the mounted status of the component.
    /// </summary>
    /// <param name="val"></param>
    public virtual void SetMounted(bool val)
    {
        mounted = val;
        if (mounted && visible)
            spriteRenderer.enabled = true;
    }

    /// <summary>
    /// The logic to move toward the player.
    /// </summary>
    internal override void HoverTowardPlayer(GameObject player)
    {
        if (!mounted)
            base.HoverTowardPlayer(player);
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
    /// Checks that this ship component is the same as another, comparing
    /// their type, tier, and class.
    /// </summary>
    /// <returns></returns>
    public bool IsSameComponentType(ShipComponentBase other)
    {
        return itemType == other.GetItemType() && itemTier == other.GetComponentTier() && componentClass == other.GetComponentClass();
    }
}
