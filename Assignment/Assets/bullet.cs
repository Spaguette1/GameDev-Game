using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bullet : MonoBehaviour

{

    public float lifeTime = 1.0f;
    public int damage = 20;
    private float elapsedTime;
    private float temp;
    private GameObject enemyCollidedWith;

    private MeleeEnemy meleeEnemy;

    //public float speed = 300.0f;

    private Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnCollisionEnter(Collision col) {


        if (col.collider.name == "MeleeEnemy(Clone)") {
            col.collider.gameObject.GetComponent<MeleeEnemy>().ApplyDamage(damage);
            temp = col.collider.gameObject.GetComponent<MeleeEnemy>().enemyHealth;
        }

        Destroy(this.gameObject);

    }

    
}
