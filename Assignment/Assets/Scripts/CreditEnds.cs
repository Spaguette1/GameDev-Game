using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditEnds : MonoBehaviour
{

    public GameObject EndScript;

    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(waiter());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator waiter()
    {
        yield return new WaitForSecondsRealtime (31);
        SceneManager.LoadScene("MainMenu");
    }
}
