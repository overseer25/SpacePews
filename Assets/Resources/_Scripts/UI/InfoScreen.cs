using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InfoScreen : MonoBehaviour {

    private RectTransform rt;

    [Header("Displays")]
    public new TextMeshProUGUI name;
    public TextMeshProUGUI type;
    public TextMeshProUGUI value;
    public TextMeshProUGUI description;
    public TextMeshProUGUI bonusText1;
    public TextMeshProUGUI bonusText2;
    public TextMeshProUGUI quantity;

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
        name.text = item.name;
        name.color = ItemColors.colors[(int)item.itemTier];
        type.text = item.type;
        value.text = item.value + ((item.value == 1) ? " Unit " : " Units ") + "(" + item.value * item.quantity + ")";
        description.text = item.description;

        // Display additional stats if it is a weapon component.
        if(item is WeaponComponent)
        {
            var weapComp = item as WeaponComponent;
            bonusText1.text = "Damage (" + weapComp.GetDamageString() + ")";
            bonusText2.text = "Crit Chance: " + weapComp.GetCriticalChanceString() ;
        }
        else
        {
            bonusText1.text = "";
            bonusText2.text = "";
        }

        quantity.text = item.quantity.ToString();

        // Resize the window to accomodate the new text.
        ResizeWindow();
    }

    /// <summary>
    /// Resize the info screen height depending on the size of the contents.
    /// </summary>
    public void ResizeWindow()
    {
        float height = 0.0f;
        float spacing = GetComponentInChildren<VerticalLayoutGroup>().spacing;
        height += (name.text != "") ? name.preferredHeight + spacing : 0;
        height += (type.text != "") ? type.preferredHeight + spacing : 0;
        height += (value.text != "") ? value.preferredHeight + spacing : 0;
        height += (description.text != "") ? description.preferredHeight + spacing : 0;
        height += (bonusText1.text != "") ? bonusText1.preferredHeight + spacing : 0;
        height += (bonusText2.text != "") ? bonusText2.preferredHeight + spacing : 0;
        height += (quantity.text != "") ? quantity.preferredHeight + spacing : 0;


        var panel = GetComponent<RectTransform>();
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
        Debug.Log(panel.sizeDelta);
    }

}
