using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoScreen : MonoBehaviour, IDragHandler {

    public Image itemImage;
    public Text description;
    public Text stats;
    public Button exitButton;
    public AudioSource exitSound;

    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    public void CloseWindow()
    {
        Debug.Log("Closed Window");
        AudioSource.PlayClipAtPoint(exitSound.clip, GameObject.FindGameObjectWithTag("Player").transform.position);
        gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100);
        var pos = Camera.main.ScreenToWorldPoint(mouse);
        transform.position = pos;
    }
}
