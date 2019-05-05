﻿using System;
using System.IO;
using UnityEngine;

/// <summary>
/// The input manager deals with all input from the player. It also manages the control scheme the player has set up.
/// This class completely bypasses the built-in Unity input manager.
/// </summary>
public class InputManager : MonoBehaviour
{
    public static Controls controls;

    [Header("Other")]
    // Contains all of the controllers needed.
    public GameObject playerShip;
    public OptionsMenu optionsMenu;
    public ControlsMenu controlsMenu;

    /// <summary>
    /// This is a static class, so it must be referenced by this variable.
    /// </summary>
    public static InputManager current;

    public bool inMainMenu = false;

    private string fileLocation;

    private PlayerController pController;
	private PlayerHealthController hController;
    private MovementController mController;
    private WeaponController wController;

	private void Awake()
    {
        if (current == null)
            current = this;
        else
            Debug.Log("An Input Manager already exists. Please check that only one Input Manager script exists in the scene.");

        if(!inMainMenu)
        {
            pController = playerShip.GetComponent<PlayerController>();
            hController = playerShip.GetComponent<PlayerHealthController>();
            mController = playerShip.GetComponent<MovementController>();
            wController = playerShip.GetComponent<WeaponController>();
        }
        fileLocation = Application.persistentDataPath + "/controls.json";
    }

    // Update is called once per frame
    void Update()
    {
        if(!inMainMenu)
        {
            if (!optionsMenu.isOpen && !optionsMenu.SubmenuIsOpen() && !hController.IsDead())
            {
                HandleMovementControls();
                HandleWeaponControls();

                if (!PauseMenuScript.IsPaused)
                {
                    // Suicide button.
                    if (Input.GetKeyDown(controls.suicide) && !pController.itemTransferConfirmWindow.activeInHierarchy)
                        hController.Kill();
                }
                HandleUIControls();

                // Handle pause menu.
                if (!pController.inventory.itemTransferPanel.activeInHierarchy)
                {
                    if (Input.GetKeyDown(controls.pause))
                    {
                        pController.pauseMenu.PauseGame(!PauseMenuScript.IsPaused);
                    }
                }

                HandleAbilityControls();
            }
            else
            {
                pController.StopAllMovement();
            }
        }   
    }

    /// <summary>
    /// Reset controls to defaults, which are defined in this method.
    /// </summary>
    public void ResetToDefault()
    {
		controls = new Controls();
    }

    /// <summary>
    /// Update the inputs from a file. If loading fails, the game will use the defaults.
    /// </summary>
    public void LoadFromFile()
    {
        try
        {
            if(File.Exists(fileLocation))
            {
                var file = File.ReadAllText(fileLocation);
                controls = JsonUtility.FromJson<Controls>(file);
			}
			else
			{
				ResetToDefault();
				SaveToFile();
			}

		}
        catch (Exception)
        {
            Debug.Log("Failed to load controls file " + fileLocation + ", using default values instead.");
            ResetToDefault();
        }
        finally
        {
            controlsMenu.UpdateControlWindow();
        }
    }

    /// <summary>
    /// Save the inputs to a file. This keeps the control preferences consistent between loads.
    /// </summary>
    public void SaveToFile()
    {
        try
        {
            var file = JsonUtility.ToJson(controls);
            if(!File.Exists(fileLocation))
                File.Create(fileLocation);

            File.WriteAllText(fileLocation, file);
        }
        catch (Exception)
        {
            Debug.Log("Failed to save controls file " + fileLocation + ", reverting to previously saved values.");
            LoadFromFile();
        }
        controlsMenu.UpdateControlWindow();
    }

