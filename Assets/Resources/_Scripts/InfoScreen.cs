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
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // How can I manipulate this so the window's centerpoint doesn't snap to cursor when dragging?
        pos.z = 0;
        transform.position = pos;
    }
}
