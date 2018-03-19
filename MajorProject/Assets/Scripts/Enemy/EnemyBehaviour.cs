using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    public GameObject player;

    private NavMeshAgent enemyNavMeshAgent;
    private float viewDistance = 5.0f;
    
    private enum State { Idle = 0, Chase = 1, Attack = 2, Flee = 3};
    private State currentState;

    private float wanderingTime;

    //private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        enemyNavMeshAgent.updateUpAxis = false;
        //enemyNavMeshAgent.enabled = false;

        currentState = State.Idle;

        wanderingTime = Time.time;
        enemyNavMeshAgent.SetDestination(RandomPosition());
    }

    private void Update()
    {
        if (currentState.Equals(State.Idle))
        {
            Debug.Log("Enemy is idle");
            IdleState();
        }
        else if (currentState.Equals(State.Chase))
        {
            Debug.Log("Enemy is chasing the player");
        }
        else if (currentState.Equals(State.Attack))
        {
            Debug.Log("Enemy is attacking the player");
        }
        else if (currentState.Equals(State.Flee))
        {
            Debug.Log("Enemy is fleeing from the player");
        }
    }

    private void IdleState()
    {
        if (Time.time - wanderingTime >= 5.0f)
        {
            wanderingTime = Time.time;
            enemyNavMeshAgent.SetDestination(RandomPosition());
        }        

        float distanceToPlayer = Vector3.Distance(enemyNavMeshAgent.transform.position, player.transform.position);
        if (distanceToPlayer < viewDistance)
        {
            //currentState = State.Chase;
        }
    }

    private void ChaseState()
    {
        enemyNavMeshAgent.destination = player.GetComponent<Rigidbody>().position;

        //if ()
    }

    private void AttackState()
    {
        // nothing yet
    }

    private void FleeState()
    {
        // nothing yet
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * 20.0f + enemyNavMeshAgent.transform.position;
        randomPosition.y = 1.0f;

        Debug.Log(randomPosition);
        return randomPosition;
    }
}
