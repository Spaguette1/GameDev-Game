using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.AI;


public class BossEnemy : MonoBehaviour

{

    public GameObject player;
    public float activateChaseRange;
    public float activateAttackRange;
    public GameObject bullet;
    public GameObject GameKillCounter;



    public FSMState curState; 
    public enum FSMState {
        Chase,
        Attack,
        Dead,
        Frenzy,
        None
    }


    public NavMeshAgent boss;

    public int bossHealth = 600;

    public int damage = 1;
    public float damageDelay = 0.2f;

    private float frenzyDelay = 5.0f;
    public float shootPower;

    public bool invincible;


    // Start is called before the first frame update
    void Start()
    {
        invincible = false;
        curState = FSMState.None;
        boss.GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        
        switch (curState)
        {
            case FSMState.None: 
                UpdateNoneState(); 
                break;
            case FSMState.Chase: 
                UpdateChaseState();
                //Debug.Log("chase");
                break;
            case FSMState.Attack: 
                UpdateAttackState(); 
                //Debug.Log("attack");
                break;
            case FSMState.Frenzy: 
                UpdateFrenzyState();
                //Debug.Log("Frenzy");
                break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //activate frenzy mode if under half health
        if (bossHealth < 300) {
            curState = FSMState.Frenzy;
        }

        if (bossHealth <= 0) {
            curState = FSMState.Dead;
        }

        SetColourbyHealthValue(bossHealth);
    }




    protected void UpdateNoneState() {

        //do nothing

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= activateChaseRange) {
            curState = FSMState.Chase;
        }

    }

    protected void UpdateChaseState() {

        Vector3 dirToPlayer = transform.position - player.transform.position;
        Vector3 newPos = transform.position - dirToPlayer;
        boss.SetDestination(newPos);
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= activateAttackRange) {
            curState = FSMState.Attack;
        }
    }

    protected void UpdateAttackState() {
        
        // Vector3 dirToPlayer = transform.position - player.transform.position;
        // Vector3 newPos = transform.position - dirToPlayer;
        // boss.SetDestination(newPos);

        //return to chase if no longer in attack range
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer > activateAttackRange) {
            curState = FSMState.Chase;
        }

        //damage player by 1 for every 0.2 seconds in attack range
        damageDelay -= Time.deltaTime;
        if (damageDelay <= 0) {
            player.GetComponent<FirstPersonController>().ApplyDamage(damage);
            damageDelay = 0.2f;
        }


        
    }

    protected void UpdateFrenzyState() {

        Debug.Log(frenzyDelay);

        frenzyDelay -= Time.deltaTime;
        if (frenzyDelay >= 0) {

            //stop moving
            boss.SetDestination(transform.position);

            //make the boss immune
            invincible = true;

            //rotate the boss
            transform.Rotate(Vector3.up * Time.deltaTime * 150);

            //fire continually in direction boss is facing
            Vector3 forward = transform.TransformDirection (Vector3.forward);
            Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);

            GameObject bulletInstance = Instantiate(bullet, spawnPos, transform.rotation);
            bulletInstance.GetComponent<Rigidbody>().AddForce(forward * shootPower);
            Physics.IgnoreCollision(bulletInstance.GetComponent<Collider>(), GetComponent<Collider>(), true);
        }
        else if (frenzyDelay <= -5) {
            frenzyDelay = 5.0f;
        }
        else if (frenzyDelay <= 0) {


            invincible = false;
            

            //move towards player again
            Vector3 dirToPlayer = transform.position - player.transform.position;
            Vector3 newPos = transform.position - dirToPlayer;
            boss.SetDestination(newPos);

            
        }

        if (bossHealth <= 0) {
            curState = FSMState.Dead;
        }
        

    }

    protected void UpdateDeadState() {
        Destroy(this.gameObject);
        GameKillCounter.GetComponent<GameKills>().IncreaseKillCount();
        player.transform.gameObject.SendMessage("UpdatekillCount", (int) 1 );
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, activateChaseRange);
        Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, activateAttackRange);
    }

    private void SetColourbyHealthValue(float number) {

        Color enemyColour;

        enemyColour = Color.Lerp(Color.red, Color.green, number/600);
        
        gameObject.GetComponent<Renderer>().material.color = enemyColour;
    }

    public void ApplyDamage(int damage) {
        if (!invincible) {
            bossHealth -= damage;
        }
    } 
}
