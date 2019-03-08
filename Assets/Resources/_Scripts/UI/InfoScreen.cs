using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{

    [Header("Displays")]
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI value;
    public TextMeshProUGUI quantity;

    //Enabling/disabling this hides the screen.
    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

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
    /// Is the panel currently visible?
    /// </summary>
    /// <returns></returns>
    public bool IsVisible()
    {
        return cg.alpha == 1;
    }

    /// <summary>
    /// Shows the info screen.
    /// </summary>
    public void Show()
    {
        if (cg.alpha == 0)
        {
            cg.alpha = 1;
        }
    }

    /// <summary>
    /// Hides the info screen.
    /// </summary>
    public void Hide()
    {
        if (cg.alpha == 1)
        {
            cg.alpha = 0;
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
            displayText.text = item.itemName + "\n";
            displayText.color = ItemColors.colors[(int)item.itemTier];
            displayText.text += "<style=\"Type\">" + weapComp.GetComponentClass() + " " + weapComp.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Damage\">Damage (<style=\"DamageNum\">" + weapComp.GetDamageString() + "</style></style>)\n";
            displayText.text += "<style=\"CritChanceText\">Crit Chance: " + weapComp.GetCriticalChanceString() + "</style>(<style=\"CritMult\">" + weapComp.GetCriticalMultiplierString() + "</style>)\n";
            displayText.text += "<style=\"Description\">" + weapComp.description + "</style>\n";
        }
        // If the item is a thruster component.
        else if(item is ThrusterComponent)
        {
            var thrusterComp = item as ThrusterComponent;
            displayText.text = item.itemName + "\n";
            displayText.color = ItemColors.colors[(int)item.itemTier];
            displayText.text += "<style=\"Type\">" + thrusterComp.GetComponentClass() + " " + thrusterComp.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Speed\">" + "Speed: <style=\"SpeedNum\">" + thrusterComp.maxSpeed + "</style> m/s" + "</style>\n";
            displayText.text += "<style=\"Acceleration\">" + "Acceleration: <style=\"AccNum\">" + thrusterComp.acceleration + "</style> m/s^2" + "</style>\n";
            displayText.text += "<style=\"Description\">" + thrusterComp.description + "</style>\n";
        }
        else if(item is StorageComponent)
        {
            var storageComp = item as StorageComponent;
            displayText.text = item.itemName + "\n";
            displayText.color = ItemColors.colors[(int)item.itemTier];
            displayText.text += "<style=\"Type\">" + storageComp.GetComponentClass() + " " + storageComp.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Speed\">" + "Size: <style=\"SpeedNum\">" + storageComp.slotCount + "</style> slots" + "</style>\n";
            displayText.text += "<style=\"Description\">" + storageComp.description + "</style>\n";
        }
        else if(item is MiningComponent)
        {
            var miningComp = item as MiningComponent;
            displayText.text = item.itemName + "\n";
            displayText.color = ItemColors.colors[(int)item.itemTier];
            displayText.text += "<style=\"Type\">" + miningComp.GetComponentClass() + " Mining Laser</style>\n";
            displayText.text += "<style=\"Speed\">" + "Mining rate: <style=\"SpeedNum\">" + miningComp.GetMiningRate() + "%</style>" + "</style>\n";
            displayText.text += "<style=\"Description\">" + miningComp.description + "</style>\n";
        }
        // Generic item.
        else
        {
            displayText.text = item.itemName + "\n";
            displayText.color = ItemColors.colors[(int)item.itemTier];
            displayText.text += "<style=\"Type\">" + item.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Description\">" + item.description + "</style>\n";
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
        height += (displayText.text != "") ? displayText.preferredHeight + spacing : 0;
        height += (value.text != "") ? value.preferredHeight + spacing : 0;
        height += (quantity.text != "") ? quantity.preferredHeight + spacing : 0;

        var panel = GetComponent<RectTransform>();
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, height);
    }

}
