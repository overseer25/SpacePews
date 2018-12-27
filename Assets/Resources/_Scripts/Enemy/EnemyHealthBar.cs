using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    private int maxHealth;
    private int currentHealth;
    private float length = 100.0f;
    private EnemyHealth targetHealth; // the target.

    public void SetTarget()
    {

    }

    private void OnGUI()
    {
        GUI.Box(new Rect(0, 0, length, 20), "");
    }

    // Update is called once per frame
    void Update()
    {
        AdjustHealthBar(0);
    }

    /// <summary>
    /// Adjust the health bar length, based on the health passed in.
    /// </summary>
    /// <param name="health"></param>
    private void AdjustHealthBar(int health)
    {

    }
}
