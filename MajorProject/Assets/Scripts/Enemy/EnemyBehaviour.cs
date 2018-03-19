using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    public GameObject player;

    private NavMeshAgent enemyNavMeshAgent;
    private float viewDistance = 20.0f;
    
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
            ChaseState();
        }
        else if (currentState.Equals(State.Attack))
        {
            Debug.Log("Enemy is attacking the player");
            AttackState();
        }
        else if (currentState.Equals(State.Flee))
        {
            Debug.Log("Enemy is fleeing from the player");
            FleeState();
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
            currentState = State.Chase;
        }
    }

    private void ChaseState()
    {
        enemyNavMeshAgent.destination = player.GetComponent<Rigidbody>().position;
        enemyNavMeshAgent.speed = 10.0f;

        float distanceToPlayer = Vector3.Distance(enemyNavMeshAgent.transform.position, player.transform.position);
        if (distanceToPlayer <= 2.5f)
        {
            currentState = State.Attack;
        }
        else if (distanceToPlayer >= viewDistance)
        {
            currentState = State.Idle;
        }

    }

    private void AttackState()
    {
        // nothing yet
    }

    private void FleeState()
    {
        enemyNavMeshAgent.speed = 10.0f;
        // nothing yet
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * 20.0f + enemyNavMeshAgent.transform.position;
        randomPosition.y = 1.0f;

        //Debug.Log(randomPosition);
        return randomPosition;
    }
}
