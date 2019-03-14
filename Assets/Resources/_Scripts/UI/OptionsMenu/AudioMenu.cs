using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMenu : MonoBehaviour
{
    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Sliders")]
    public Slider masterSlider;
    public Slider soundEffectsSlider;
    public Slider musicSlider;
    public Slider interfaceSlider;

    [Header("Audio")]
    public AudioClip clickSound;
    public AudioClip hoverSound;

    public AudioSource masterSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;

    [Header("Other")]
    public OptionsMenu optionsMenu;
    public bool isOpen;

    private string fileLocation;

    private void Awake()
    {
        masterSource = GetComponent<AudioSource>();
        fileLocation = Application.persistentDataPath + "/audio.json";
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
    /// Highlight the handle of a slider. Usually called on hover.
    /// </summary>
    /// <param name="slider"></param>
    public void HighlightSlider(Image slider)
    {
        slider.color = new Color(slider.color.r, slider.color.g, slider.color.b, 1.0f);
    }

    /// <summary>
    /// Dehighlight the handle of a slider. Usually called on hover.
    /// </summary>
    /// <param name="slider"></param>
    public void DehighlightSlider(Image slider)
    {
        slider.color = new Color(slider.color.r, slider.color.g, slider.color.b, 0.7f);
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

    public void PlayMasterSound()
    {
        masterSource.Play();
    }

    public void PlaySFXClick()
    {
        sfxSource.Play();
    }

    public void PlayUIClick()
    {
        uiSource.Play();
    }

    /// <summary>
    /// Plays the control menu hover sound.
    /// </summary>
    public void PlayHoverSound()
    {
        uiSource.PlayOneShot(hoverSound);
    }

    /// <summary>
    /// Plays the control menu click sound.
    /// </summary>
    public void PlayClickSound()
    {
        uiSource.PlayOneShot(clickSound);
    }

    /// <summary>
    /// Called when the value of any of the sliders is changed.
    /// </summary>
    public void UpdateAudio()
    {
        mixer.SetFloat("masterVolume", masterSlider.value);
        mixer.SetFloat("sfxVolume", soundEffectsSlider.value);
        mixer.SetFloat("musicVolume", musicSlider.value);
        mixer.SetFloat("uiVolume", interfaceSlider.value);
    }

    /// <summary>
    /// Resets the audio settings to their default values.
    /// </summary>
    public AudioData ResetToDefault()
    {
        var data = new AudioData()
        {
            master = 0,
            sfx = -20,
            music = -20,
            ui = -20
        };
        masterSlider.value = data.master;
        soundEffectsSlider.value = data.sfx;
        musicSlider.value = data.music;
        interfaceSlider.value = data.ui;

        UpdateAudio();
        return data;
    }

    /// <summary>
    /// Update the values of the sliders. Usually occurs when the saved audio data is loaded.
    /// </summary>
    private void UpdateSliders(AudioData data)
    {
        masterSlider.value = data.master;
        soundEffectsSlider.value = data.sfx;
        musicSlider.value = data.music;
        interfaceSlider.value = data.ui;
    }

    /// <summary>
    /// Saves the audio settings to a file in the game directory.
    /// </summary>
    /// <returns></returns>
    public void SaveToFile()
    {
        var data = new AudioData()
        {
            master = masterSlider.value,
            sfx = soundEffectsSlider.value,
            music = musicSlider.value,
            ui = interfaceSlider.value
        };

        try
        {
            var file = JsonUtility.ToJson(data);
            using (FileStream stream = File.Create(fileLocation))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.WriteLine(file);
                }
            }
        }
        catch (Exception)
        {
            Debug.Log("Failed to save audio file " + fileLocation + ", reverting to previously saved values.");
            LoadFromFile();
        }
    }

    /// <summary>
    /// Update the inputs from a file. If loading fails, the game will use the defaults.
    /// </summary>
    public void LoadFromFile()
    {
        AudioData data = null;

        try
        {
            if (File.Exists(fileLocation))
            {
                using (var stream = File.Open(fileLocation, FileMode.Open))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string json = reader.ReadToEnd();
                        data = JsonUtility.FromJson<AudioData>(json);
                    }
                }
            }
            else
                data = ResetToDefault();
        }
        catch (Exception)
        {
            Debug.Log("Failed to load audio file " + fileLocation + ", using default values instead.");
            data = ResetToDefault();
        }
        finally
        {
            UpdateSliders(data);
            UpdateAudio();
        }
    }
}
