using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public OptionsMenu optionsMenu;
    public GameObject confirmQuitDialogue;
	public OutputWindow outputWindow;
    public Texture2D menuCursor;
    public Texture2D shootCursor;
	public GameObject healthUI;


    [SerializeField]
    private Inventory inventory;
    [SerializeField]
    private WeaponController weaponController;
    [Header("Audio")]
    [SerializeField]
    private AudioClip buttonHoverSound;
    [SerializeField]
    private AudioClip buttonClickSound;
    [SerializeField]
    private AudioClip pauseSound;
    [SerializeField]
    private AudioClip resumeSound;
    private AudioSource source;

    public static bool IsPaused { get; protected set; }
    private bool dead = false;

    // Use this for initialization
    private void Start()
    {
        source = GetComponent<AudioSource>();
        optionsMenu.Initialize();
        ResumeGame();
    }

    /// <summary>
    /// Update to the death state of the player.
    /// </summary>
    /// <param name="isDead"></param>
    public void UpdateDead(bool isDead)
    {
        dead = isDead;
    }

    /// <summary>
    /// Deactivates all the pieces of the pause menu.
    /// </summary>
    public void DeactivatePauseMenu()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        this.GetComponent<Image>().enabled = false;
    }

    /// <summary>
    /// Activates all the pieces of the pause menu.
    /// </summary>
    public void ActivatePauseMenu()
    {
        ChangeQuitDialogueState(false);
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
        this.GetComponent<Image>().enabled = true;
    }

    /// <summary>
    /// Activates or deactivates the quit dialogue.
    /// </summary>
    /// <param name="active"></param>
    private void ChangeQuitDialogueState(bool active)
    {
        confirmQuitDialogue.SetActive(active);
    }

    /// <summary>
    /// Pauses the player's game. Currently this brings up pause screen buttons
    /// and prevents the player from shooting and opening other menus, but not from dying
    /// or moving.
    /// </summary>
    /// <param name="pause">If this is true, the game will be paused, if false it will resume.</param>
    public void PauseGame(bool pause)
    {
        if (pause)
        {
            source.PlayOneShot(pauseSound);
            PauseGame();
        }
        else
        {
            source.PlayOneShot(resumeSound);
            ResumeGame();
        }
    }

    /// <summary>
    /// Force pause the game.
    /// </summary>
    public void PauseGame()
    {
		healthUI.SetActive(false);
		IsPaused = true;
        ActivatePauseMenu();
        Cursor.SetCursor(menuCursor, Vector2.zero, CursorMode.Auto);
		outputWindow.Hide();
        weaponController.menuOpen = true;
        inventory.UpdatePaused(true);
    }

    /// <summary>
    /// Force resume the game. Close quit dialogue if open.
    /// </summary>
    public void ResumeGame()
    {
		healthUI.SetActive(true);
		IsPaused = false;
        ChangeQuitDialogueState(false);
        DeactivatePauseMenu();
        optionsMenu.HideAll();
		outputWindow.Show();
        Cursor.SetCursor(shootCursor, new Vector2(32, 32), CursorMode.Auto);
        weaponController.menuOpen = false;
        inventory.UpdatePaused(false);
    }

    /// <summary>
    /// Open the quit dialogue and deactivate pause menu.
    /// </summary>
    public void OpenQuitDialogue()
    {
        ChangeQuitDialogueState(true);
        DeactivatePauseMenu();
    }

    /// <summary>
    /// Close quit dialogue and bring back pause menu.
    /// </summary>
    public void CloseQuitDialogue()
    {
        ChangeQuitDialogueState(false);
        ActivatePauseMenu();
    }

    /// <summary>
    /// Actually quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Bring up the options the player may set.
    /// </summary>
    public void OpenOptions()
    {
        DeactivatePauseMenu();
        optionsMenu.Show();
    }

    /// <summary>
    /// Hide the options menu, and return to the main pause screen.
    /// </summary>
    public void CloseOptions()
    {
        ActivatePauseMenu();
        optionsMenu.Hide();
	}

    /// <summary>
    /// Play the hover sound. Used by the Unity Event System.
    /// </summary>
    public void PlayHoverSound()
    {
        source.PlayOneShot(buttonHoverSound);
    }

    /// <summary>
    /// Play the click sound. Used by the Unity Event System.
    /// </summary>
    public void PlayClickSound()
    {
        source.PlayOneShot(buttonClickSound);
    }

}
