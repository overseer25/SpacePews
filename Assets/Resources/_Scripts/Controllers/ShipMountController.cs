﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipMountController : MonoBehaviour {

    // Each mount on the ship.
    private ShipMount[] mounts;

    void Awake()
    {
        mounts = GetComponentsInChildren<ShipMount>();
    }

    /// <summary>
    /// Gets all the mounts on the ship.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetAllMounts()
    {
        return mounts;
    }

    public ShipMount GetTurretMount()
    {
        return mounts.Where(m => m.GetMountType() == ItemType.Turret).First();
    }

    /// <summary>
    /// Gets all the utility mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetUtilityMounts()
    {
        return mounts.Where(m => m.GetMountType() == ItemType.Utility && m.GetShipComponent() != null).ToArray();
    }

    /// <summary>
    /// Gets all the Thruster mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetThrusterMounts()
    {
        return mounts.Where(m => m.GetMountType() == ItemType.Thruster && m.GetShipComponent() != null).ToArray();
    }

    /// <summary>
    /// Gets all the Storage mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetStorageMounts()
    {
        return mounts.Where(m => m.GetMountType() == ItemType.Storage && m.GetShipComponent() != null).ToArray();
    }

    /// <summary>
    /// Gets all the Shield mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetShieldMounts()
    {
        return mounts.Where(m => m.GetMountType() == ItemType.Shield).ToArray();
    }

    /// <summary>
    /// Gets all the Upgrade mounts being managed.
    /// </summary>
    /// <returns></returns>
    public ShipMount[] GetUpgradeMounts()
    {
        return mounts.Where(m => m.GetMountType() == ItemType.Upgrade).ToArray();
    }
}
