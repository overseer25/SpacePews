using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private bool dead = false;
    internal bool isPaused = false;
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
        if(!dead)
        {
            if (currentComponent != null)
            {
                if (currentComponent is WeaponComponent)
                    (currentComponent as WeaponComponent).GetComponent<ProjectilePool>().DestroyPool();
                Destroy(currentComponent.gameObject);
            }
            if (component == null)
            {
                return;
            }
            var hotbarSlotItem = inventory.GetSelectedHotbarSlot().GetItem();
            currentComponent = Instantiate(hotbarSlotItem, turret.transform.position, turret.transform.rotation, turret.transform) as ShipComponent;
            currentComponent.gameObject.SetActive(true);
            if (currentComponent is WeaponComponent)
                (currentComponent as WeaponComponent).GetComponent<ProjectilePool>().CreatePool();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!dead && !isPaused)
        {
            // Change the item if necessary.
            if (Input.GetMouseButton(0) && !menuOpen && currentComponent != null)
            {
                if (currentComponent is WeaponComponent)
                {
                    var weapon = currentComponent as WeaponComponent;
                    if (Time.time > weapon.GetNextShotTime())
                    {
                        weapon.Fire();
                        weapon.SetLastShot(Time.time);
                    }
                }
                else if (currentComponent is MiningComponent)
                {
                    var miningLaser = currentComponent as MiningComponent;
                    miningLaser.Fire();
                }
            }
            if (Input.GetMouseButtonUp(0) || menuOpen || currentComponent == null)
            {
                if (currentComponent is MiningComponent)
                {
                    var miningLaser = currentComponent as MiningComponent;
                    miningLaser.StopFire();
                }
            }
        }
    }

    /// <summary>
    /// Update the controller on the state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        if(!dead && isDead)
        {
            dead = isDead;
            turret.gameObject.SetActive(false);
            menuOpen = false;
        }
        else if(dead && !isDead)
        {
            dead = isDead;
            turret.gameObject.SetActive(true);
        }
        
    }
}
