﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

/// <summary>
/// Converts inputs given by the player into actions and movements of the player character.
/// </summary>
public class PlayerController : MonoBehaviour
{
    // Constants
    private const float RESPAWN_WAIT_TIME = 5.0f;
    private const float RESPAWN_ANIMATION_TIME = 0.5f;
    private const float CAMERA_ZOOM_INCREMENT = 10.0f;
    private const float CAMERA_MAX_ZOOM = -40.0f;
    private const float CAMERA_MIN_ZOOM = -60.0f;
    private const float CAMERA_FOLLOW_SPEED = 10.0f;

    [Header("UI")]
    public Inventory inventory;
    public DeathScreen deathScreen;
    public PauseMenuScript pauseMenu;
    public GameObject itemTransferConfirmWindow;
    public GameObject healthUI;
	public GameObject buffGrid;
    public GameObject abilityChargeBar;
    public TextMeshProUGUI currencyCount;
    [Header("State")]
    public GameObject respawnPoint;
    [Header("Effects/Sounds")]
    public ParticleEffect deathExplosion;
    public ParticleEffect respawnEffect;
    public AudioSource engineSource;
    public FadeEffect ghostEffect;

    // The active ability the player can currently use.
    private AbilityBase ability;
    private Coroutine abilityCooldown;
    private float acceleration;
    private Rigidbody2D rigidBody;
    private MovementController movementController;
    private ShipMountController mountController;
    private WeaponController weaponController;
    private PlayerHealthController healthController;
    private List<Thruster> thrusters; // Contains the thrusters of the ship;
    private Vector2 moveInput;
    private Vector3 previousCameraPosition; // Used to create floaty camera effect.
    private AudioClip engineAudio;

    private bool dead;
    private bool respawning;

    [HideInInspector]
    public bool rotatingLeft;
    [HideInInspector]
    public bool rotatingRight;
    [HideInInspector]
    public bool movingForward;
    private float currentCameraZoom;

    // The ship variables.
    private SpriteRenderer shipRenderer;
    private GameObject ship;
    private Canvas[] canvases;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        movementController = GetComponent<MovementController>();
        mountController = GetComponent<ShipMountController>();
        weaponController = GetComponent<WeaponController>();
        healthController = GetComponent<PlayerHealthController>();
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        canvases = transform.parent.GetComponentsInChildren<Canvas>();
        // Initial thrusters.
        thrusters = new List<Thruster>();
        foreach (var thrusterObj in mountController.GetThrusterMounts())
            thrusters.Add(thrusterObj.GetShipComponent().gameObject.GetComponentInChildren<Thruster>(true));
        UpdateThrusterList();

