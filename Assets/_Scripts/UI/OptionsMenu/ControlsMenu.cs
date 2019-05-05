using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [Header("Inputs")]
    public Image fire;
    public Image forward;
    public Image left;
    public Image right;
    public Image inventory;
    public Image ability;
    public Image cameraZoomIn;
    public Image cameraZoomOut;
    public Image suicide;

    [Header("Audio")]
    public AudioClip failedSound;
    public AudioClip clickSound;
    public AudioClip hoverSound;

    [Header("Other")]
    public OptionsMenu optionsMenu;

    public bool isOpen;
    public bool inputIsSelected;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        if(!inputIsSelected && Input.GetKeyDown(InputManager.controls.pause) && isOpen)
        {
            PlayClickSound();
            optionsMenu.HideControls();
        }

        foreach (var input in GetComponentsInChildren<ChangeKeyField>())
        {
            if (input.isSelected)
            {
                inputIsSelected = true;
                return;
            }
        }
        inputIsSelected = false;
    }

    /// <summary>
    /// Reset the controls to their default values.
    /// </summary>
    public void ResetControls()
    {
        InputManager.current.ResetToDefault();
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
    /// Plays the control menu hover sound.
    /// </summary>
    public void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverSound);
    }

    /// <summary>
    /// Update the window to use the values in the InputManager.
    /// </summary>
    public void UpdateControlWindow()
    {
        string result;
        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.fire, out result);
        fire.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.fire.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.forward, out result);
        forward.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.forward.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.left, out result);
        left.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.left.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.right, out result);
        right.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.right.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.inventory, out result);
        inventory.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.inventory.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.ability, out result);
        ability.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.ability.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.cameraZoomIn, out result);
        cameraZoomIn.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.cameraZoomIn.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.cameraZoomOut, out result);
        cameraZoomOut.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.cameraZoomOut.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.controls.suicide, out result);
        suicide.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.controls.suicide.ToString() : result;
    }

    /// <summary>
    /// Plays a failure sound.
    /// </summary>
    public void PlayFailedSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
        audioSource.PlayOneShot(failedSound, audioSource.volume * 0.5f);
    }

    /// <summary>
    /// If the incoming new key allocation is a duplicate, empty the control that contained the key before.
    /// </summary>
    private void EmptyControl(KeyCode value)
    {
        if (InputManager.controls.fire == value)
            InputManager.controls.fire = KeyCode.None;
        else if (InputManager.controls.forward == value)
            InputManager.controls.forward = KeyCode.None;
        else if (InputManager.controls.left == value)
            InputManager.controls.left = KeyCode.None;
        else if (InputManager.controls.right == value)
            InputManager.controls.right = KeyCode.None;
        else if (InputManager.controls.inventory == value)
            InputManager.controls.inventory = KeyCode.None;
        else if (InputManager.controls.ability == value)
            InputManager.controls.ability = KeyCode.None;
        else if (InputManager.controls.cameraZoomIn == value)
            InputManager.controls.cameraZoomIn = KeyCode.None;
        else if (InputManager.controls.cameraZoomOut == value)
            InputManager.controls.cameraZoomOut = KeyCode.None;
        else if (InputManager.controls.suicide == value)
            InputManager.controls.suicide = KeyCode.None;
    }

    /// <summary>
    /// Change the value of the specified control to the incoming key code.
    /// </summary>
    /// <param name="keyName"></param>
    /// <param name="value"></param>
    public void ChangeControl(string keyName, KeyCode value)
    {
        EmptyControl(value);
        switch (keyName)
        {
            case "fire":
                InputManager.controls.fire = value;
                optionsMenu.PlayClickSound();
                break;
            case "forward":
                InputManager.controls.forward = value;
                optionsMenu.PlayClickSound();
                break;
            case "left":
                InputManager.controls.left = value;
                optionsMenu.PlayClickSound();
                break;
            case "right":
                InputManager.controls.right = value;
                optionsMenu.PlayClickSound();
                break;
            case "inventory":
                InputManager.controls.inventory = value;
                optionsMenu.PlayClickSound();
                break;
            case "ability":
                InputManager.controls.ability = value;
                optionsMenu.PlayClickSound();
                break;
            case "camerazoomin":
                InputManager.controls.cameraZoomIn = value;
                optionsMenu.PlayClickSound();
                break;
            case "camerazoomout":
                InputManager.controls.cameraZoomOut = value;
                optionsMenu.PlayClickSound();
                break;
            case "suicide":
                InputManager.controls.suicide = value;
                optionsMenu.PlayClickSound();
                break;
        }

        InputManager.current.SaveToFile();
    }
}
