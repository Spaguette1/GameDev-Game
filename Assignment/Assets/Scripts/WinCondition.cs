using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{

    void OnTriggerEnter(Collider col) 
    {
         Debug.Log("entered win condition: " + col.GetComponent<Collider>().name);

         
         PauseMenu.GameIsPaused = true;
         Time.timeScale = 0f;
         //currentHealth = 100;
         Cursor.visible = true;
         Cursor.lockState = CursorLockMode.Confined;
         SceneManager.LoadScene("EndScreen");
         
        
    }
}