    /// <summary>
    /// Handles the movement controls of the player.
    /// </summary>
    private void HandleMovementControls()
    {
		if (hController.IsDead())
			return;

        if ((!Input.GetKeyDown(controls.forward) && Input.GetKey(controls.forward)) || (Input.GetKeyDown(controls.forward)))
        {
            pController.movingForward = true;
            if (!pController.GetThrusterState())
                pController.SetThrusterState(true);
        }
        else if (Input.GetKeyUp(controls.forward))
        {
            pController.movingForward = false;
            if (pController.GetThrusterState())
                pController.SetThrusterState(false);
        }

        if (Input.GetKeyDown(controls.right))
        {
            pController.rotatingRight = true;
            pController.rotatingLeft = false;
        }
        else if (Input.GetKeyUp(controls.right))
        {
            pController.rotatingRight = false;
        }
        if (Input.GetKeyDown(controls.left))
        {
            pController.rotatingRight = false;
            pController.rotatingLeft = true;
        }
        else if (Input.GetKeyUp(controls.left))
        {
            pController.rotatingLeft = false;
        }
    }

    /// <summary>
    /// Handles controls and effects for abilities
    /// </summary>
    private void HandleAbilityControls()
    {
        var ability = pController.GetAbility();
        // Handle abilities.
        if (Input.GetKeyDown(controls.ability) && ability != null && !ability.recharging)
        {
            ability.Activate(pController.gameObject);
            if(ability is DashAbility)
            {
                var dash = ability as DashAbility;
                StartCoroutine(pController.Ghost(dash.ghostRate, dash.ghostFadeRate, dash.ghostFadeAmount, dash.ghostCount));
            }
            pController.StartAbilityCooldown();
        }
    }

    /// <summary>
    /// Handles controls for weapons.
    /// </summary>
    private void HandleWeaponControls()
    {
        if (!hController.IsDead())
        {
            if (wController.currentComponent is ChargedWeapon)
            {
                HandleChargedWeapon(wController.currentComponent as ChargedWeapon);
            }
            else if (wController.currentComponent is AutomaticWeapon)
            {
                if (Input.GetKey(controls.fire) && !wController.menuOpen && wController.currentComponent != null)
                {
                    (wController.currentComponent as AutomaticWeapon).CheckFire();
                }
            }
            else if (wController.currentComponent is MiningComponent)
            {
                HandleMiningTool(wController.currentComponent as MiningComponent);
            }
        }
    }

    /// <summary>
    /// Handle the functionality of the charged weapon equipped.
    /// </summary>
    /// <param name="weapon"></param>
    private void HandleChargedWeapon(ChargedWeapon weapon)
    {
        if (Input.GetKey(controls.fire) && !weapon.IsCoolingDown() && !weapon.IsDecharging())
        {
            if (!wController.menuOpen)
                weapon.CheckFire();
            else
                weapon.CancelFire();
        }
        else if (Input.GetKeyUp(controls.fire))
        {
            if (weapon.IsCharged())
            {
                weapon.Fire();
            }
            else
            {
                weapon.CancelFire();
            }
        }
        else if (weapon.IsCoolingDown())
        {
            weapon.StartCooldown();
        }
        else if (weapon.IsDecharging())
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
        if (Input.GetKey(controls.fire) && !wController.menuOpen)
        {
            miningComponent.Fire();
        }
        // What to do if a menu opens.
        if (!Input.GetKey(controls.fire) || wController.menuOpen)
        {
            miningComponent.StopFire();
        }
    }

    /// <summary>
    /// Handles controls for interacting with the UI.
    /// </summary>
    private void HandleUIControls()
    {
        if (Input.GetKeyDown(controls.inventory) && !pController.itemTransferConfirmWindow.activeInHierarchy)
        {
            wController.menuOpen = !wController.menuOpen;
            pController.inventory.Toggle();
			InfoScreen.current.Hide();
        }

        if (Input.GetKeyDown(controls.cameraZoomIn))
            pController.ZoomInCamera();
        if (Input.GetKeyDown(controls.cameraZoomOut))
            pController.ZoomOutCamera();

        // Handle closing out the inventory item transfer window with the cancel button.
        if (Input.GetKeyDown(controls.pause) && pController.inventory.itemTransferPanel.activeInHierarchy)
        {
            pController.inventory.TransferItemCancelClick();
        }
    }
}
