using UnityEngine;

public class ThrusterComponent : ShipComponent
{
    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        itemType = ItemType.Thruster;
    }

    private void Start()
    {
        if (!mounted)
            GetComponentInChildren<Thruster>().gameObject.SetActive(false);
    }
}
