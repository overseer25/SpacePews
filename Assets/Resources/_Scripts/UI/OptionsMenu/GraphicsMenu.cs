using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip clickSound;

    [Header("Video Settings")]
    public Dropdown resolutionSelection;
    public Dropdown aspectSelection;
    public Dropdown framerateSelection;

    [Header("Other")]
    public OptionsMenu optionsMenu;
    public bool isOpen;

    private AudioSource audioSource;

    private Resolution previousResolution;
    private string previousAspectRatio;
    private int previousRefreshRate;

    private List<int> refreshRates;
    private List<string> aspectRatios;
    private List<Resolution> resolutions;

    // Variables for selections.
    private bool vsyncEnabled;
    private int selectedFramerate;
    private int selectedResolution;
    private int selectedAspectRatio;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        InitializeDropdownMenus();
    }

    private void LateUpdate()
    {
        if (!Input.GetMouseButton(0) && Input.GetKeyDown(InputManager.current.controls.pause) && isOpen)
        {
            PlayClickSound();
            optionsMenu.HideAudio();
        }
    }

    /// <summary>
    /// After making changes to the resolution, ask the user to confirm. After a set amount of time,
    /// the resolution will reset to the previous value. This is to prevent the user from using a resolution
    /// that doesn't work well with their monitor and the game.
    /// </summary>
    public void AskForConfirmation()
    {

    }

    /// <summary>
    /// Set the aspect ratio selection.
    /// </summary>
    /// <param name="selection"></param>
    public void SetAspectRatio(int selection)
    {
        selectedAspectRatio = selection;
    }

    /// <summary>
    /// Set the resolution selection.
    /// </summary>
    /// <param name="selection"></param>
    public void SetResolution(int selection)
    {
        selectedResolution = selection;
    }

    /// <summary>
    /// Update the list of resolutions the user can choose from, based on the aspect ratio selected.
    /// </summary>
    public void UpdateResolutionDropdown()
    {
        if (resolutions != null)
            previousResolution = resolutions[selectedResolution];

        resolutionSelection.options = new List<Dropdown.OptionData>();
        List<string> resolutionStrings = new List<string>();
        resolutions = new List<Resolution>();
        Dropdown.OptionData resOption;

        foreach (var resolution in Screen.resolutions)
        {
            resOption = new Dropdown.OptionData(resolution.width + "x" + resolution.height);

            string resolutionAspect = GetAspectRatio((float)resolution.width / resolution.height);
            // Make sure the resolutions being displayed are of the appropriate aspect ratio.
            if (!resolutionStrings.Contains(resOption.text) && resolutionAspect == aspectRatios[selectedAspectRatio])
            {
                resolutionSelection.options.Add(resOption);
                resolutionStrings.Add(resOption.text);
                resolutions.Add(resolution);
            }
        }

        selectedResolution = 0;
        resolutionSelection.value = selectedResolution;
        resolutionSelection.captionText.text = resolutionSelection.options[selectedResolution].text;

        UpdateFramerateDropdown();
    }

    /// <summary>
    /// Set the framerate selection.
    /// </summary>
    /// <param name="val"></param>
    public void SetFrameRate(int selection)
    {
        selectedFramerate = selection;
    }

    public void UpdateFramerateDropdown()
    {
        if(refreshRates != null)
            previousRefreshRate = refreshRates[selectedFramerate];

        framerateSelection.options = new List<Dropdown.OptionData>();
        refreshRates = new List<int>();

        foreach (var resolution in Screen.resolutions)
        {
            string resolutionAspect = GetAspectRatio((float)resolution.width / resolution.height);
            if (!refreshRates.Contains(resolution.refreshRate) && resolutionAspect == aspectRatios[selectedAspectRatio])
            {
                framerateSelection.options.Add(new Dropdown.OptionData(resolution.refreshRate + " hz"));
                refreshRates.Add(resolution.refreshRate);
            }
        }
        var index = refreshRates.IndexOf(previousRefreshRate);

        if (index > 0)
            selectedFramerate = index;
        else
            selectedFramerate = 0;

        framerateSelection.value = selectedFramerate;
        framerateSelection.captionText.text = framerateSelection.options[selectedFramerate].text;
    }

    /// <summary>
    /// Toggle VSync.
    /// </summary>
    /// <param name="val"></param>
    public void ToggleVsync(bool val)
    {
        if (!val)
            vsyncEnabled = false;
        else
            vsyncEnabled = true;
    }

    /// <summary>
    /// Disable or Enable the frame rate slider, based on the incoming value.
    /// </summary>
    /// <param name="val"></param>
    public void SetDisableFramerateSlider(bool val)
    {
        if(!val)
        {
            resolutionSelection.enabled = true;
            foreach (var childImage in resolutionSelection.GetComponentsInChildren<Image>())
                childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, 1.0f);
        }
        if(val)
        {
            resolutionSelection.enabled = false;
            foreach (var childImage in resolutionSelection.GetComponentsInChildren<Image>())
                childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, 0.5f);
        }
    }

    /// <summary>
    /// Apply all the changes the user has made.
    /// </summary>
    public void ApplyChanges()
    {
        if (vsyncEnabled)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;

        Screen.SetResolution(resolutions[selectedResolution].width, resolutions[selectedResolution].height, FullScreenMode.ExclusiveFullScreen, refreshRates[selectedFramerate]);

    }

    public void Show()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
        isOpen = true;
    }

    public void Hide()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        isOpen = false;
    }

    /// <summary>
    /// Plays the control menu click sound.
    /// </summary>
    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickSound);
    }

    /// <summary>
    /// Sets up the framerate slider.
    /// </summary>
    private void InitializeDropdownMenus()
    {
        //TODO: Logic for value loaded from file.
        if (aspectRatios != null)
            previousAspectRatio = aspectRatios[selectedAspectRatio];

        aspectSelection.options = new List<Dropdown.OptionData>();
        aspectRatios = new List<string>();
        
        foreach (var resolution in Screen.resolutions)
        {
            var aspect = (float)resolution.width / resolution.height;
            var aspectString = GetAspectRatio(aspect);

            if (!aspectRatios.Contains(aspectString))
            {
                aspectRatios.Add(aspectString);
                aspectSelection.options.Add(new Dropdown.OptionData(aspectString));
            }
        }

        UpdateResolutionDropdown();
    }

    private string GetAspectRatio(float val)
    {
        if (val >= 3.5f)
            return "32:9";
        if (val >= 2.3f)
            return "21:9";
        else if (val >= 1.7f)
            return "16:9";
        else if (val >= 1.6f)
            return "16:10";
        else if (val >= 1.3f)
            return "4:3";
        else
            return "5:4";
    }
}
