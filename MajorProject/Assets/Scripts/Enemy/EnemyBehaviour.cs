using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    public Vector3 playerPosition;
    public PlayerHealth playerHealth;

    private NavMeshAgent enemyNavMeshAgent;
    private float viewDistance = 20.0f;
    
    private enum State { Idle, Chase, Attack, Flee};
    private State currentState;

    private float wanderingTime;

    private float attackSpeed = 2.5f;
    private float timeBetweenAttacks;
    private float baseDamage = 5.0f;

    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        enemyNavMeshAgent.updateUpAxis = false;

        currentState = State.Idle;
        Debug.Log("The current state is: " + currentState);

        wanderingTime = Time.time;
        enemyNavMeshAgent.SetDestination(RandomPosition());

        timeBetweenAttacks = 1.5f;

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        switch(currentState)
        {
            case State.Idle:
                IdleState();
                break;

            case State.Chase:
                ChaseState();
                break;

            case State.Attack:
                AttackState();
                break;

            case State.Flee:
                FleeState();
                break;
        }
    }

    private void IdleState()
    {
        enemyNavMeshAgent.speed = 2.0f;

        if (Time.time - wanderingTime >= 5.0f)
        {
            wanderingTime = Time.time;
            enemyNavMeshAgent.SetDestination(RandomPosition());
        }        

        float distanceToPlayer = Vector3.Distance(enemyNavMeshAgent.transform.position, playerPosition);
        if (distanceToPlayer < viewDistance)
        {
            currentState = State.Chase;
            Debug.Log("The current state is: " + currentState);
        }
    }

    private void ChaseState()
    {
        enemyNavMeshAgent.destination = playerPosition;
        enemyNavMeshAgent.speed = 10.0f;
        
        if (CalculateDistance() <= 2.5f)
        {
            timeBetweenAttacks = 2.5f;
            currentState = State.Attack;
            Debug.Log("The current state is: " + currentState);
        }
        else if (CalculateDistance() >= viewDistance)
        {
            wanderingTime = 5.0f;
            currentState = State.Idle;
            Debug.Log("The current state is: " + currentState);
        }
    }

    private void AttackState()
    {
        if (Time.time - timeBetweenAttacks >= attackSpeed)
        {
            float damage = baseDamage + Mathf.Round(Random.Range(1.0f, 5.0f));
            playerHealth.ChangeHealthPoints(-damage);

            timeBetweenAttacks = Time.time;
        }
        
        if (CalculateDistance() >= 2.5f)
        {
            currentState = State.Chase;
            Debug.Log("The current state is: " + currentState);
        }
        else if (CalculateDistance() >= viewDistance)
        {
            wanderingTime = 5.0f;
            currentState = State.Idle;
            Debug.Log("The current state is: " + currentState);
        }
        else if (enemyHealth.GetHealthPoints() < 50.0f)
        {
            currentState = State.Flee;
            Debug.Log("The current state is: " + currentState);
        }
    }

    private void FleeState()
    {
        enemyNavMeshAgent.speed = 10.0f;

        enemyNavMeshAgent.SetDestination(-playerPosition);

        if (CalculateDistance() >= viewDistance)
        {
            wanderingTime = 5.0f;
            currentState = State.Idle;
            Debug.Log("The current state is: " + currentState);
        }
    }

    private Vector3 RandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * 20.0f + enemyNavMeshAgent.transform.position;
        randomPosition.y = 1.0f;
        
        return randomPosition;
    }

    private float CalculateDistance()
    {
        return Vector3.Distance(enemyNavMeshAgent.transform.position, playerPosition);
    }
}
