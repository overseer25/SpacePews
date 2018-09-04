using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountUI : MonoBehaviour
{
    [SerializeField]
    private ShipMountController mountController;

    private ShipMount[] mounts;
    private InventorySlot[] mountSlotsUI;

    void Update()
    {
        mounts = mountController.GetAllMounts();
        
    }

    void Start()
    {

    }
    
}
