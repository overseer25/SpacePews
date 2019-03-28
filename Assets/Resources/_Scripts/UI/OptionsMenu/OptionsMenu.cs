using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public ControlsMenu controlsMenu;
    public AudioMenu audioMenu;
    public GraphicsMenu graphicsMenu;

    [Header("Sound")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Other")]
    public PauseMenuScript pauseMenu;
	public GameObject healthUI;
    public bool isOpen;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        transform.position = Vector2.zero;
    }

    private void Update()
    {
        if (isOpen && !SubmenuIsOpen() && Input.GetKeyDown(InputManager.current.controls.pause))
        {
            PlayClickSound();
            Hide();
            pauseMenu.ActivatePauseMenu();
        }
    }

    /// <summary>
    /// Initialize saved data.
    /// </summary>
    public void Initialize()
    {
        InputManager.current.LoadFromFile();
        audioMenu.LoadFromFile();
    }

    /// <summary>
    /// Is one of the sub-menus open?
    /// </summary>
    /// <returns></returns>
    public bool SubmenuIsOpen()
    {
        return controlsMenu.isOpen || audioMenu.isOpen || graphicsMenu.IsOpen();
    }

    private IEnumerator ChangeIsOpenBool()
    {
        yield return new WaitForSeconds(0.01f);
        isOpen = false;
        yield break;
    }

    /// <summary>
    /// Display the main options menu.
    /// </summary>
    public void Show()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
        this.GetComponent<Image>().enabled = true;
        isOpen = true;
		healthUI.SetActive(false);
    }

    /// <summary>
    /// Hide the main options menu.
    /// </summary>
    public void Hide()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        this.GetComponent<Image>().enabled = false;
        StartCoroutine(ChangeIsOpenBool());
		healthUI.SetActive(true);
    }

    /// <summary>
    /// Hide all parts of the options menu.
    /// </summary>
    public void HideAll()
    {
        Hide();
        controlsMenu.Hide();
        audioMenu.Hide();
        graphicsMenu.Hide();
    }

    /// <summary>
    /// Show the controls menu.
    /// </summary>
    public void ShowControls()
    {
        Hide();
        controlsMenu.Show();
    }

    /// <summary>
    /// Hide the controls menu.
    /// </summary>
    public void HideControls()
    {
        Show();
        controlsMenu.Hide();
    }

    /// <summary>
    /// Show the audio menu.
    /// </summary>
    public void ShowAudio()
    {
        Hide();
        audioMenu.Show();
    }

    /// <summary>
    /// Hide the audio menu.
    /// </summary>
    public void HideAudio()
    {
        Show();
        audioMenu.Hide();
    }

    /// <summary>
    /// Show the graphics menu.
    /// </summary>
    public void ShowGraphics()
    {
        Hide();
        graphicsMenu.Show();
    }

    /// <summary>
    /// Hide the graphics menu.
    /// </summary>
    public void HideGraphics()
    {
        Show();
        graphicsMenu.Hide();
    }

    /// <summary>
    /// Play hover sound.
    /// </summary>
    public void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverSound);
    }

    /// <summary>
    /// Play click sound.
    /// </summary>
    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

}
