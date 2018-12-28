using UnityEngine;

public class ThrusterComponent : ShipComponent
{
    [Header("Stats")]
    public float acceleration;
    public float deceleration;
    public float maxSpeed;
    public float rotationSpeed;

    [Header("Sounds")]
    public AudioClip engine;

    void Awake()
    {
        itemColor = ItemColors.colors[(int)itemTier];
        itemType = ItemType.Thruster;
    }

    private void Start()
    {
        var particleSystem = GetComponentInChildren<ParticleSystem>(true);
        var thruster = GetComponentInChildren<Thruster>(true);

        if(particleSystem == null)
            Debug.LogError("No particle system attached to: " + gameObject);
        if (thruster == null)
            Debug.LogError("No thruster script attached to: " + gameObject);

        if (thruster != null && particleSystem != null && !mounted)
        {
            thruster.gameObject.SetActive(false);
            particleSystem.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Set the mounted status of the component.
    /// </summary>
    /// <param name="val"></param>
    public override void SetMounted(bool val)
    {
        mounted = val;
        if(mounted)
            GetComponentInChildren<ParticleSystem>(true).gameObject.SetActive(true);
    }
}
