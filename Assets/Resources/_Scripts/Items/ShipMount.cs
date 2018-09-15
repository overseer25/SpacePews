using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMount : MonoBehaviour {

    [SerializeField]
    private ItemType mountType;
    [SerializeField]
    private ItemClass mountClass;
    [SerializeField]
    private ItemTier mountTier;

    // The direction to display the UI element of the ship mount.
    [Header("UI Element Attributes")]
    public float uiAngle;
    public float distanceFromShip;

    public ShipComponent startingComponent;
    private ShipComponent component;
    private bool isEmpty = false;

    /// <summary>
    /// Clear the mount.
    /// </summary>
    public void ClearMount()
    {
        if (component is WeaponComponent)
            (component as WeaponComponent).SetMounted(false);
        else
            component.SetMounted(false);
        Destroy(component.gameObject);
    }

    /// <summary>
    /// Set the component of the mount.
    /// </summary>
    /// <param name="component"></param>
    public void SetComponent(ShipComponent component)
    {
        if(this.component != null)
        {
            if (this.component is WeaponComponent)
                (this.component as WeaponComponent).SetMounted(false);
            else
                this.component.SetMounted(false);
            Destroy(this.component.gameObject);
        }

        this.component = Instantiate(component, transform.position, transform.rotation, transform) as ShipComponent;
        this.component.gameObject.SetActive(true);
        if (this.component is WeaponComponent)
            (this.component as WeaponComponent).SetMounted(true);
        else if (this.component is ThrusterComponent)
        {
            SendMessageUpwards("UpdateThrusterList");
            this.component.SetMounted(true);
        }
        else
            this.component.SetMounted(true);
    }

    /// <summary>
    /// Checks to see if the provided component can fit in this mount.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool IsComponentCompatible(ShipComponent component)
    {
        if (component == null)
            return false;
        return mountType == component.GetItemType() && mountTier == component.GetComponentTier() && mountClass == component.GetComponentClass();
    }

    /// <summary>
    /// Get the direction to face the UI element associated with this
    /// mount.
    /// </summary>
    /// <returns></returns>
    public float GetUIDirection()
    {
        return uiAngle;
    }

    /// <summary>
    /// Get the distance away from the ship to place the UI element associated
    /// with this mount.
    /// </summary>
    /// <returns></returns>
    public float GetDistanceFromShip()
    {
        return distanceFromShip;
    }

    /// <summary>
    /// Gets the type of this mount.
    /// </summary>
    /// <returns></returns>
    public ItemType GetMountType()
    {
        return mountType;
    }

    /// <summary>
    /// Gets the class of this mount.
    /// </summary>
    /// <returns></returns>
    public ItemClass GetMountClass()
    {
        return mountClass;
    }

    /// <summary>
    /// Gets the tier of this mount.
    /// </summary>
    /// <returns></returns>
    public ItemTier GetMountTier()
    {
        return mountTier;
    }

    /// <summary>
    /// Gets the component installed in this mount.
    /// </summary>
    /// <returns></returns>
    public ShipComponent GetShipComponent()
    {
        return component;
    }

    /// <summary>
    /// Check for if the current component on this mount is
    /// destroyed.
    /// </summary>
    /// <returns></returns>
    public bool IsComponentDestroyed()
    {
        return component.GetRemainingHealth() <= 0;
    }

    /// <summary>
    /// Checks to see if the mount is currently empty or not.
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty()
    {
        return isEmpty;
    }

    /// <summary>
    /// Toggles visibility of component. If component is invisible, then the mount is empty.
    /// </summary>
    /// <returns></returns>
    public void ToggleEmpty()
    {
        isEmpty = !isEmpty;
    }
}
