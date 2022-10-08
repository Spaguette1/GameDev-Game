using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemySpawner : MonoBehaviour
{
    public GameObject FPSController;
    public GameObject meleeEnemy;
    public GameObject meleeEnemySpawner;

    public float spawnerInterval = 7.5f;
    public float spawnDistanceMin = 25.0f;
    public float spawnDistanceMax = 150.0f;
    public int totalSpawns = 5;

    private float distanceToPlayer;
    private int spawnCounter = 0;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(spawnEnemy(spawnerInterval, meleeEnemy));
    }

 
    private IEnumerator spawnEnemy(float spawnerInterval, GameObject meleeEnemy) {
        
        yield return new WaitForSeconds(spawnerInterval);

        if (spawnCounter <= totalSpawns) {
            distanceToPlayer = Vector3.Distance(transform.position, FPSController.transform.position);
            if ((distanceToPlayer <= spawnDistanceMax) && (distanceToPlayer >= spawnDistanceMin)) {
                GameObject newEnemy = Instantiate(meleeEnemy, transform.position, transform.rotation);
                newEnemy.transform.parent = meleeEnemySpawner.transform.parent;
                spawnCounter += 1;
                Debug.Log("spawned");
            }

        StartCoroutine(spawnEnemy(spawnerInterval, meleeEnemy));
        }

        

        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, spawnDistanceMin);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, spawnDistanceMax);
    }
}
