using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GraphicsMenu : MonoBehaviour
{
    [Header("Video Settings")]
    public Dropdown resolutionSelection;
    public Dropdown aspectSelection;
    public Dropdown refreshRateSelection;
    public Toggle vsyncToggle;
    public Slider itemCountSlider;
    public TextMeshProUGUI itemCountText;
    public Slider effectCountSlider;
    public TextMeshProUGUI effectCountText;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioClip hoverSound;
    public AudioClip saveChangesPopUpSound;

    [Header("Other")]
    public OptionsMenu optionsMenu;
    public GameObject confirmationWindow;
    public Button applyButton;

    private bool isOpen;
    private bool isConfirmationOpen;

    private AudioSource audioSource;

    // These are the selections that are stored in the file.
    private int previousResolution;
    private int previousAspectRatio;
    private int previousRefreshRate;
    private int previousItemCount;
    private int previousEffectCount;
    private bool previousVsync;

    private List<int> refreshRates;
    private List<string> aspectRatios;
    private List<Resolution> resolutions;

    // Variables for selections.
    private bool vsyncEnabled;
    private int selectedRefreshRate;
    private int selectedResolution;
    private int selectedAspectRatio;
    private int itemCount;
    private int effectCount;

    private string fileLocation;
    private bool firstLoad = true;

    // Track whether options have been changed. One of these must be true for the save changes dialog window to display.
    private bool changedResolution = false;
    private bool changedAspectRatio = false;
    private bool changedRefreshRate = false;
    private bool changedItemCount = false;
    private bool changedEffectCount = false;
    private bool changedVsync = false;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (vsyncToggle.isOn)
            SetDisableFramerateSlider(true);


        fileLocation = Application.persistentDataPath + "/graphics.json";
        LoadFromFile();
        transform.position = Vector2.zero;
        confirmationWindow.transform.position = Vector2.zero;
        HideConfirmation();
    }

    private void LateUpdate()
    {
        if (!Input.GetMouseButton(0) && Input.GetKeyDown(InputManager.current.controls.pause) && isOpen)
        {
            if(ChangesMade())
            {
                Hide();
                DisplayConfirmation();
            }
            else
            {
                PlayClickSound();
                optionsMenu.HideGraphics();
            }
        }
        if (ChangesMade())
            applyButton.interactable = true;
        else
            applyButton.interactable = false;

    }

    /// <summary>
    /// Is any element of this menu open?
    /// </summary>
    /// <returns></returns>
    public bool IsOpen()
    {
        return isOpen || isConfirmationOpen;
    }

    /// <summary>
    /// Set the aspect ratio selection.
    /// </summary>
    /// <param name="selection"></param>
    public void SetAspectRatio(int selection)
    {
        if (selection == previousAspectRatio)
            changedAspectRatio = false;
        else
            changedAspectRatio = true;

        selectedAspectRatio = selection;
    }

    /// <summary>
    /// Update the list of aspect ratios available.
    /// </summary>
    public void UpdateAspectRatioDropdown()
    {
        aspectRatios = new List<string>();
        aspectSelection.options = new List<Dropdown.OptionData>();
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
        aspectSelection.value = selectedAspectRatio;
        aspectSelection.captionText.text = aspectRatios[selectedAspectRatio];
        UpdateResolutionDropdown();
    }

    /// <summary>
    /// Set the resolution selection.
    /// </summary>
    /// <param name="selection"></param>
    public void SetResolution(int selection)
    {
        if (selection == previousResolution)
            changedResolution = false;
        else
            changedResolution = true;

        selectedResolution = selection;
    }

    /// <summary>
    /// Update the list of resolutions the user can choose from, based on the aspect ratio selected.
    /// </summary>
    public void UpdateResolutionDropdown()
    {
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
        if(previousAspectRatio != selectedAspectRatio)
        {
            selectedResolution = 0;
        }
        resolutionSelection.value = selectedResolution;
        resolutionSelection.captionText.text = resolutionStrings[selectedResolution];
        UpdateFramerateDropdown();
    }

    /// <summary>
    /// Set the framerate selection.
    /// </summary>
    /// <param name="val"></param>
    public void SetRefreshRate(int selection)
    {
        if(selection == previousRefreshRate)
            changedRefreshRate = false;
        else
            changedRefreshRate = true;

        selectedRefreshRate = selection;
    }

    /// <summary>
    /// Update the list of framerates the user can choose from.
    /// </summary>
    public void UpdateFramerateDropdown()
    {
        refreshRateSelection.options = new List<Dropdown.OptionData>();
        refreshRates = new List<int>();
        var curRefreshRate = refreshRateSelection.captionText.text;

        foreach (var resolution in Screen.resolutions)
        {
            string resolutionAspect = GetAspectRatio((float)resolution.width / resolution.height);
            if (!refreshRates.Contains(resolution.refreshRate) && resolutionAspect == aspectRatios[selectedAspectRatio])
            {
                refreshRateSelection.options.Add(new Dropdown.OptionData(resolution.refreshRate + " hz"));
                refreshRates.Add(resolution.refreshRate);
            }
        }
        if(refreshRateSelection.options.Select(e => e.text).Contains(curRefreshRate))
        {
            selectedRefreshRate = 0;
        }

        refreshRateSelection.value = selectedRefreshRate;
        refreshRateSelection.captionText.text = refreshRateSelection.options[selectedRefreshRate].text;
    }

    /// <summary>
    /// Toggle VSync.
    /// </summary>
    /// <param name="val"></param>
    public void ToggleVsync(bool val)
    {
        if (val == previousVsync)
            changedVsync = false;
        else
            changedVsync = true;

        vsyncEnabled = val;
    }

    /// <summary>
    /// Have changes been made?
    /// </summary>
    /// <returns></returns>
    public bool ChangesMade()
    {
        return changedAspectRatio || changedResolution || changedRefreshRate || changedItemCount || changedEffectCount || changedVsync;
    }

    /// <summary>
    /// Update the item count and the text on the UI.
    /// </summary>
    public void UpdateItemCount()
    {
        itemCount = (int)itemCountSlider.value;
        itemCountText.text = itemCount.ToString();

        if (itemCount == previousItemCount)
            changedItemCount = false;
        else
            changedItemCount = true;
    }

    /// <summary>
    /// Update the effect count and the text on the UI.
    /// </summary>
    public void UpdateEffectCount()
    {
        effectCount = (int)effectCountSlider.value;
        effectCountText.text = effectCount.ToString();

        if (effectCount == previousEffectCount)
            changedEffectCount = false;
        else
            changedEffectCount = true;
    }

    /// <summary>
    /// Disable or Enable the frame rate slider, based on the incoming value.
    /// </summary>
    /// <param name="val"></param>
    public void SetDisableFramerateSlider(bool val)
    {
        if (!val)
        {
            refreshRateSelection.enabled = true;
            foreach (var childImage in refreshRateSelection.GetComponentsInChildren<Image>())
                childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, 1.0f);
        }
        if (val)
        {
            refreshRateSelection.enabled = false;
            foreach (var childImage in refreshRateSelection.GetComponentsInChildren<Image>())
                childImage.color = new Color(childImage.color.r, childImage.color.g, childImage.color.b, 0.5f);
        }
    }

    /// <summary>
    /// After making changes to the resolution/aspect ratio/refresh rate, ask the user to confirm. After a set amount of time,
    /// the resolution will reset to the previous value. This is to prevent the user from using a resolution
    /// that doesn't work well with their monitor and the game.
    /// </summary>
    public void DisplayConfirmation()
    {
        if (ChangesMade())
        {
            confirmationWindow.SetActive(true);
            isConfirmationOpen = true;
            Hide();
            PlayPopUpSound();
        }
        else
            optionsMenu.HideGraphics();
    }

    /// <summary>
    /// Hide the apply changes confirmation window.
    /// </summary>
    public void HideConfirmation()
    {
        confirmationWindow.SetActive(false);
        isConfirmationOpen = false;
    }

    /// <summary>
    /// Apply all the changes the user has made.
    /// </summary>
    public void ApplyChanges()
    {
        if(changedVsync || firstLoad)
        {
            if (vsyncEnabled)
                QualitySettings.vSyncCount = 1;
            else
                QualitySettings.vSyncCount = 0;
        }

        if(changedResolution || changedRefreshRate || firstLoad)
        {
            Screen.SetResolution(resolutions[selectedResolution].width, resolutions[selectedResolution].height, FullScreenMode.ExclusiveFullScreen, refreshRates[selectedRefreshRate]);
            Application.targetFrameRate = refreshRates[selectedRefreshRate];
        }

		if (changedItemCount || firstLoad)
			ItemPool.current.SetPoolSize(itemCount);
		if (changedEffectCount || firstLoad)
			ParticlePool.current.SetPoolSize(effectCount);

        firstLoad = false;

        changedAspectRatio = false;
        changedResolution = false;
        changedRefreshRate = false;
        changedItemCount = false;
        changedEffectCount = false;
        changedVsync = false;
    }

    /// <summary>
    /// If the user doesn't want to save their changes, revert to what is on file.
    /// </summary>
    public void CancelChanges()
    {
        changedAspectRatio = false;
        changedResolution = false;
        changedRefreshRate = false;
        changedItemCount = false;
        changedEffectCount = false;
        changedVsync = false;

        LoadFromFile();
    }

    /// <summary>
    /// Reset the graphics settings to their default values.
    /// </summary>
    /// <returns></returns>
    public GraphicsData ResetToDefault()
    {
        vsyncEnabled = false;
        selectedAspectRatio = 0;
        selectedResolution = 0;
        selectedRefreshRate = 0;
        itemCount = 64;
        effectCount = 64;

        var data = new GraphicsData()
        {
            vsync = vsyncEnabled,
            aspectRatio = selectedAspectRatio,
            resolution = selectedResolution,
            framerate = selectedRefreshRate,
            itemCount = itemCount,
            effectCount = effectCount
        };

        SaveToFile();
        return data;
    }

    /// <summary>
    /// Saves the settings to file.
    /// </summary>
    public void SaveToFile()
    {
        var data = new GraphicsData()
        {
            vsync = vsyncEnabled,
            aspectRatio = selectedAspectRatio,
            resolution = selectedResolution,
            framerate = selectedRefreshRate,
            itemCount = itemCount,
            effectCount = effectCount
        };

        var json = JsonUtility.ToJson(data);

        try
        {
            using (FileStream stream = File.Create(fileLocation))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(json);
                }
            }
        }
        catch (Exception)
        {
            Debug.Log("Failed to save graphics file " + fileLocation + ", reverting to previously saved values.");
            LoadFromFile();
        }
        finally
        {
            UpdateSettings(data);
            ApplyChanges();
        }
    }

    /// <summary>
    /// Loads the settings from file.
    /// </summary>
    public void LoadFromFile()
    {
        GraphicsData data = null;
        try
        {
            if (File.Exists(fileLocation))
            {
                using (var stream = File.Open(fileLocation, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        data = JsonUtility.FromJson<GraphicsData>(json);
                    }
                }
            }
            else
            {
                data = ResetToDefault();
                SaveToFile();
            }
        }
        catch (Exception)
        {
            Debug.Log("Failed to load graphics file " + fileLocation + ", using default values instead.");
            data = ResetToDefault();
        }
        finally
        {
            UpdateSettings(data);
            ApplyChanges();
        }
    }

    private void UpdateSettings(GraphicsData data)
    {
        vsyncEnabled = data.vsync;
        selectedAspectRatio = data.aspectRatio;
        selectedResolution = data.resolution;
        selectedRefreshRate = data.framerate;
        itemCount = data.itemCount;
        effectCount = data.effectCount;

        previousAspectRatio = selectedAspectRatio;
        previousResolution = selectedResolution;
        previousRefreshRate = selectedRefreshRate;
        previousItemCount = itemCount;
        previousEffectCount = effectCount;
        previousVsync = vsyncEnabled;

        UpdateAspectRatioDropdown();

        itemCountSlider.value = itemCount;
        itemCountText.text = itemCount.ToString();
        effectCountSlider.value = effectCount;
        effectCountText.text = effectCount.ToString();
        vsyncToggle.isOn = vsyncEnabled;
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
    /// Plays the hover sound.
    /// </summary>
    public void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverSound);
    }

    /// <summary>
    /// Plays the pop up sound for the save changes window.
    /// </summary>
    public void PlayPopUpSound()
    {
        audioSource.PlayOneShot(saveChangesPopUpSound);
    }

    /// <summary>
    /// Handles the event that the input hovers over the apply button.
    /// </summary>
    public void ApplyButtonHoverEvent()
    {
        if (applyButton.interactable)
            PlayHoverSound();
    }

    /// <summary>
    /// Handles the event that the input hovers over the apply button.
    /// </summary>
    public void ApplyButtonClickEvent()
    {
        if (applyButton.interactable)
            PlayClickSound();
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
