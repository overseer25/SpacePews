using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    public bool menuOpen = false;
    [Header("Inventory")]
    public Inventory inventory;
    private Ship ship;

    private GameObject turret;
    private ShipComponent currentComponent;

    [SerializeField]
    private ShipMountController mountController;

    void Start()
    {
        ship = GetComponentInChildren<Ship>();
        turret = ship.turret;
    }

    /// <summary>
    /// Update the turret to use the provided component.
    /// </summary>
    /// <param name="component"></param>
    public void UpdateTurret(ShipComponent component)
    {
        if (!dead)
        {
            if (currentComponent != null)
            {
                if (currentComponent is WeaponComponent)
                    (currentComponent as WeaponComponent).SetMounted(false);
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
                currentComponent = Instantiate(hotbarSlotItem, turret.transform.position, turret.transform.rotation, turret.transform) as ShipComponent;
                currentComponent.gameObject.SetActive(true);
                if (currentComponent is WeaponComponent)
                    (currentComponent as WeaponComponent).SetMounted(true);
                else if (currentComponent is MiningComponent)
                    (currentComponent as MiningComponent).SetMounted(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (currentComponent is ChargedWeapon)
            {
                HandleChargedWeapon(currentComponent as ChargedWeapon);
            }
            else if (currentComponent is AutomaticWeapon)
            {
                if (Input.GetMouseButton(0) && !menuOpen && currentComponent != null)
                {
                    (currentComponent as AutomaticWeapon).CheckFire();
                }
            }
            else if (currentComponent is MiningComponent)
            {
                HandleMiningTool(currentComponent as MiningComponent);
            }
        }
    }

    /// <summary>
    /// Handle the functionality of the charged weapon equipped.
    /// </summary>
    /// <param name="weapon"></param>
    private void HandleChargedWeapon(ChargedWeapon weapon)
    {
        if (Input.GetMouseButton(0) && currentComponent != null && !weapon.cooldownActive && !weapon.decharging)
        {
            if (!menuOpen)
                weapon.CheckFire();
            else
                weapon.CancelFire();
        }
        else if (Input.GetMouseButtonUp(0) && currentComponent != null && !weapon.cooldownActive && !weapon.decharging)
        {
            if (weapon.charged)
            {
                weapon.Fire();
            }
            else
            {
                weapon.CancelFire();
            }
        }
        else if (weapon.cooldownActive)
        {
            weapon.StartCooldown();
        }
        else if (weapon.decharging)
        {
            weapon.CancelFire();
        }
    }

    /// <summary>
    /// Handle the functionality of the mining tool equipped.
    /// </summary>
    /// <param name="miningComponent"></param>
    private void HandleMiningTool(MiningComponent miningComponent)
    {
        if (Input.GetMouseButton(0) && !menuOpen && currentComponent != null)
        {
            UpdateMiningFire(miningComponent);
        }
        // What to do if a menu opens.
        if (Input.GetMouseButtonUp(0) || menuOpen || currentComponent == null)
        {
            miningComponent.StopFire();
        }
    }

    /// <summary>
    /// Fires the mining laser.
    /// </summary>
    /// <param name="currentComponent"></param>
    private void UpdateMiningFire(MiningComponent currentComponent)
    {
        var miningLaser = currentComponent;
        miningLaser.Fire();
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
            turret.gameObject.SetActive(false);
            menuOpen = false;
        }
        else if (dead && !isDead)
        {
            dead = isDead;
            turret.gameObject.SetActive(true);
        }

    }
}
