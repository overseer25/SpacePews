using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public GameObject healthSpritePrefab;
    public GameObject explosion;

    public GameObject heartUI;

    private float startingXPos = 29.5f;
    private float startingYPos = -28.1f;
    private float shiftAmout = 50f;

    private List<bool> heartsActive = new List<bool>();
    private int lastActiveIndex = 0;

    //void Update()
    //{
    //    // If killed
    //    if (player.health <= 0)
    //    {
    //        Instantiate(explosion, transform.position, Quaternion.identity);
    //        //heartUI.sprite = healthSprites[0];
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        //heartUI.sprite = healthSprites[player.health];
    //    }        
    //}

    public void SetupHealthSprite(int numToDraw)
    {
        for (int i = 0; i < numToDraw; i++)
        {
            GameObject heart = Instantiate(healthSpritePrefab, heartUI.transform, false);
            heartsActive.Add(true);
        }
        lastActiveIndex = heartsActive.Count - 1;
    }


    public void RedrawHealthSprites(int numToDraw, float transparencyOfLast)
    {
        Debug.Log(numToDraw);
        int currentHeartCount = lastActiveIndex + 1;
        //number of new full hearts equals total number hearts displayed
        if(numToDraw == currentHeartCount)
        {
            heartUI.transform.GetChild(currentHeartCount - 1).GetComponent<Image>().color = Color.white;            
        }
        else if(numToDraw > currentHeartCount) // number of full hearts is more than total current heart count
        {
            heartUI.transform.GetChild(currentHeartCount - 1).GetComponent<Image>().color = Color.white;
            for (int i = currentHeartCount; i < heartsActive.Count; i++)
            {
                heartUI.transform.GetChild(i).GetComponent<Image>().enabled = true;
                heartUI.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                heartsActive[i] = true;
            }
            //not enough hearts currently parented, need to add more
            if(heartsActive.Count < numToDraw)
            {
                int currentAmountActive = heartsActive.Count;
                for (int i = 0; i < numToDraw - currentAmountActive; i++)
                {
                    GameObject heart = Instantiate(healthSpritePrefab, heartUI.transform, false);
                    heartsActive.Add(true);
                }
                lastActiveIndex = heartsActive.Count - 1;
            }
        }
        else // number of full hearts is less than the total currently shown
        {
            for (int i = currentHeartCount - 1; i > numToDraw; i--)
            {
                heartUI.transform.GetChild(i).GetComponent<Image>().enabled = false;
                heartsActive[i] = false;
            }
            lastActiveIndex = numToDraw - 1;
        }

        //If there is to be a heart that is slightly damaged
        if (transparencyOfLast > 0)
        {
            if(currentHeartCount > heartsActive.Count)
            {
                GameObject heart = Instantiate(healthSpritePrefab, heartUI.transform, false);
                heart.GetComponent<Image>().color = new Color(1, 1, 1, transparencyOfLast);
                heartsActive.Add(true);
                lastActiveIndex = heartsActive.Count - 1;
            }
            else
            {
                lastActiveIndex++;
                heartUI.transform.GetChild(lastActiveIndex).GetComponent<Image>().enabled = true;
                heartUI.transform.GetChild(lastActiveIndex).GetComponent<Image>().color = new Color(1, 1, 1, transparencyOfLast);
                heartsActive[lastActiveIndex] = true;
            }
        }
    }
}
