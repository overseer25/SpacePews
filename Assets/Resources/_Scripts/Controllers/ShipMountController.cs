using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipMountController : MonoBehaviour {

    // Each mount on the ship.
    private ShipMount[] mounts;

    void Start()
    {
        mounts = GetComponentsInChildren<ShipMount>();
    }

    /// <summary>
    /// Gets all the weapon mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetWeaponMounts()
    {
        return mounts.Where(m => m.GetMountType() == ComponentType.Weapon).ToArray();
    }

    /// <summary>
    /// Gets all the utility mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetUtilityMounts()
    {
        return mounts.Where(m => m.GetMountType() == ComponentType.Utility).ToArray();
    }

    /// <summary>
    /// Gets all the Thruster mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetThrusterMounts()
    {
        return mounts.Where(m => m.GetMountType() == ComponentType.Thruster).ToArray();
    }

    /// <summary>
    /// Gets all the Storage mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetStorageMounts()
    {
        return mounts.Where(m => m.GetMountType() == ComponentType.Storage).ToArray();
    }

    /// <summary>
    /// Gets all the Shield mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetShieldMounts()
    {
        return mounts.Where(m => m.GetMountType() == ComponentType.Shield).ToArray();
    }

    /// <summary>
    /// Gets all the Upgrade mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetUpgradeMounts()
    {
        return mounts.Where(m => m.GetMountType() == ComponentType.Upgrade).ToArray();
    }
}
