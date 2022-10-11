using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;


public class EnemyBomber : MonoBehaviour
{

    public enum FSMState {
        None,
        Chase,
        Explode
    }
    public FSMState curState; 

    public UnityEngine.AI.NavMeshAgent bomber;
    public GameObject FPSController;
    public GameObject GameKillCounter;

    public int damage = 35;

    public float chaseRange = 15.0f;
    public float attackRange = 1.5f;

    private float explodeFuse = 0.0f;

    public int enemyHealth = 100;


    private List<string> collisions = new List<string>();





    // Start is called before the first frame update
    void Start()
    {
        bomber.GetComponent<UnityEngine.AI.NavMeshAgent>();
        curState = FSMState.None; //initial state

    }

    // Update is called once per frame
    void Update()
    {
        switch (curState)
        {
            case FSMState.None: UpdateNoneState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Explode: UpdateExplodeState(); break;
        }

        if (enemyHealth <= 0) {
            Destroy(this.gameObject);
        }
        
        SetColourbyHealthValue(enemyHealth);
    }

    protected void UpdateNoneState() {

        //wait for player to get closer

        float distanceToPlayer = Vector3.Distance(transform.position, FPSController.transform.position);
        if (distanceToPlayer <= chaseRange) {
            curState = FSMState.Chase;
        }
    }

    protected void UpdateChaseState() {
        Vector3 dirToPlayer = transform.position - FPSController.transform.position;
        Vector3 newPos = transform.position - dirToPlayer;
        bomber.SetDestination(newPos);

        float distanceToPlayer = Vector3.Distance(transform.position, FPSController.transform.position);
        if (distanceToPlayer <= attackRange) {
            curState = FSMState.Explode;
        }
    }

    protected void UpdateExplodeState() {
        bomber.SetDestination(transform.position);

        // if (distanceToPlayer > attackRange) {
        //     curState = FSMState.Chase;
        // }

        explodeFuse += Time.deltaTime; 
        if (explodeFuse >= 2.0f) {
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i] == "FPSController") {
                    FPSController.GetComponent<FirstPersonController>().ApplyDamage(damage);
                }
            }


            Destroy(this.gameObject);
            GameKillCounter.GetComponent<GameKills>().IncreaseKillCount();
            FPSController.transform.gameObject.SendMessage("UpdatekillCount", (int) 1 );
        }
    }

    public void ApplyDamage(int damage) {
        enemyHealth -= damage;
    }

    void OnTriggerEnter(Collider col) {
        Debug.Log("enter: " + col.GetComponent<Collider>().name);
        collisions.Add(col.GetComponent<Collider>().name);
    }

    void OnTriggerExit(Collider col) {
        Debug.Log("exit: " + col.GetComponent<Collider>().name);
        collisions.Remove(col.GetComponent<Collider>().name);
    }

    private void SetColourbyHealthValue(float number) {

        Color enemyColour;

        enemyColour = Color.Lerp(Color.red, Color.green, number/100);
        
        gameObject.GetComponent<Renderer>().material.color = enemyColour;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
