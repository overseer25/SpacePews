using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public GameObject explosion;

    private bool dead = false;

    private void Start()
    {

    }

    /// <summary>
    /// Called by other scripts to inform the UI that the player has died.
    /// </summary>
    /// <param name="isDead">Is the player dead?</param>
    public void SetIsDead(bool isDead)
    {
        dead = isDead;
    }
}
