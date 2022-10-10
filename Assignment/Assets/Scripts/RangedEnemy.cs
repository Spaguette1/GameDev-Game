using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class RangedEnemy : MonoBehaviour
{
    public enum FSMState
    {
        Patrol,
        Attack,
        Dead,
        None
    }

    public FSMState curState;
    public bool stationary;

    public GameObject[] wayPointList;

    private NavMeshAgent nav;
    private int currentDestination;

    public float attackRange = 20.0f;

    public float enemyHealth = 100.0f;

    public GameObject FPSController;
    public GameObject GameKillCounter;
    public GameObject bullet;

    protected Transform playerTransform;
	protected float elapsedTime;

    public float shootRate = 1.0f;
    public float shootPower;
    private Animator animator;







    // Start is called before the first frame update
    void Start()
    {
        if (!stationary) {
            curState = FSMState.Patrol;
        }
        else {
            curState = FSMState.None;
        }
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        currentDestination = 0;

        playerTransform = FPSController.transform;
        elapsedTime = 0.0f;

    }

    // Update is called once per frame
    void Update()
    {
        switch (curState) {
            case FSMState.Patrol: 
                UpdatePatrolState(); 

                //Debug.Log("patrol");
            break;
            case FSMState.Attack: 
                UpdateAttackState(); 
                //Debug.Log("attack");

            break;
            case FSMState.Dead: 
                UpdateDeadState(); 
                //Debug.Log("dead");

            break;
            case FSMState.None: 
                UpdateNoneState(); 
                //Debug.Log("dead");

            break;
        }

        if (enemyHealth <= 0) {
            curState = FSMState.Dead;
        }

        elapsedTime += Time.deltaTime;




        SetColourbyHealthValue(enemyHealth);

    }


    protected void UpdatePatrolState() {

        if ((transform.position.x == wayPointList[currentDestination].transform.position.x) && 
            (transform.position.z == wayPointList[currentDestination].transform.position.z)) {
                currentDestination++;
                if (currentDestination > 1) {
                    currentDestination = 0;
                }

        }
        else { //if not at destination ... move toward destination
            nav.SetDestination(wayPointList[currentDestination].transform.position);
        }

        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange) {
            animator.SetBool("Aware", true);
            curState = FSMState.Attack;
            
        }
    }

    protected void UpdateAttackState() {
        
        nav.isStopped = true;
        Vector3 enemyPos = transform.position;
        Vector3 playerPos = playerTransform.position;
        Vector3 direction = new Vector3(playerPos.x - enemyPos.x, playerPos.y - enemyPos.y, playerPos.z - enemyPos.z).normalized;
        Quaternion enemyRotation = Quaternion.LookRotation(direction);
        transform.rotation = enemyRotation;

        if (elapsedTime >= shootRate) {
            
            Debug.Log("shoot");
            GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().AddForce(direction * shootPower);

            Physics.IgnoreCollision(bulletInstance.GetComponent<Collider>(), GetComponent<Collider>(), true);

			
            elapsedTime = 0.0f;
        }

        if (!stationary) {
            if (Vector3.Distance(transform.position, playerTransform.position) > attackRange) {
                nav.isStopped = false;
                animator.SetBool("Aware", false);
                curState = FSMState.Patrol;
                


            }
        }
        
    }

    protected void UpdateDeadState() {
        //Destroy(this.gameObject);
        Destroy(this.gameObject);
        GameKillCounter.GetComponent<GameKills>().IncreaseKillCount();

        FPSController.transform.gameObject.SendMessage("UpdatekillCount", (int) 1 );
    }

    protected void UpdateNoneState() {
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange) {
                curState = FSMState.Attack;
            }
    }



    public void ApplyDamage(int damage ) {
    	enemyHealth -= damage;
        animator.SetBool("TakeDamage", true);
    }

    private void SetColourbyHealthValue(float number) {

        Color enemyColour;

        enemyColour = Color.Lerp(Color.red, Color.green, number/100);
        
        gameObject.GetComponent<Renderer>().material.color = enemyColour;
    }

    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.yellow;
	// 	Gizmos.DrawWireSphere(transform.position, attackRange);
    // }
}
