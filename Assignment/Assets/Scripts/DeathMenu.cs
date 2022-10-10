using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathMenu : MonoBehaviour
{
    public static bool GameIsPaused = true;
    public GameObject deathMenuUI;


    public void Retry() 
    {
        PauseMenu.GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map_1");
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }
}
