using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    public bool menuOpen = false;
    [Header("Inventory")]
    public Inventory inventory;
    [Header("Ship")]
    public Ship ship;

    private ShipComponent turret;
    
    [SerializeField]
    private ShipMountController mountController;

    void Start()
    {
        turret = ship.turret.GetComponent<ShipComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change the item if necessary.
        // TODO: Implement it so that the turret updates with the selected component.
        var hotbarSlotItem = inventory.GetSelectedHotbarSlot().GetItem();
        turret = hotbarSlotItem as ShipComponent;

        if (Input.GetMouseButton(0) && !menuOpen && turret != null)
        {
            if (turret is WeaponComponent)
            {
                var weapon = turret as WeaponComponent;
                if (Time.time > weapon.GetNextShotTime())
                {
                    weapon.Fire();
                    weapon.SetLastShot(Time.time);
                }
            }
        }
        if (Input.GetMouseButtonUp(0) || menuOpen || turret == null)
        {
            if (turret is MiningComponent)
            {
                var miningLaser = turret as MiningComponent;
                miningLaser.StopFire();
            }
        }

        //if (Input.GetMouseButton(0) && !menuOpen)
        //{
        //    if(turret.GetShipComponent() is WeaponComponent)
        //    {
        //        var weapon = turret.GetShipComponent() as WeaponComponent;
        //        if (Time.time > weapon.GetNextShotTime())
        //        {
        //            weapon.Fire();
        //            weapon.SetLastShot(Time.time);
        //        }
        //    }
        //    if(turret.GetShipComponent() is MiningComponent)
        //    {
        //        var miningLaser = turret.GetShipComponent() as MiningComponent;
        //        miningLaser.Fire();
        //    }
        //}
        //if (Input.GetMouseButtonUp(0) || menuOpen)
        //{
        //    if(turret.GetShipComponent() is MiningComponent)
        //    {
        //        var miningLaser = turret.GetShipComponent() as MiningComponent;
        //        miningLaser.StopFire();
        //    }
        //}
    }

    /// <summary>
    /// Update the controller on the state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        dead = isDead;
    }
}
