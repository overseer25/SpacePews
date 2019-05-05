using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles transitions between worlds.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Load the world.
    /// </summary>
    public void LoadWorld(string worldName)
    {
        SceneManager.LoadScene(worldName);
    }

    /// <summary>
    /// Quit the game.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
