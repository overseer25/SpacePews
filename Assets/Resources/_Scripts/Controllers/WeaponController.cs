using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    public bool menuOpen = false;
    private ShipMount turret;

    [SerializeField]
    private ShipMountController mountController;

    private void Start()
    {
        turret = mountController.GetTurretMount();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !menuOpen)
        {
            if(turret.GetShipComponent() is WeaponComponent)
            {
                var weapon = turret.GetShipComponent() as WeaponComponent;
                if (Time.time > weapon.GetNextShotTime())
                {
                    weapon.Fire();
                    weapon.SetLastShot(Time.time);
                }
            }
            if(turret.GetShipComponent() is MiningComponent)
            {
                var miningLaser = turret.GetShipComponent() as MiningComponent;
                miningLaser.Fire();
            }
        }
        if (Input.GetMouseButtonUp(0) || menuOpen)
        {
            if(turret.GetShipComponent() is MiningComponent)
            {
                var miningLaser = turret.GetShipComponent() as MiningComponent;
                miningLaser.StopFire();
            }
        }
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
