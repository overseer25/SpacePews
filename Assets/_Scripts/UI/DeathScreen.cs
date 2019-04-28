using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{
    private TextMeshProUGUI text;
    private float alpha = 0.0f;
    private float fadeSpeed = 5.0f;
    private float waitTime = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.alpha = 0.0f;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(alpha < 1.0f)
        {
            alpha += 0.01f;
            text.alpha = alpha;
        }
    }

    /// <summary>
    /// Tell the death screen to display.
    /// </summary>
    public void Display()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Tell the death screen to hide.
    /// </summary>
    public void Hide()
    {
        alpha = 0.0f;
        text.alpha = alpha;
        gameObject.SetActive(false);
    }
}
