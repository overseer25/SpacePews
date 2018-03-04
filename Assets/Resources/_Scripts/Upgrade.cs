using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour {


    public string[] ShotModifier; // List of modifiers that should be applied, such as quad shot.
    public float speedModifier; // alters speed of the player ship.
    public float shotSpeedModifier; // Modifier of the velocity of the player projectile.
    public float firerateModifier; // alters fire rate of the player turret.
    public GameObject projectile; // alters the sprite of the projectile the player shoots.

    public GameObject upgradeCollect; // The collection animation

    private GameObject player;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	public void ApplyUpgrade()
    {

        Instantiate(upgradeCollect, transform.position, Quaternion.identity); // Play collection anim

        PlayerTurretController turret = player.GetComponent<PlayerTurretController>();
        
        foreach(string mod in ShotModifier)
        {
            if(mod == "doubleShot")
                turret.doubleShot = true;
            if (mod == "tripleShot")
                turret.tripleShot = true;
            if (mod == "quadShot")
            {
                turret.ResetToDefault();
                turret.quadShot = true;
                turret.tripleShot = false;
                turret.doubleShot = false;
            }
            if (mod == "crossShot")
            {
                turret.ResetToDefault();
                turret.crossShot = true;
            }  
            if (mod == "omniShot")
            {
                turret.ResetToDefault();
                turret.omniShot = true;
            }
        }

        turret.fireRate += firerateModifier;
        turret.shotSpeed += shotSpeedModifier;
        player.GetComponent<PlayerController>().maxSpeed += speedModifier;

        if(projectile != null)
        {
            turret.projectile = projectile;
        }

    }
}
