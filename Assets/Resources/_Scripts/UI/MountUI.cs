//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class MountUI : Inventory
//{
//    [SerializeField]
//    private ShipMountController mountController;
//    private ShipMount[] mounts;
//    private List<MountSlot> mountSlotsUI;

//    void Start()
//    {
//        mountSlotsUI = new List<MountSlot>();
//        audioSource = GetComponent<AudioSource>();
//        mounts = mountController.GetAllMounts();
//        int index = 0;
//        foreach (var mount in mounts)
//        {
//            var prefabName = mount.GetMountType().ToString() + mount.GetMountTier().ToString() + mount.GetMountClass().ToString();
//            var slot = Instantiate(Resources.Load("_Prefabs/UI/MountingSystem/" + prefabName), mount.transform.position, mount.transform.rotation, transform) as GameObject;
//            slot.GetComponent<MountSlot>().Initialize(mount, index++);
//            mountSlotsUI.Add(slot.GetComponent<MountSlot>());
//        }
//    }

//    /// <summary>
//    /// Called to toggle display to inventory UI.
//    /// </summary>
//    public override void Toggle()
//    {
//        if (!isOpen)
//        {
//            //audioSource.clip = openSound;
//            //audioSource.Play();
//            gameObject.GetComponent<Canvas>().enabled = true;
//            foreach (var mount in mountSlotsUI)
//                mount.gameObject.SetActive(true);
//            isOpen = true;
//        }
//        else
//        {
//            //audioSource.clip = closeSound;
//            //audioSource.Play();
//            gameObject.GetComponent<Canvas>().enabled = false;
//            foreach (var mount in mountSlotsUI)
//                mount.gameObject.SetActive(false);
//            isOpen = false;
//        }
//    }

//    /// <summary>
//    /// Populates the tooltip with the information of the item in the inventory slot at the given index
//    /// and displays the tooltip.
//    /// </summary>
//    /// <param name="index"></param>
//    public override void ShowHoverTooltip(int index)
//    {
//        if (mountSlotsUI[index].isEmpty)
//        {
//            infoScreen.Hide();
//        }
//        else
//        {
//            var item = mountSlotsUI[index].GetInventoryItem().item;

//            infoScreen.SetInfo(item, 0);
//            infoScreen.Show();
//        }
//    }

//}
