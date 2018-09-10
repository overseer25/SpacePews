using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    public bool menuOpen = false;

    [SerializeField]
    private ShipMountController mountController;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && !menuOpen)
        {
            foreach (var mount in mountController.GetWeaponMounts())
            {
                var weapon = mount.GetShipComponent() as WeaponComponent;
                if(weapon != null)
                {
                    if (Time.time > weapon.GetNextShotTime())
                    {
                        weapon.Fire();
                        weapon.SetLastShot(Time.time);
                    }
                }
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
