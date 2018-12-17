﻿using UnityEngine;

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
        var particleSystem = GetComponentInChildren<ParticleSystem>();
        var thruster = GetComponentInChildren<Thruster>();

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
            GetComponentInChildren<ParticleSystem>().gameObject.SetActive(true);
    }
}
