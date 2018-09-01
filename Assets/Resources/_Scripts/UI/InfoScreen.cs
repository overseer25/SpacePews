﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InfoScreen : MonoBehaviour {

    private RectTransform rt;

    [Header("Displays")]
    public new GameObject name;
    public GameObject type;
    public GameObject value;
    public GameObject description;
    public GameObject damage;
    public GameObject critChance;

    [Header("Sounds")]
    public AudioClip hoverOver;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        var newPos = Input.mousePosition;
        newPos.z = transform.position.z - Camera.main.transform.position.z;

        transform.position = Camera.main.ScreenToWorldPoint(newPos);
    }

    /// <summary>
    /// Shows the info screen.
    /// </summary>
    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);

            gameObject.GetComponent<AudioSource>().clip = hoverOver;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    /// <summary>
    /// Hides the info screen.
    /// </summary>
    public void Hide()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Sets the information to display to the user.
    /// </summary>
    public void SetInfo(Item item)
    {
        name.GetComponent<TextMeshProUGUI>().text = item.name;
        name.GetComponent<TextMeshProUGUI>().color = ItemColors.colors[(int)item.itemTier];
        type.GetComponent<TextMeshProUGUI>().text = item.type;
        value.GetComponent<TextMeshProUGUI>().text = item.value.ToString();
        description.GetComponent<TextMeshProUGUI>().text = item.description;
        damage.GetComponent<TextMeshProUGUI>().text = "";
        critChance.GetComponent<TextMeshProUGUI>().text = "";
    }

}
