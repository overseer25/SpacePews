using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    [HideInInspector]
    public bool menuOpen = false;
    [Header("Inventory")]
    public Inventory inventory;
	public GameObject turret;
    [HideInInspector]
    public ShipComponentBase currentComponent;

    [SerializeField]
    private ShipMountController mountController;

    /// <summary>
    /// Update the turret to use the provided component.
    /// </summary>
    /// <param name="component"></param>
    public void UpdateTurret(ShipComponentBase component)
    {
        if (!dead)
        {
            if (currentComponent != null)
            {
                if (currentComponent is WeaponComponentBase)
                    (currentComponent as WeaponComponentBase).SetMounted(false);
                else if (currentComponent is MiningComponent)
                    (currentComponent as MiningComponent).SetMounted(false);
                Destroy(currentComponent.gameObject);
            }
            if (component == null)
            {
                return;
            }
            var hotbarSlotItem = inventory.GetSelectedHotbarSlot().GetItem();
            if (hotbarSlotItem != null)
            {
                currentComponent = Instantiate(hotbarSlotItem, turret.transform.position, turret.transform.rotation, turret.transform) as ShipComponentBase;
                currentComponent.gameObject.SetActive(true);
                if (currentComponent is WeaponComponentBase)
                    (currentComponent as WeaponComponentBase).SetMounted(true);
                else if (currentComponent is MiningComponent)
                    (currentComponent as MiningComponent).SetMounted(true);
            }
        }
    }

    /// <summary>
    /// Update the controller on the state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        if (!dead && isDead)
        {
            dead = isDead;
            turret.SetActive(false);
            menuOpen = false;
        }
        else if (dead && !isDead)
        {
            dead = isDead;
            turret.SetActive(true);
        }
    }
}
