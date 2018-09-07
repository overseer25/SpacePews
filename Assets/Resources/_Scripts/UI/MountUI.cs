using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountUI : MonoBehaviour
{
    [SerializeField]
    private ShipMountController mountController;
    [SerializeField]
    private MountSlot mountSlot;

    private ShipMount[] mounts;
    private InventorySlot[] mountSlotsUI;

    void Start()
    {
        mounts = mountController.GetAllMounts();
        int index = 0;
        foreach (var mount in mounts)
        {
            var slot = Instantiate(mountSlot, mount.transform.position, mount.transform.rotation, transform);
            slot.Initialize(mount, index++);
        }
    }
    
}
