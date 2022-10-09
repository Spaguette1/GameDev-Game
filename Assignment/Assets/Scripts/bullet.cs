using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;


public class bullet : MonoBehaviour

{

    public float lifeTime = 10f;
    public int damage = 20;
    private float temp;
    public GameObject bulletInst;

    private MeleeEnemy meleeEnemy;

    //public float speed = 300.0f;

    private Vector3 newPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) {
            Destroy(this.gameObject);
        }

        gameObject.GetComponent<Renderer>().material.color = Color.red;

    }

    void OnCollisionEnter(Collision col) {

        Debug.Log(col.collider.name);

        if (col.collider.name == "MeleeEnemy(Clone)") {
            col.collider.gameObject.GetComponent<MeleeEnemy>().ApplyDamage(damage);
            Destroy(this.gameObject);
        }
        else if (col.collider.tag == "RangedEnemy") {
            col.collider.gameObject.GetComponent<RangedEnemy>().ApplyDamage(damage);
            Destroy(this.gameObject);
        }
        else if (col.collider.name == "FPSController") {
            col.collider.gameObject.GetComponent<FirstPersonController>().ApplyDamage(damage);
            Destroy(this.gameObject);

        }
        else if (col.collider.name == "bullet(Clone)") {
            //do not destroy
        }
        else {
            Destroy(this.gameObject);
        }
       
        

    }

    
}
