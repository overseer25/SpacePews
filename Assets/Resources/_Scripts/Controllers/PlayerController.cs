using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	private const float CAMERA_MAX_ZOOM = -20.0f;
	private const float CAMERA_MIN_ZOOM = -50.0f;
	private const float CAMERA_FOLLOW_SPEED = 10.0f;

	[Header("State")]
	public Inventory inventory;
	public DeathScreen deathScreen;
	public PauseMenuScript pauseMenu;
	public GameObject itemTransferConfirmWindow;
    public GameObject HealthUI;
	public GameObject respawnPoint;
	[Header("Effects/Sounds")]
	public ParticleEffect deathExplosion;
	public AudioClip deathSound;
	public ParticleEffect respawnEffect;
	public AudioClip respawnSound;

	private float acceleration;
	private Rigidbody2D rigidBody;
	private MovementController movementController;
	private ShipMountController mountController;
	private WeaponController weaponController;
	private PlayerHealthController healthController;
	private List<Thruster> thrusters; // Contains the thrusters of the ship;
	private Vector2 moveInput;
	private Vector3 previousCameraPosition; // Used to create floaty camera effect.
	private AudioSource engine;
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
		engine = GetComponent<AudioSource>();
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

	/// <summary>
	/// Gets the current health value of the player.
	/// </summary>
	/// <returns></returns>
	public int GetHealth()
	{
		return 0;
	}

	/// <summary>
	/// Movement stuff.
	/// </summary>
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
				if (engine.isPlaying)
					StopEngineSound();
			}

			if (rotatingRight)
				movementController.RotateRight();
			else if (rotatingLeft)
				movementController.RotateLeft();

			Camera.main.transform.position = Vector3.Slerp(Camera.main.transform.position,
											new Vector3(ship.transform.position.x, ship.transform.position.y, currentCameraZoom), CAMERA_FOLLOW_SPEED * Time.deltaTime);
		}
	}

	/// <summary>
	/// Zoom in the player camera, by increments based on constant defined.
	/// </summary>
	public void ZoomInCamera()
	{
		if (currentCameraZoom + CAMERA_ZOOM_INCREMENT > CAMERA_MAX_ZOOM)
			return;

		currentCameraZoom += CAMERA_ZOOM_INCREMENT;
		foreach (var canvas in canvases)
		{
			canvas.planeDistance = -currentCameraZoom;
		}
	}

	/// <summary>
	/// Zoom out the player camera, by increments based on constant defined.
	/// </summary>
	public void ZoomOutCamera()
	{
		if (currentCameraZoom - CAMERA_ZOOM_INCREMENT < CAMERA_MIN_ZOOM)
			return;

		currentCameraZoom -= CAMERA_ZOOM_INCREMENT;
		foreach (var canvas in canvases)
		{
			canvas.planeDistance = -currentCameraZoom;
		}
	}

	/// <summary>
	/// Deals with inputs.
	/// </summary>
	void Update()
	{
		if (healthController.IsDead() && !dead)
		{
			dead = true;
			Die();
		}

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
			engine.clip = engineAudio;
			engine.volume = 0.0f;
		}
		engine.Stop();
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
			thruster.gameObject.SetActive(state);
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
		return thrusters.FirstOrDefault().gameObject.activeSelf;
	}

	/// <summary>
	/// Fade in the engine sound.
	/// </summary>
	private void PlayEngineSound()
	{
		if (!engine.isPlaying)
			engine.Play();
		engine.volume = (engine.volume < 1) ? engine.volume + movementController.GetAcceleration() : 1.0f;
	}

	/// <summary>
	/// Fade out the engine sound, and stop it once it reaches 0.
	/// </summary>
	private void StopEngineSound()
	{
		engine.volume = (engine.volume > 0) ? engine.volume - movementController.GetDeceleration() : 0.0f;
		if (engine.volume <= 0.0f)
			engine.Stop();
	}

	/// <summary>
	/// Deals with all collisions with the player character
	/// </summary>
	/// <param name="collider"></param>
	void OnTriggerEnter2D(Collider2D collider)
	{
		switch (collider.gameObject.tag)
		{
			case "Immovable":
			case "Asteroid":
			case "Mineable":
				Vector2 direction = Vector2.zero; // Direction of the collision, with the magnitude applied being the speed of the player.
												  // If the player isn't moving, we need to move them away, as they would be able to clip into the immovable otherwise.
				if (rigidBody.velocity == Vector2.zero)
				{
					direction = (gameObject.transform.position - collider.gameObject.transform.position) * 20;
				}
				// The difference here is the player has a velocity, so we will propel them away from the immovable using their velocity magnitude.
				else
				{
					if (rigidBody.velocity.magnitude > 10)
					{
						healthController.TakeDamage((int)System.Math.Floor(rigidBody.velocity.magnitude / 3));
					}
					direction = (gameObject.transform.position - collider.gameObject.transform.position).normalized * movementController.GetMaxSpeed() * 10f;
				}
				movementController.Stop();
                if(!healthController.IsDead())
				    movementController.MoveDirection(direction);
				break;

			case "Enemy":
				healthController.TakeDamage(5);
				break;
		}
	}

	/// <summary>
	/// Is the player dead?
	/// </summary>
	/// <returns></returns>
	public bool IsDead()
	{
		return dead;
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
        HealthUI.SetActive(false);
		engine.Stop();

		movingForward = false;
		rotatingLeft = false;
		rotatingRight = false;

		//spawn explosion effect.
		ParticleManager.PlayParticle(deathExplosion, gameObject);

		StartCoroutine(Respawn());

	}

	/// <summary>
	/// Respawn the player.
	/// </summary>
	/// <returns></returns>
	public IEnumerator Respawn()
	{
		if(!respawning)
		{
			respawning = true;
			// Wait before beginning the respawn animation.
			yield return new WaitForSeconds(RESPAWN_WAIT_TIME);

			// Spawn respawn effect.
			ParticleManager.PlayParticle(respawnEffect, respawnPoint);

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
            HealthUI.SetActive(true);
			respawning = false;
			healthController.ResetHealth(GetComponent<Actor>().health / 2);
		}
	}
}
