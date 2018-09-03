using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMount : MonoBehaviour {

    [SerializeField]
    private ComponentType mountType;
    [SerializeField]
    private ItemClass mountClass;
    [SerializeField]
    private ItemTier mountTier;


    private ShipComponent component;
    private bool isEmpty = false;

    /// <summary>
    /// Initialization.
    /// </summary>
    void Start()
    {
        component = GetComponentInChildren<ShipComponent>();
        component.mounted = true;
        if (component.GetComponentType() != mountType)
            Debug.LogError("Component type not equal to mount type!");
        if (component.GetComponentClass() != mountClass)
            Debug.LogError("Component class not equal to mount class!");
        if (component.GetComponentTier() != mountTier)
            Debug.LogError("Component tier not equal to mount tier!");
    }

    /// <summary>
    /// Gets the type of this mount.
    /// </summary>
    /// <returns></returns>
    public ComponentType GetMountType()
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
