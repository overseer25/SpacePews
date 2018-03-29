using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InfoButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public InventorySlot slot; // Slot being manipulated
    public InfoScreen screen;
    public AudioSource menuOpenSound;


    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Pressed " + gameObject.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        screen.itemImage.sprite = slot.item_sprite.sprite;
        screen.description.text = slot.GetItem().description;
        screen.stats.text = "";
        screen.stats.text += "Name: " + slot.GetItem().name + "\n";
        screen.stats.text += "Type: " + slot.GetItem().type + "\n";
        screen.stats.text += "Quantity: " + slot.quantity + "\n"; // need to change quantity to store in the slot, not item.
        screen.stats.text += "Value: " + slot.GetItem().value * slot.quantity;

        screen.gameObject.SetActive(true);
        AudioSource.PlayClipAtPoint(menuOpenSound.clip, GameObject.FindGameObjectWithTag("Player").transform.position);

        GetComponentInParent<InventoryTooltip>().gameObject.SetActive(false);
        Debug.Log("Pointer Released " + gameObject.name);
    }
}