        if ((shipRenderer = GetComponentInChildren<SpriteRenderer>()) == null)
            Debug.LogError("Ship contains no Sprite Renderer :(");
        else
        {
            ship = shipRenderer.gameObject;
        }
        currentCameraZoom = CAMERA_MIN_ZOOM;
    }

    /// <summary>
    /// Tells the movement controller to decelerate the player ship and stop rotating.
    /// </summary>
    public void StopAllMovement()
    {
        movingForward = false;
        rotatingLeft = false;
        rotatingRight = false;
        SetThrusterState(false);
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            if (movingForward)
            {
                movementController.MoveForward();
                if (thrusters.Count > 0)
                    PlayEngineSound();
            }
            else
            {
                movementController.Decelerate();
                if (engineSource.isPlaying)
                    StopEngineSound();
            }

            if (rotatingRight)
                movementController.RotateRight();
            else if (rotatingLeft)
                movementController.RotateLeft();
        }
    }

    /// <summary>
    /// Movement stuff.
    /// </summary>
    private void Update()
    {
        if (healthController.IsDead() && !dead)
        {
            dead = true;
            Die();
        }
        if (ability != null && !PauseMenuScript.IsPaused && !dead)
        {
            if (!abilityChargeBar.activeInHierarchy)
                abilityChargeBar.SetActive(true);
            abilityChargeBar.GetComponent<ChargeBar>().SetFillPercentage(1 - ability.GetCooldownTimeRemaining());
        }
        else
        {
            if (abilityChargeBar.activeInHierarchy)
                abilityChargeBar.SetActive(false);
        }

        //Camera.main.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, currentCameraZoom)
    }

	private void LateUpdate()
	{
		var x = Mathf.Lerp(Camera.main.transform.position.x, ship.transform.position.x, CAMERA_FOLLOW_SPEED * Time.deltaTime);
		var y = Mathf.Lerp(Camera.main.transform.position.y, ship.transform.position.y, CAMERA_FOLLOW_SPEED * Time.deltaTime);
		Camera.main.transform.position = new Vector3(x, y, currentCameraZoom);
	}

    /// <summary>
    /// Zoom in the player camera, by increments based on constant defined.
    /// </summary>
    public void ZoomInCamera()
    {
        if (currentCameraZoom + CAMERA_ZOOM_INCREMENT > CAMERA_MAX_ZOOM)
            return;

        currentCameraZoom += CAMERA_ZOOM_INCREMENT;
    }

    /// <summary>
    /// Zoom out the player camera, by increments based on constant defined.
    /// </summary>
    public void ZoomOutCamera()
    {
        if (currentCameraZoom - CAMERA_ZOOM_INCREMENT < CAMERA_MIN_ZOOM)
            return;

        currentCameraZoom -= CAMERA_ZOOM_INCREMENT;
    }

    /// <summary>
    /// Update the list of thrusters.
    /// </summary>
    public void UpdateThrusterList()
    {
        float acc = 0;
        float dec = 0;
        float maxSpeed = 0;
        float rotSpeed = 0;
        if (mountController == null)
            return;
        thrusters = new List<Thruster>();
        foreach (var thrusterObj in mountController.GetThrusterMounts())
        {
            if (!thrusterObj.IsEmpty())
            {
                var thruster = thrusterObj.GetShipComponent().gameObject.GetComponentInChildren<Thruster>(true);
                thrusters.Add(thruster);
                var comp = ((ThrusterComponent)thrusterObj.GetShipComponent());
                acc += comp.acceleration;
                dec += comp.deceleration;
                maxSpeed += comp.maxSpeed;
                rotSpeed += comp.rotationSpeed;
            }

        }
        movementController.UpdateAcceleration(acc);
        movementController.UpdateMaxSpeed(maxSpeed);
        movementController.UpdateRotationSpeed(rotSpeed);
        movementController.UpdateDeceleration(dec);

        // Set the engine audio to the audio for the first thruster encountered in the list.
        if (thrusters.Count > 0)
        {
            engineAudio = (mountController.GetThrusterMounts().First().GetShipComponent() as ThrusterComponent).engine;
            engineSource.clip = engineAudio;
            engineSource.volume = 0.0f;
        }
        engineSource.Stop();
        SetThrusterState(false);
    }

    /// <summary>
    /// Set the state of all thrusters on the ship.
    /// </summary>
    /// <param name="state"></param>
    public void SetThrusterState(bool state)
    {
        if (thrusters.FirstOrDefault() == null)
            return;
        foreach (var thruster in thrusters)
        {
			if (state)
				thruster.Activate();
			else
				thruster.Deactivate();
        }
    }

    /// <summary>
    /// Get the state of the thrusters on the ship.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public bool GetThrusterState()
    {
        if (thrusters.FirstOrDefault() == null)
            return false;
        return thrusters.FirstOrDefault().isActive;
    }

    /// <summary>
    /// Set the active ability of the player. Also forces the ability to initially cooldown so that it cannot be abused.
    /// </summary>
    /// <param name="ability"></param>
    public void SetAbility(AbilityBase ability)
    {

        // If the same, do nothing.
        if (this.ability == ability)
            return;

        this.ability = ability;
        if (ability != null)
        {
            abilityChargeBar.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;
            if (abilityCooldown != null)
                StopCoroutine(abilityCooldown);
            abilityCooldown = StartCoroutine(this.ability.Cooldown());
        }
    }

    /// <summary>
    /// Get the active ability of the player.
    /// </summary>
    /// <returns></returns>
    public AbilityBase GetAbility()
    {
        return ability;
    }

    /// <summary>
    /// Start the cooldown process of the active ability.
    /// </summary>
    public void StartAbilityCooldown()
    {
        abilityCooldown = StartCoroutine(ability.Cooldown());
    }

    /// <summary>
    /// Apply a ghost effect to the player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Ghost(float rateToGenerate, float fadeRate, float fadeAmount, int numOfGhosts)
    {
        var count = 0;
        while(count != numOfGhosts)
        {
            var ghost = Instantiate(ghostEffect);
            ghost.Initialize(shipRenderer.sprite, ship.transform.position, ship.transform.rotation, fadeRate, fadeAmount, ship.gameObject.transform.localScale);
            count++;
            yield return new WaitForSeconds(rateToGenerate);
        }

        yield return null;
    }

    /// <summary>
    /// Die really hard.
    /// </summary>
    public void Die()
    {
        if (GetThrusterState())
            SetThrusterState(false);

        GetComponentInChildren<SpriteRenderer>().enabled = false;
        movementController.Stop();
        mountController.HideMounted();
        weaponController.UpdateDead(dead);
        inventory.UpdateDead(dead);
        movementController.UpdateDead(dead);
        pauseMenu.UpdateDead(dead);
        pauseMenu.ResumeGame();
        deathScreen.Display();
        healthUI.SetActive(false);
		buffGrid.SetActive(false);
        if (GetAbility() != null)
            abilityChargeBar.SetActive(false);
        BuffManager.current.RemoveAllTimedBuffs();
        engineSource.Stop();

        movingForward = false;
        rotatingLeft = false;
        rotatingRight = false;

        //spawn explosion effect.
        ParticleManager.PlayParticle(deathExplosion, gameObject.transform.position);

        StartCoroutine(Respawn());

    }

    /// <summary>
    /// Respawn the player.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Respawn()
    {
        if (!respawning)
        {
            respawning = true;
            // Wait before beginning the respawn animation.
            yield return new WaitForSeconds(RESPAWN_WAIT_TIME);

            // Spawn respawn effect.
            ParticleManager.PlayParticle(respawnEffect, respawnPoint.transform.position);

            transform.position = respawnPoint.transform.position;
            ship.transform.rotation = respawnPoint.transform.rotation;
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            deathScreen.Hide();

            // Wait before returning control to the player.
            yield return new WaitForSeconds(RESPAWN_ANIMATION_TIME);

            dead = false;
            GetComponentInChildren<SpriteRenderer>().enabled = true;
            mountController.ShowMounted();
            weaponController.UpdateDead(dead);
            inventory.UpdateDead(dead);
            movementController.UpdateDead(dead);
            pauseMenu.UpdateDead(dead);
            healthUI.SetActive(true);
			buffGrid.SetActive(true);
            if (GetAbility() != null)
                abilityChargeBar.SetActive(true);

            respawning = false;
            healthController.ResetHealth(GetComponent<Actor>().health / 2);
        }
    }

    /// <summary>
    /// Fade in the engine sound.
    /// </summary>
    private void PlayEngineSound()
    {
        if (!engineSource.isPlaying)
            engineSource.Play();
        engineSource.volume = (engineSource.volume < 0.5f) ? engineSource.volume + 0.2f : 0.5f;
    }

    /// <summary>
    /// Fade out the engine sound, and stop it once it reaches 0.
    /// </summary>
    private void StopEngineSound()
    {
        engineSource.volume = (engineSource.volume > 0) ? engineSource.volume - 0.02f : 0.0f;
        if (engineSource.volume <= 0.0f)
            engineSource.Stop();
    }

    /// <summary>
    /// Deals with all collisions with the player character
    /// </summary>
    /// <param name="collider"></param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        switch (collider.gameObject.tag)
        {
            case "Immovable":
            case "Asteroid":
            case "Mineable":
                Vector2 direction = Vector2.zero;
                if (rigidBody.velocity.magnitude > 10)
                {
					var damage = (int)System.Math.Floor(rigidBody.velocity.magnitude / 3);
					healthController.TakeDamage(damage);
					if(OptionsMenu.current.graphicsMenu.DamageNumbersEnabled())
					{
						var popUpText = PopUpTextPool.current.GetPooledObject() as PopUpText;
						if (popUpText != null)
						{
							popUpText.Initialize(gameObject, damage.ToString(), 10f, Color.red);
						}
					}
                }
                direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized * movementController.GetMaxSpeed() * Time.fixedDeltaTime * 20f;

                movementController.Stop();
                if (!healthController.IsDead())
                {
                    movementController.MoveDirection(direction, true);
                }
                break;

            case "Enemy":
                healthController.TakeDamage(5);
                break;
        }
    }
}
