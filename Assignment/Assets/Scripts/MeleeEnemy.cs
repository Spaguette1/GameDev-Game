using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;

public class MeleeEnemy : MonoBehaviour
{
    //melee enemy always chases

    

    public enum FSMState {
        Chase,
        Attack,
        Dead
    }

    public FSMState curState;
    private Animator animator;

    public NavMeshAgent enemy;
    public GameObject FPSController;
    public GameObject GameKillCounter;

    public float enemyHealth = 100.0f;
    public int damage = 10;

    public float attackRange = 2.0f;

    private float timeInAttack;

    // Start is called before the first frame update
    void Start()
    {
        enemy.GetComponent<NavMeshAgent>();
        curState = FSMState.Chase; //initial state
        animator = GetComponentInChildren<Animator>();

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

        if (distanceToPlayer < attackRange) { //ATTACK STATE
            //attack range, switch to attack state
            //Debug.Log("attack");
            if (curState != FSMState.Attack) {
                timeInAttack = 0f;
            }
            curState = FSMState.Attack;
        }
        else if (enemyHealth <= 0) { //DEAD STATE
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
        animator.SetBool("attack", false);
    }

    protected void UpdateAttackState() {
        enemy.SetDestination(transform.position); //stop the enemy from chasing
        float distanceToPlayer = Vector3.Distance(transform.position, FPSController.transform.position);

        timeInAttack += Time.deltaTime; 
        if (timeInAttack >= 0.2f) {
            FPSController.GetComponent<FirstPersonController>().ApplyDamage(damage);
            timeInAttack = 0f;
            animator.SetBool("attack", true);
            if (distanceToPlayer > attackRange)
            {
                ;
                curState = FSMState.Chase;

            }

        }
    }

    protected void UpdateDeadState() {
        GameKillCounter.GetComponent<GameKills>().IncreaseKillCount();

        FPSController.transform.gameObject.SendMessage("UpdatekillCount", (int) 1 );
        animator.SetBool("Isdead", true);

    }

    public void ApplyDamage(int damage ) {
    	enemyHealth -= damage;
        animator.SetTrigger("Hit");

    }

    private void SetColourbyHealthValue(float number) {

        Color enemyColour;

        enemyColour = Color.Lerp(Color.red, Color.green, number/100);
        
        gameObject.GetComponent<Renderer>().material.color = enemyColour;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}



