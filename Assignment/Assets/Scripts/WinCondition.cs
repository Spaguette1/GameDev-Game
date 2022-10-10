using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    void OnTriggerEnter(Collider col) {
        Debug.Log("entered win condition: " + col.GetComponent<Collider>().name);
        
    }
}
