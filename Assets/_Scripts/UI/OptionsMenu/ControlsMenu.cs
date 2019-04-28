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
        if(!inputIsSelected && Input.GetKeyDown(InputManager.current.controls.pause) && isOpen)
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
        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.fire, out result);
        fire.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.fire.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.forward, out result);
        forward.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.forward.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.left, out result);
        left.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.left.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.right, out result);
        right.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.right.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.inventory, out result);
        inventory.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.inventory.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.ability, out result);
        ability.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.ability.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.cameraZoomIn, out result);
        cameraZoomIn.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.cameraZoomIn.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.cameraZoomOut, out result);
        cameraZoomOut.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.cameraZoomOut.ToString() : result;

        KeyCodeExtensions.keyCodeNames.TryGetValue(InputManager.current.controls.suicide, out result);
        suicide.GetComponentInChildren<TextMeshProUGUI>().text = (result == default(string)) ? InputManager.current.controls.suicide.ToString() : result;
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
        if (InputManager.current.controls.fire == value)
            InputManager.current.controls.fire = KeyCode.None;
        else if (InputManager.current.controls.forward == value)
            InputManager.current.controls.forward = KeyCode.None;
        else if (InputManager.current.controls.left == value)
            InputManager.current.controls.left = KeyCode.None;
        else if (InputManager.current.controls.right == value)
            InputManager.current.controls.right = KeyCode.None;
        else if (InputManager.current.controls.inventory == value)
            InputManager.current.controls.inventory = KeyCode.None;
        else if (InputManager.current.controls.ability == value)
            InputManager.current.controls.ability = KeyCode.None;
        else if (InputManager.current.controls.cameraZoomIn == value)
            InputManager.current.controls.cameraZoomIn = KeyCode.None;
        else if (InputManager.current.controls.cameraZoomOut == value)
            InputManager.current.controls.cameraZoomOut = KeyCode.None;
        else if (InputManager.current.controls.suicide == value)
            InputManager.current.controls.suicide = KeyCode.None;
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
                InputManager.current.controls.fire = value;
                optionsMenu.PlayClickSound();
                break;
            case "forward":
                InputManager.current.controls.forward = value;
                optionsMenu.PlayClickSound();
                break;
            case "left":
                InputManager.current.controls.left = value;
                optionsMenu.PlayClickSound();
                break;
            case "right":
                InputManager.current.controls.right = value;
                optionsMenu.PlayClickSound();
                break;
            case "inventory":
                InputManager.current.controls.inventory = value;
                optionsMenu.PlayClickSound();
                break;
            case "ability":
                InputManager.current.controls.ability = value;
                optionsMenu.PlayClickSound();
                break;
            case "camerazoomin":
                InputManager.current.controls.cameraZoomIn = value;
                optionsMenu.PlayClickSound();
                break;
            case "camerazoomout":
                InputManager.current.controls.cameraZoomOut = value;
                optionsMenu.PlayClickSound();
                break;
            case "suicide":
                InputManager.current.controls.suicide = value;
                optionsMenu.PlayClickSound();
                break;
        }

        InputManager.current.SaveToFile();
    }
}
