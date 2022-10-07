using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    public GameObject player;
    public FSMState curState;

    public float attackRange = 25.0f;
    public float health = 100.0f;

    public enum FSMState {
        None,
        Attack,
        Dead
    }

    // Start is called before the first frame update
    void Start()
    {
        curState = FSMState.None;
    }

    // Update is called once per frame
    void Update()
    {
        lookAtPlayer();

        switch (curState) {
            case FSMState.None: UpdateNoneState(); break;
            case FSMState.Attack: UpdateAttackState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        //conditions to change state
        if (health <= 0) {
            Debug.Log("Turret: dead");
            curState = FSMState.Dead;
        }
        else if (Vector3.Distance(player.transform.position, transform.position) < attackRange) {
            Debug.Log("Turrets: attack");
            curState = FSMState.Attack;
        }
        else if (Vector3.Distance(player.transform.position, transform.position) >= attackRange) {
            Debug.Log("Turrets: none");
            curState = FSMState.None;
        }

        
    }

    //attack state
    protected void UpdateAttackState() {
        lookAtPlayer();
    }

    protected void UpdateNoneState() {

    }

    protected void UpdateDeadState() {

    }






    protected void lookAtPlayer() {
        Vector3 look = player.transform.position - transform.position;
        look.y = transform.position.y;
        Quaternion rot = Quaternion.LookRotation(look);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1);
    }
    

    
}
