using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MountUI : MonoBehaviour
{
    [SerializeField]
    private ShipMountController mountController;
    [SerializeField]
    // The mount slot prefab.
    private MountSlot mountSlot;
    [Header("Sounds")]
    [SerializeField]
    private AudioClip mountUIOpen;
    [SerializeField]
    private AudioClip mountUIClose;

    private ShipMount[] mounts;
    private List<MountSlot> mountSlotsUI;
    private AudioSource audioSource;
    private bool isOpen = false;

    void Start()
    {
        mountSlotsUI = new List<MountSlot>();
        audioSource = GetComponent<AudioSource>();
        mounts = mountController.GetAllMounts();
        int index = 0;
        foreach (var mount in mounts)
        {
            var slot = Instantiate(mountSlot, mount.transform.position, mount.transform.rotation, transform) as MountSlot;
            slot.Initialize(mount, index++);
            mountSlotsUI.Add(slot);
        }
    }
    
    /// <summary>
    /// Called to toggle display to inventory UI.
    /// </summary>
    public void Toggle()
    {
        if (!isOpen)
        {
            foreach (var mount in mountSlotsUI)
                mount.gameObject.SetActive(true);
            audioSource.clip = mountUIOpen;
            audioSource.Play();
            gameObject.GetComponent<Canvas>().enabled = true;
            isOpen = true;
        }
        else
        {
            foreach (var mount in mountSlotsUI)
                mount.gameObject.SetActive(false);
            audioSource.clip = mountUIClose;
            audioSource.Play();
            gameObject.GetComponent<Canvas>().enabled = false;
            isOpen = false;
        }
    }

}
