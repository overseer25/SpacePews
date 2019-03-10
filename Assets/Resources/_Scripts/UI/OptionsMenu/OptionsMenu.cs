using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public ControlsMenu controlsMenu;

    [Header("Sound")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Other")]
    public PauseMenuScript pauseMenu;
    public bool isOpen;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(InputManager.current.controls.pause))
        {
            Hide();
            pauseMenu.ActivatePauseMenu();
        }
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
        isOpen = false;
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
