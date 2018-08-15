using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public GameObject healthSpritePrefab;
    public GameObject explosion;

    public GameObject heartUI;

    public float heartSize = 20f;

    private List<bool> heartsActive = new List<bool>();
    private int lastActiveIndex = 0;

    private bool dead = false;

    private void Start()
    {
        heartUI.GetComponent<GridLayoutGroup>().cellSize = new Vector2(heartSize, heartSize);
    }

    /// <summary>
    /// Draw starting health hearts at full opacity.
    /// Setup array to track active hearts.
    /// </summary>
    /// <param name="numToDraw"></param>
    public void SetupHealthSprite(int numToDraw)
    {
        for (int i = 0; i < numToDraw; i++)
        {
            GameObject heart = Instantiate(healthSpritePrefab, heartUI.transform, false);
            heartsActive.Add(true);
        }
        lastActiveIndex = heartsActive.Count - 1;
    }

    /// <summary>
    /// Redraw the number of hearts that should be displayed representing players health.
    /// </summary>
    /// <param name="numToDraw">How many full hearts the player has. Determined by health / healthChunk</param>
    /// <param name="transparencyOfLast">If the last heart is damaged, and what opacity it should represent.</param>
    public void RedrawHealthSprites(int numToDraw, float transparencyOfLast)
    {
        if (dead)
        {
            DrawNoHealth();
            return;
        }
        int currentHeartCount = lastActiveIndex + 1;
        //number of new full hearts equals total number hearts displayed
        if (numToDraw == currentHeartCount)
        {
            heartUI.transform.GetChild(currentHeartCount - 1).GetComponent<Image>().color = Color.white;
        }
        else if (numToDraw > currentHeartCount) // number of full hearts is more than total current heart count
        {
            heartUI.transform.GetChild(currentHeartCount - 1).GetComponent<Image>().color = Color.white;
            for (int i = currentHeartCount; i < heartsActive.Count; i++)
            {
                heartUI.transform.GetChild(i).GetComponent<Image>().enabled = true;
                heartUI.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                heartsActive[i] = true;
            }
            //not enough hearts currently parented, need to add more
            if (heartsActive.Count < numToDraw)
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
            for (int i = currentHeartCount - 1; i >= numToDraw; i--)
            {
                heartUI.transform.GetChild(i).GetComponent<Image>().enabled = false;
                heartsActive[i] = false;
            }
            lastActiveIndex = numToDraw - 1;
        }

        //If there is to be a heart that is slightly damaged
        if (transparencyOfLast > 0)
        {
            if (currentHeartCount > heartsActive.Count)
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

    /// <summary>
    /// When the player is dead, hide all drawn hearts, and show that none are active.
    /// </summary>
    public void DrawNoHealth()
    {
        for (int i = 0; i < heartsActive.Count; i++)
        {
            heartsActive[i] = false;
            lastActiveIndex = -1;
            heartUI.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            heartUI.transform.GetChild(i).GetComponent<Image>().enabled = false;
        }
    }

    /// <summary>
    /// Called by other scripts to inform the UI that the player has died.
    /// </summary>
    /// <param name="isDead">Is the player dead?</param>
    public void SetIsDead(bool isDead)
    {
        dead = isDead;
    }

    /// <summary>
    /// Change the size of the heart icon in the player HUD.
    /// </summary>
    /// <param name="newSize"></param>
    public void ChangeHeartSize(float newSize)
    {
        heartSize = newSize;
        heartUI.GetComponent<GridLayoutGroup>().cellSize = new Vector2(heartSize, heartSize);
    }
}
