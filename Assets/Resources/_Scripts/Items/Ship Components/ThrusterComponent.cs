using UnityEngine;

public class ThrusterComponent : ShipComponent
{
    public float acceleration;
    public float deceleration;
    public float maxSpeed;
    public float rotationSpeed;

    public AudioClip engine;

    public Thruster thruster;
    public new ParticleSystem particleSystem;

    protected override void Awake()
    {
        base.Awake();
        itemType = ItemType.Thruster;
        particleSystem = GetComponentInChildren<ParticleSystem>(true);
        thruster = GetComponentInChildren<Thruster>(true);
        mounted = false;
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
        base.SetMounted(val);
        if(mounted)
            particleSystem.gameObject.SetActive(true);
    }
}
