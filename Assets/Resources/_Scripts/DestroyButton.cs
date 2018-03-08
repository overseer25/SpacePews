using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public InventorySlot slot; // Slot being manipulated
    public AudioSource destroyAudio;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Pointer Pressed " + gameObject.name);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        slot.SetItem(null);
        slot.UpdateSlot();
        GetComponentInParent<InventoryTooltip>().gameObject.SetActive(false);
        AudioSource.PlayClipAtPoint(destroyAudio.clip, GameObject.FindGameObjectWithTag("Player").transform.position);
        Debug.Log("Pointer Released " + gameObject.name);
    }
}
