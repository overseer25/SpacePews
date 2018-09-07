using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableElement : MonoBehaviour {

    [Header("Sounds")]
    public AudioClip enterSound;
    public AudioClip exitSound;

    // Highlight the image when hovering over it
    internal Image image;
    private TextMeshProUGUI text;
    internal int index;
    internal AudioSource audioSource;
    private bool hovering = false;

    void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        audioSource = GetComponent<AudioSource>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);
    }

    /// <summary>
    /// Sets the index of the slot.
    /// </summary>
    public void SetIndex(int i)
    {
        index = i;
    }

    /// <summary>
    /// Gets the index of the slot.
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        return index;
    }

    // Remove highlight when no longer hovering
    void OnMouseOver()
    {
        if(!hovering && InventoryItem.dragging)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
            if (text != null)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
            hovering = true;

            if (enterSound != null)
            {
                audioSource.clip = enterSound;
                audioSource.Play();
            }
        }
        // If item is no longer being dragged, do not highlight.
        else if(hovering && !InventoryItem.dragging)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);
            if (text != null)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.7f);
            hovering = false;
        }
    }

    // Remove highlight when no longer hovering
    void OnMouseExit()
    {
        if (hovering && InventoryItem.dragging)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.7f);
            if (text != null)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.7f);
            hovering = false;

            if (exitSound != null)
            {
                audioSource.clip = exitSound;
                audioSource.Play();
            }
        }
    }
}
