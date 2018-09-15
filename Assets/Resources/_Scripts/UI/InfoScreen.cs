using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{

    [Header("Displays")]
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    public TextMeshProUGUI value;
    public TextMeshProUGUI quantity;

    void Update()
    {
        var mousePos = Input.mousePosition;
        mousePos.y = Input.mousePosition.y + 30.0f;
        mousePos.z = transform.position.z - Camera.main.transform.position.z;

        var pos = Camera.main.ScreenToWorldPoint(mousePos);
        pos.z = transform.parent.transform.position.z;
        transform.position = Vector3.Slerp(transform.position, pos, Time.deltaTime * 10.0f);
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
    public void SetInfo(Item item, int count)
    {
        // Display stats different if item is a weapon component.
        if (item is WeaponComponent)
        {
            var weapComp = item as WeaponComponent;
            text1.text = item.name;
            text1.color = ItemColors.colors[(int)item.itemTier];
            text2.text = "<style=\"Type\">" + item.type + "</style>";
            text3.text = "<style=\"Damage\">Damage (<style=\"DamageNum\">" + weapComp.GetDamageString() + "</style></style>)";
            text4.text = "Crit Chance: " + weapComp.GetCriticalChanceString() + " (<style=\"CritMult\">" + weapComp.GetCriticalMultiplierString() + "</style>)";
            text5.text = "<style=\"Description\">" + item.description + "</style>";
        }
        // If the item is a thruster component.
        else if(item is ThrusterComponent)
        {
            var thrusterComp = item as ThrusterComponent;
            text1.text = item.name;
            text1.color = ItemColors.colors[(int)item.itemTier];
            text2.text = "<style=\"Type\">" + item.type + "</style>";
            text3.text = "<style=\"Speed\">" + "Speed: <style=\"SpeedNum\">" + thrusterComp.maxSpeed + "</style> m/s" + "</style>";
            text4.text = "<style=\"Acceleration\">" + "Acceleration: <style=\"AccNum\">" + thrusterComp.acceleration + "</style> m/s^2" + "</style>";
            text5.text = "<style=\"Description\">" + item.description + "</style>";
        }
        // Generic item.
        else
        {
            text1.text = item.name;
            text1.color = ItemColors.colors[(int)item.itemTier];
            text2.text = "<style=\"Type\">" + item.type + "</style>";
            text3.text = "<style=\"Description\">" + item.description + "</style>";
            text4.text = "";
            text5.text = "";
        }

        value.text = "<style=\"Value\">" + item.value + ((item.value == 1) ? " Unit " : " Units ");
        value.text += (count > 1) ? "(" + item.value * count + ")</style>" : "";
        quantity.text = (item.stackable) ? count.ToString() : " ";

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
        height += (text1.text != "") ? text1.preferredHeight + spacing : 0;
        height += (text2.text != "") ? text2.preferredHeight + spacing : 0;
        height += (text3.text != "") ? text3.preferredHeight + spacing : 0;
        height += (text4.text != "") ? text4.preferredHeight + spacing : 0;
        height += (text5.text != "") ? text5.preferredHeight + spacing : 0;
        height += (value.text != "") ? value.preferredHeight + spacing : 0;
        height += (quantity.text != "") ? quantity.preferredHeight + spacing : 0;

        var panel = GetComponent<RectTransform>();
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
    }

}
