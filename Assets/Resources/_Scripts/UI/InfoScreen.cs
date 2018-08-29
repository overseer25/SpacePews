using System.Collections;
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
    public GameObject quantity;
    public GameObject value;
    public GameObject description;
    public GameObject damage;
    public GameObject critChance;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        Debug.Log("Mouse: " + Input.mousePosition);
        var newPos = Input.mousePosition;
        newPos.z = transform.position.z - Camera.main.transform.position.z;

        transform.position = Camera.main.ScreenToWorldPoint(newPos);

        Debug.Log(transform.position);
    }

    /// <summary>
    /// Sets the information to display to the user.
    /// </summary>
    public void SetInfo(string name, string type, string quantity, string value, string description, string damage = null, string critChance = null)
    {
        this.name.GetComponent<TextMeshProUGUI>().text = name;
        this.type.GetComponent<TextMeshProUGUI>().text = type;
        this.quantity.GetComponent<TextMeshProUGUI>().text = quantity;
        this.value.GetComponent<TextMeshProUGUI>().text = value;
        this.description.GetComponent<TextMeshProUGUI>().text = description;
        this.damage.GetComponent<TextMeshProUGUI>().text = damage;
        this.critChance.GetComponent<TextMeshProUGUI>().text = critChance;
    }

}
