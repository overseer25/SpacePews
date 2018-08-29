using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject confirmQuitDialogue;
    public Texture2D menuCursor;
    public Texture2D shootCursor;
    public Menu_Enter playerMenus;

    public KeyCode pauseKey = KeyCode.Escape;

    public static bool IsPaused { get; protected set; }


    // Use this for initialization
    private void Start()
    {
        ResumeGame();
    }

    private void LateUpdate()
    {
        bool pause = Input.GetKeyDown(pauseKey);
        //player has hit pause key
        if (pause)
        {
            //player has hit quit but has not quit yet and has the dialogue open
            if (confirmQuitDialogue.activeInHierarchy)
            {
                ChangeQuitDialogueState(false);
                ActivatePauseMenu();
            }
            else
            {
                PauseGame(!IsPaused);
            }

        }
    }

    /// <summary>
    /// Deactivates all the pieces of the pause menu.
    /// </summary>
    private void DeactivatePauseMenu()
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
    private void ActivatePauseMenu()
    {
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
        IsPaused = pause;
        if (pause)
        {
            ActivatePauseMenu();
            Cursor.SetCursor(menuCursor, Vector2.zero, CursorMode.Auto);
            playerMenus.CloseMenusForPause();
        }
        else
        {
            DeactivatePauseMenu();
            ChangeQuitDialogueState(false);
            Cursor.SetCursor(shootCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Force pause the game.
    /// </summary>
    public void PauseGame()
    {
        IsPaused = true;
        ActivatePauseMenu();
        Cursor.SetCursor(menuCursor, Vector2.zero, CursorMode.Auto);
        playerMenus.CloseMenusForPause();
    }

    /// <summary>
    /// Force resume the game. Close quit dialogue if open.
    /// </summary>
    public void ResumeGame()
    {
        IsPaused = false;
        ChangeQuitDialogueState(false);
        DeactivatePauseMenu();
        Cursor.SetCursor(shootCursor, Vector2.zero, CursorMode.Auto);
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
        //TODO: make options screen
    }

}
