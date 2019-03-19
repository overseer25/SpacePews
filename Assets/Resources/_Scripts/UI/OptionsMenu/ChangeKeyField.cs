using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChangeKeyField : MonoBehaviour
{
    private ControlsMenu menu;
    private Image box;
    private bool waiting;
    private KeyCode pressed;

    public bool isSelected;
    public bool isFocused;

    // The list of keycodes that this field can not have.
    public List<KeyCode> forbiddenKeys;

    private Color defaultColor = new Color(Color.white.r, Color.white.g, Color.white.b, 0.5f);
    private Color highlightColor = new Color(Color.white.r, Color.white.g, Color.white.b, 1.0f);
    private Color selectedColor = new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 1.0f);

    private void Start()
    {
        menu = GetComponentInParent<ControlsMenu>();
        box = GetComponentInChildren<Image>();
    }

    /// <summary>
    /// This is executed when a text input in the controls window is selected.
    /// </summary>
    private void LateUpdate()
    {
        if (Input.anyKeyDown)
        {
            if (!isFocused && Input.GetMouseButtonDown(0))
            {
                Deselect();
                return;
            }
            foreach (KeyCode code in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(code))
                {
                    if(forbiddenKeys.Contains(code))
                    {
                        Failed();
                        Deselect();
                        return;
                    }
                    pressed = code;
                    if(isSelected)
                    {
                        box.GetComponentInChildren<Text>().text = code.ToString();
                        menu.ChangeControl(name.ToLower(), code);
                        if (isFocused)
                        {
                            isSelected = false;
                            Highlight();
                        }
                        else
                            Deselect();
                        StartCoroutine(WaitForNextClick());
                    }
                }
            }
        }
    }

    /// <summary>
    /// When the user is changing a control to a mouse button, it treats the button setting as another
    /// key event. So we need to wait before we allow them to click on a control box again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForNextClick()
    {
        waiting = true;
        yield return new WaitForSeconds(0.2f);
        waiting = false;
        yield break;
    }

    /// <summary>
    /// What happens when the user inputs a code that is invalid.
    /// </summary>
    public void Failed()
    {
        menu.PlayFailedSound();
    }

    public void Highlight()
    {
        isFocused = true;
        if (isSelected)
            return;
        box.color = highlightColor;
        box.GetComponentInChildren<Text>().color = highlightColor;
    }

    public void Dehighlight()
    {
        isFocused = false;
        if (isSelected)
            return;
        box.color = defaultColor;
        box.GetComponentInChildren<Text>().color = defaultColor;
    }

    public void Select()
    {
        if (pressed != KeyCode.Mouse0 || waiting || isSelected)
            return;
        box.color = selectedColor;
        box.GetComponentInChildren<Text>().color = selectedColor;
        isSelected = true;
    }

    public void Deselect()
    {
        if (!isSelected)
            return;
        box.color = defaultColor;
        box.GetComponentInChildren<Text>().color = defaultColor;
        isSelected = false;
    }
}
