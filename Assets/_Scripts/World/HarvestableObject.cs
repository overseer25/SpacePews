﻿using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// A script attached to a world object that allows that object to be mined for resources using mining equipment.
/// The object can decide the 
/// </summary>
public class HarvestableObject : Lootable
{
    /// <summary>
    /// Amount of time it takes to harvest.
    /// </summary>
    public float timeToMine;
    /// <summary>
    /// Speed at which to reset the object to its initial state. This rate is applied to each state.
    /// </summary>
    public float resetSpeed;
    /// <summary>
    /// Use this for a <see cref="DynamicParticle"/> effect.
    /// </summary>
    public DynamicParticle destroyParticleDynamic;
    /// <summary>
    /// Use this for a <see cref="ParticleEffect"/> effect.
    /// </summary>
    public ParticleEffect destroyParticle;

    /// <summary>
    /// The states of the object change the appearance of the object as it is harvested. Transitions to
    /// the next state in the array occur as the harvesting gets closer to completion.
    /// </summary>
    public Sprite[] states;

    private Coroutine mining;
    private Coroutine reset;
    private bool beingMined = false;
    private float timeStarted;
    private AudioSource audioSource;
    private int currentState = 0;

    private void Start()
    {
        gameObject.tag = "Mineable";
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Begins the mining process.
    /// <param name="mineRateModifier">The modifier of the mining tool to speed/slow down the mining time.</param>
    /// </summary>
    public void Mine(float mineRateModifier)
    {
        if(reset != null)
        {
            StopCoroutine(reset);
            reset = null;
        }
        if(!beingMined)
        {
            timeStarted = Time.deltaTime;
            mining = StartCoroutine(Mining(mineRateModifier));
        }
    }

    /// <summary>
    /// Cancel the mining process, and begin resetting to default state.
    /// </summary>
    public void CancelMining()
    {
        if(mining != null)
        {
            beingMined = false;
            StopCoroutine(mining);
            mining = null;
            reset = StartCoroutine(ResetToInitialState());
        }
    }

    /// <summary>
    /// Play the destroy effect(s). Can play both if they are each not null.
    /// </summary>
    private void PlayDestroyEffect()
    {
        if(destroyParticle != null)
        {
            ParticleManager.PlayParticle(destroyParticle, transform.position);
        }

        if(destroyParticleDynamic != null)
        {
            var effect = Instantiate(destroyParticleDynamic, transform.position, destroyParticleDynamic.transform.rotation);
            effect.Play();
        }
    }

    /// <summary>
    /// Changes the sprite of the <see cref="HarvestableObject"/> based on the current state,
    /// and spawns the loot and destroys the object when mining is complete.
    /// </summary>
    /// <param name="mineRateModifier"></param>
    /// <returns></returns>
    private IEnumerator Mining(float mineRateModifier)
    {
        beingMined = true;
        float mineTime = timeToMine / mineRateModifier;

        // If the object has states, incorporate them into the mining process.
        if (states != null && states.Length > 0)
        {
            while (currentState != states.Length)
            {
                UpdateState();
                yield return new WaitForSeconds(mineTime / states.Length);
                currentState++;
            }
        }
        else
            yield return new WaitForSeconds(mineTime);

        SpawnLoot();
        PlayDestroyEffect();
        Destroy(gameObject);
    }

    /// <summary>
    /// Reset the object to its default state (0).
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetToInitialState()
    {
        while(currentState != 0)
        {
            yield return new WaitForSeconds(resetSpeed);
            currentState--;
            UpdateState();
        }
        yield return null;
    }

    /// <summary>
    /// Updates the sprite to the one designated by the current state variable.
    /// </summary>
    private void UpdateState()
    {
        GetComponent<SpriteRenderer>().sprite = states[currentState];
    }

}
