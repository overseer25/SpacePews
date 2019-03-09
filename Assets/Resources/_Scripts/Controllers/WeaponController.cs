﻿using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    [HideInInspector]
    public bool menuOpen = false;
    [Header("Inventory")]
    public Inventory inventory;
    private Ship ship;

    private GameObject turret;
    [HideInInspector]
    public ShipComponent currentComponent;

    [SerializeField]
    private ShipMountController mountController;

    void Start()
    {
        ship = GetComponentInChildren<Ship>();
        turret = ship.turret;
    }

    /// <summary>
    /// Update the turret to use the provided component.
    /// </summary>
    /// <param name="component"></param>
    public void UpdateTurret(ShipComponent component)
    {
        if (!dead)
        {
            if (currentComponent != null)
            {
                if (currentComponent is WeaponComponent)
                    (currentComponent as WeaponComponent).SetMounted(false);
                else if (currentComponent is MiningComponent)
                    (currentComponent as MiningComponent).SetMounted(false);
                Destroy(currentComponent.gameObject);
            }
            if (component == null)
            {
                return;
            }
            var hotbarSlotItem = inventory.GetSelectedHotbarSlot().GetItem();
            if (hotbarSlotItem != null)
            {
                currentComponent = Instantiate(hotbarSlotItem, turret.transform.position, turret.transform.rotation, turret.transform) as ShipComponent;
                currentComponent.gameObject.SetActive(true);
                if (currentComponent is WeaponComponent)
                    (currentComponent as WeaponComponent).SetMounted(true);
                else if (currentComponent is MiningComponent)
                    (currentComponent as MiningComponent).SetMounted(true);
            }
        }
    }

    /// <summary>
    /// Update the controller on the state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        if (!dead && isDead)
        {
            dead = isDead;
            turret.gameObject.SetActive(false);
            menuOpen = false;
        }
        else if (dead && !isDead)
        {
            dead = isDead;
            turret.gameObject.SetActive(true);
        }

    }
}
