using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Mineable : MonoBehaviour
{
    public int amountLootSpawned = 10;
    /// <summary>
    /// Mine rate describes how long in seconds it takes for the next loot item to be spawned in for this mineable object.
    /// </summary>
    [Tooltip("How many seconds until loot is spawned.")]
    public float mineRate = 1.0f;
    public float timeMinedSoFar { get; protected set; }

    public bool depleted = false;

    public GameObject explosion; // Sprite animation for explosion

    /// <summary>
    /// Loot possible is what resources can be spawned by this mineable object, 
    /// and it is in alignment with the spawn chance list.
    /// </summary>
    public List<GameObject> lootPossible;
    /// <summary>
    /// This describes the percent chance an item has to be granted to the player,
    /// and is one to one with the loot possible list.
    /// </summary>
    public List<float> spawnChance;

    /// <summary>
    /// Interally this is the basic setup for the loot table.
    /// </summary>
    private List<GameObject> items;
    private GameObject textMeshObj;
    private TextMesh text;

    private int lootAvailable;

    private static System.Random rand;

    private void Start()
    {
        timeMinedSoFar = 0;
        if (lootPossible.Count != spawnChance.Count)
        {
            Debug.LogError("The spawn options and spawn chances do not match up in length.");
            throw new Exception("Uh oh");
        }
        lootAvailable = amountLootSpawned;
        rand = new System.Random();
        rand.Next();

        text = this.GetComponentInChildren<TextMesh>();
        if(text != null)
        {
            textMeshObj = text.gameObject;
            textMeshObj.SetActive(false);
        }
        else
        {
            Debug.LogError("couldnt find text mesh " + this.name);
        }

    }

    private void OnMouseOver()
    {
        if (depleted)
        {
            textMeshObj.SetActive(true);
            textMeshObj.transform.position = this.transform.position + Vector3.Scale(Vector3.up * 1.5f, this.transform.localScale);
            textMeshObj.transform.rotation = Quaternion.identity;
        }
    }
    
    private void OnMouseExit()
    {
        if (depleted)
        {
            textMeshObj.SetActive(false);
        }
    }

    /// <summary>
    /// Called after mineRate time has passed that the player has been mining.
    /// </summary>
    public void SpawnLoot()
    {
        timeMinedSoFar = 0;
        int chance = rand.Next(101); //want a number that is [0,100]
        float percentChance = (float)chance / 100f;
        float chanceThusFar = 0f;
        int directionModifierX = chance > 50 ? 1 : -1;
        int directionModifierY = chance % 2 == 0 ? -1 : 1;
        for (int i = 0; i < spawnChance.Count; i++)
        {
            if(percentChance <= spawnChance[i] + chanceThusFar)
            {
                Instantiate(lootPossible[i], this.transform.position + new Vector3(directionModifierX * percentChance * (this.transform.localScale.x+1), directionModifierY * (1-percentChance) * (this.transform.localScale.y+1), 0), Quaternion.identity);
                break;
            }
            chanceThusFar += spawnChance[i];
        }
        lootAvailable--;
        if(lootAvailable == 0)
        {
            depleted = true;
        }
    }


    public float AddTimeMined(float timeToAdd)
    {
        timeMinedSoFar += timeToAdd;
        return timeMinedSoFar;
    }

}
