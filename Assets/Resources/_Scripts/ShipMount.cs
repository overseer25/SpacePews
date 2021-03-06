﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShipMount : MonoBehaviour {

    [SerializeField]
    private ItemType mountType;
    [SerializeField]
    private ItemClass mountClass;
    [SerializeField]
    private ItemTier mountTier;
    [SerializeField]
    private bool isGimbal;

    // The direction to display the UI element of the ship mount.
    [Header("UI Element Attributes")]
    public float uiAngle;
    public float distanceFromShip;

    public ShipComponentBase startingComponent;
    private ShipComponentBase component;
    private bool isEmpty = false;
	private Actor parentActor;
	private PlayerController pController;

    void Start()
    {
        if(isGimbal)
        {
            transform.GetComponent<SortingGroup>().sortingLayerName = "Ship";
            transform.GetComponent<SortingGroup>().sortingOrder = 1;
        }
		parentActor = GetComponentInParent<Actor>();
		pController = GetComponentInParent<PlayerController>();
		if (parentActor == null)
			Debug.LogError("Failed to find parent actor of " + this);
        
    }

    void Update()
    {
        if(isGimbal)
        {
            var pos = Input.mousePosition;
            pos.z = transform.position.z - Camera.main.transform.position.z;
            pos = Camera.main.ScreenToWorldPoint(pos);

            transform.rotation = Quaternion.FromToRotation(Vector3.up, pos - transform.position);
        }
    }

    /// <summary>
    /// Is the mount a gimbal mount?
    /// </summary>
    /// <returns></returns>
    public bool IsGimbal()
    {
        return isGimbal;
    }

    /// <summary>
    /// Clear the mount.
    /// </summary>
    public void ClearMount()
    {
		if (component is ThrusterComponent)
			(component as ThrusterComponent).SetMounted(false);
		if (component is StatBuffUpgradeComponent)
			(component as StatBuffUpgradeComponent).SetMounted(parentActor, false);
		else if (component is ActiveAbilityComponent)
			(component as ActiveAbilityComponent).SetMounted(pController, false);
		else
			component.SetMounted(false);
        Destroy(component.gameObject);
        isEmpty = true;
    }

    /// <summary>
    /// Set the component of the mount.
    /// </summary>
    /// <param name="incomingComponent"></param>
    public void SetComponent(ShipComponentBase incomingComponent)
    {
        // If the starting component is not compatible with the ship mount.
        if (incomingComponent != null && !IsComponentCompatible(incomingComponent))
        {
            Debug.LogError("Component: " + incomingComponent.itemName + " is not compatible with mount: " + this.name);
            return;
        }

		// Destroy the old component.
        if (component != null)
        {
			if (component is StatBuffUpgradeComponent)
				(component as StatBuffUpgradeComponent).SetMounted(parentActor, false);
			else if (component is ActiveAbilityComponent)
				(component as ActiveAbilityComponent).SetMounted(pController, false);
			else
				component.SetMounted(false);
			Destroy(component.gameObject);
        }

        component = Instantiate(incomingComponent, transform.position, transform.rotation, transform) as ShipComponentBase;
        component.gameObject.SetActive(incomingComponent.IsVisible());

		if (component is ThrusterComponent)
		{
			SendMessageUpwards("UpdateThrusterList");
			component.SetMounted(true);
		}
		else if (component is StatBuffUpgradeComponent)
			(component as StatBuffUpgradeComponent).SetMounted(parentActor, true);
		else if (component is ActiveAbilityComponent)
			(component as ActiveAbilityComponent).SetMounted(pController, true);
		else
			component.SetMounted(true);

		isEmpty = false;
    }

    /// <summary>
    /// Checks to see if the provided component can fit in this mount.
    /// </summary>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool IsComponentCompatible(ShipComponentBase component)
    {
        if (component == null)
            return false;
        return mountType == component.GetItemType() && mountTier >= component.GetComponentTier();
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
    public ShipComponentBase GetShipComponent()
    {
        return component;
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
