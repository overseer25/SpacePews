using UnityEngine;

/// <summary>
/// The input manager deals with all input from the player. It also manages the control scheme the player has set up.
/// This class completely bypasses the built-in Unity input manager.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode fire;
    public KeyCode moveForward;
    public KeyCode turnLeft;
    public KeyCode turnRight;
    public KeyCode openInventory;
    public KeyCode submit;
    public KeyCode cancel;
    public KeyCode pause;
    public KeyCode cameraZoomIn;
    public KeyCode cameraZoomOut;
    public KeyCode suicide;

    // Contains all of the controllers needed.
    public GameObject playerShip;

    /// <summary>
    /// This is a static class, so it must be referenced by this variable.
    /// </summary>
    public static InputManager current;

    private PlayerController pController;
    private MovementController mController;
    private WeaponController wController;

    private void Start()
    {
        pController = playerShip.GetComponent<PlayerController>();
        mController = playerShip.GetComponent<MovementController>();
        wController = playerShip.GetComponent<WeaponController>();
        if (current == null)
            current = this;
        else
            Debug.Log("An Input Manager already exists. Please check that only one Input Manager script exists in the scene.");
    }

    // Update is called once per frame
    void Update()
    {
        // Suicide button.
        if (Input.GetKeyDown(suicide) && !pController.itemTransferConfirmWindow.activeInHierarchy)
            pController.Kill();

        HandleMovementControls();
        HandleWeaponControls();
        HandleUIControls();
    }

    /// <summary>
    /// Handles the movement controls of the player.
    /// </summary>
    private void HandleMovementControls()
    {
        if ((!Input.GetKeyDown(moveForward) && Input.GetKey(moveForward)) || (Input.GetKeyDown(moveForward)))
        {
            pController.movingForward = true;
            if (!pController.GetThrusterState())
                pController.SetThrusterState(true);
        }
        else if (Input.GetKeyUp(moveForward))
        {
            pController.movingForward = false;
            if (pController.GetThrusterState())
                pController.SetThrusterState(false);
        }

        if (Input.GetKeyDown(turnRight))
        {
            pController.rotatingRight = true;
            pController.rotatingLeft = false;
        }
        else if (Input.GetKeyUp(turnRight))
        {
            pController.rotatingRight = false;
        }
        if (Input.GetKeyDown(turnLeft))
        {
            pController.rotatingRight = false;
            pController.rotatingLeft = true;
        }
        else if (Input.GetKeyUp(turnLeft))
        {
            pController.rotatingLeft = false;
        }
    }

    /// <summary>
    /// Handles controls for weapons.
    /// </summary>
    private void HandleWeaponControls()
    {
        if (!pController.IsDead())
        {
            if (wController.currentComponent is ChargedWeapon)
            {
                HandleChargedWeapon(wController.currentComponent as ChargedWeapon);
            }
            else if (wController.currentComponent is AutomaticWeapon)
            {
                if (Input.GetMouseButton(0) && !wController.menuOpen && wController.currentComponent != null)
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
        if (Input.GetKey(fire) && wController.currentComponent != null && !weapon.IsCoolingDown() && !weapon.IsDecharging())
        {
            if (!wController.menuOpen)
                weapon.CheckFire();
            else
                weapon.CancelFire();
        }
        else if (Input.GetKeyUp(fire) && wController.currentComponent != null && !weapon.IsCoolingDown() && !weapon.IsDecharging())
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
        if (Input.GetKey(fire) && !wController.menuOpen && wController.currentComponent != null)
        {
            UpdateMiningFire(miningComponent);
        }
        // What to do if a menu opens.
        if (Input.GetKey(fire) || wController.menuOpen || wController.currentComponent == null)
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
    /// Handles controls for interacting with the UI.
    /// </summary>
    private void HandleUIControls()
    {
        if (Input.GetKeyDown(openInventory) && !pController.itemTransferConfirmWindow.activeInHierarchy)
        {
            wController.menuOpen = !wController.menuOpen;
            pController.inventory.Toggle();
            pController.inventory.infoScreen.Hide();
        }

        if (Input.GetKeyDown(cameraZoomIn))
            pController.ZoomInCamera();
        if (Input.GetKeyDown(cameraZoomOut))
            pController.ZoomOutCamera();

        // Handle closing out the inventory item transfer window with the cancel button.
        if (Input.GetKeyDown(cancel) && pController.inventory.itemTransferPanel.activeInHierarchy)
        {
            pController.inventory.TransferItemCancelClick();
        }
        // Handle pause screen.
        else if (!pController.IsDead() && !pController.inventory.itemTransferPanel.activeInHierarchy)
        {
            if (Input.GetKeyDown(pause))
            {
                pController.pauseMenu.PauseGame(!PauseMenuScript.IsPaused);
            }
        }
    }
}
