using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : MonoBehaviour
{
    //melee enemy always chases

    

    public enum FSMState {
        Chase,
        Attack,
        Dead
    }

    public FSMState curState; 

    public NavMeshAgent enemy;
    public GameObject FPSController;
    public GameObject GameKillCounter;

    public float enemyHealth = 100.0f;

    public float attackRange = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        enemy.GetComponent<NavMeshAgent>();
        curState = FSMState.Chase; //initial state
    }

    // Update is called once per frame
    void Update()
    {

        switch (curState)
        {
            case FSMState.Dead: UpdateDeadState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
        }


        float distanceToPlayer = Vector3.Distance(transform.position, FPSController.transform.position);

        if (distanceToPlayer < attackRange) {
            //attack range, switch to attack state
            //Debug.Log("attack");
            curState = FSMState.Attack;
        }
        else if (enemyHealth <= 0) {
            curState = FSMState.Dead;
        }
        else {
            //chase range, switch to chase state
            //Debug.Log("chase");
            curState = FSMState.Chase;
            
        }

        //mapping colour to health
        SetColourbyHealthValue(enemyHealth);
    }

    protected void UpdateChaseState() {
        Vector3 dirToPlayer = transform.position - FPSController.transform.position;
        Vector3 newPos = transform.position - dirToPlayer;
        enemy.SetDestination(newPos);
    }

    protected void UpdateAttackState() {
        //attack
    }

    protected void UpdateDeadState() {
        Destroy(this.gameObject);
        GameKillCounter.GetComponent<score>().IncreaseKillCount();
    }

    public void ApplyDamage(int damage ) {
    	enemyHealth -= damage;
    }

    private void SetColourbyHealthValue(float number) {

        Color enemyColour;

        enemyColour = Color.Lerp(Color.red, Color.green, number/100);
        
        gameObject.GetComponent<Renderer>().material.color = enemyColour;
    }
}



