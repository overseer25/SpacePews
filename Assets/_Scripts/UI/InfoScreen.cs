using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{

	public static InfoScreen current;

    [Header("Displays")]
    public TextMeshProUGUI displayText;
    public TextMeshProUGUI value;
    public TextMeshProUGUI quantity;
	public Image panelSprite;

	//Enabling/disabling this hides the screen.
	private CanvasGroup cg;
	private bool isDead;

	private float yPad;
	private float xPad;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
		if (current == null)
			current = this;
    }

    void Update()
    {
		if (PauseMenuScript.IsPaused)
			Hide();

        var mousePos = Input.mousePosition;
		mousePos.x += xPad;
		mousePos.y += yPad;
        mousePos.z = transform.position.z - Camera.main.transform.position.z;

        var pos = Camera.main.ScreenToWorldPoint(mousePos);
        pos.z = transform.parent.transform.position.z;
        transform.position = Vector3.Slerp(transform.position, pos, Time.deltaTime * 10.0f);
    }

	public void UpdateDead(bool dead)
	{
		isDead = dead;
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
		if (isDead || PauseMenuScript.IsPaused)
			return;

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
    /// Sets the item information to display to the user.
    /// </summary>
    public void SetInfo(Item item, int count = 0)
    {
		Color color = ItemColors.colors[(int)item.itemTier];
		var hex = color.ToHex();
		displayText.text = "<color=" + hex + ">" + item.itemName + "</color>\n";
		panelSprite.enabled = true;

		if(item is Consumable)
		{
			var consumable = item as Consumable;
			displayText.text += "<style=\"Type\">Consumable</style>\n";
		}
		// Display stats different if item is a weapon component.
		else if (item is WeaponComponentBase)
        {
            var weapComp = item as WeaponComponentBase;
            
            displayText.text += "<style=\"Type\">" + weapComp.GetComponentClass() + " " + weapComp.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Damage\">Damage (<style=\"DamageNum\">" + weapComp.GetDamageString() + ")</style></style>\n";
            displayText.text += "<style=\"CritChanceText\">Crit Chance: " + weapComp.GetCriticalChanceString() + "</style>(<style=\"CritMult\">" + weapComp.GetCriticalMultiplierString() + "</style>)\n";
        }
        // If the item is a thruster component.
        else if(item is ThrusterComponent)
        {
            var thrusterComp = item as ThrusterComponent;
            displayText.text += "<style=\"Type\">" + thrusterComp.GetComponentClass() + " " + thrusterComp.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Speed\">" + "Speed: <style=\"SpeedNum\">" + thrusterComp.maxSpeed + "</style> m/s" + "</style>\n";
            displayText.text += "<style=\"Acceleration\">" + "Acceleration: <style=\"AccNum\">" + thrusterComp.acceleration + "</style> m/s^2" + "</style>\n";
        }
        else if(item is StorageComponent)
        {
            var storageComp = item as StorageComponent;
            displayText.text += "<style=\"Type\">" + storageComp.GetComponentClass() + " " + storageComp.GetItemType() + "</style>\n";
            displayText.text += "<style=\"Speed\">" + "Size: <style=\"SpeedNum\">" + storageComp.slotCount + "</style> slots" + "</style>\n";
        }
        else if(item is MiningComponent)
        {
            var miningComp = item as MiningComponent;
            displayText.text += "<style=\"Type\">" + miningComp.GetComponentClass() + " Mining Laser</style>\n";
            displayText.text += "<style=\"Speed\">" + "Mining rate: <style=\"SpeedNum\">" + miningComp.GetMiningRate() + "%</style>" + "</style>\n";
        }
        else if(item is StatBuffUpgradeComponent)
        {
            var buffComp = item as StatBuffUpgradeComponent;
            displayText.text += "<style=\"Type\">" + buffComp.GetComponentClass() + " Upgrade Component</style>\n";
            foreach(var buff in buffComp.buffs)
                displayText.text += "<style=\"BuffText\">" + "-" + buff.infoScreenText + "</style>\n";
        }
		else if(item is ActiveAbilityComponent)
		{
			var activeAbilityComp = item as ActiveAbilityComponent;
			displayText.text += "<style=\"Type\">" + activeAbilityComp.GetComponentClass() + " Upgrade Component</style>\n";
			displayText.text += "<style=\"BuffText\">" + activeAbilityComp.BuildInfoScreenText() + "</style>\n";
		}
        // Generic item.
        else
        {
            displayText.text += "<style=\"Type\">" + item.GetItemType() + "</style>\n";
        }

		displayText.text += "\n<style=\"Description\">" + item.description + "</style>\n";

		value.text = "<style=\"Value\">" + item.value + ((item.value == 1) ? " Unit " : " Units ");
        value.text += (count > 1) ? "(" + item.value * count + ")</style>" : "";
        quantity.text = (item.stackable) ? count.ToString() : " ";

        // Resize the window to accomodate the new text.
        ResizeWindow();
		xPad = 0f;
		yPad = 30f;
    }

	/// <summary>
	/// Sets the buff information to display to the user.
	/// </summary>
	/// <param name="buff"></param>
	public void SetInfo(Buff buff)
	{
		panelSprite.enabled = false;
		displayText.text = "<style=\"BuffIconText\">" + buff.buffIconText + "</style>";
		quantity.text = "";
		value.text = "";
		ResizeWindow();
		xPad = 0f;
		yPad = -GetComponent<RectTransform>().sizeDelta.y;
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
