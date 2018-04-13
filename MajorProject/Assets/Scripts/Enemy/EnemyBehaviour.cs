using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(NavMeshAgent))]
public class EnemyBehaviour : MonoBehaviour
{
    public Transform playerPosition;
    public PlayerHealth playerHealth;

    private NavMeshAgent enemyNavMeshAgent;
    private float viewDistance = 10.0f; // Distance in which the player can be "seen"
    
    private enum State {Idle, Chase, Attack, Flee}; // States for the Finite State Machine
    private State currentState;

    private float wanderingTime; // Tracks how long enemy is "wandering"

    private float attackSpeed = 2.5f; // Interval at which enemy can attack
    private float timeBetweenAttacks; // Tracks how long since last attack
    private float baseDamage = 5.0f;

    private EnemyHealth enemyHealth;

    private void Start()
    {
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        enemyNavMeshAgent.updateUpAxis = false;

        currentState = State.Idle;
        Debug.Log("The current state is: " + currentState);

        wanderingTime = Time.time;
        SetDestination(RandomPosition()); // Sets up the enemy to "wander" to the first point

        timeBetweenAttacks = 0.5f;

        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void Update()
    {
        switch(currentState) // Responsible for calling out the State methods
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

    /// <summary> Base state in which enemy "wanders" around, looking for the player </summary>
    private void IdleState()
    {
        enemyNavMeshAgent.speed = 2.0f; // Set up the NavMeshAgent
        enemyNavMeshAgent.stoppingDistance = 0.0f; // Distance to stop from the destination

        if (Time.time - wanderingTime >= 5.0f) // Checks if it's the time to take a new destination to walk to
        {
            wanderingTime = Time.time;
            SetDestination(RandomPosition());
        }        
        
        if (CalculateDistance() < viewDistance) // Checks if it "sees" the player
        {
            if (enemyHealth.GetHealthPoints() <= 15.0f)
            {
                currentState = State.Flee; // Change state to Flee
            }
            else
            {
                currentState = State.Chase; // Change state to Chase
            }
            
            Debug.Log("The current state is: " + currentState);
        }
    }

    /// <summary> Chasing after the Player, trying to get in range to attack him </summary>
    private void ChaseState()
    {
        SetDestination(playerPosition.position);
        enemyNavMeshAgent.speed = 10.0f;
        enemyNavMeshAgent.stoppingDistance = 2.5f; // Sets up NavMeshAgent
        
        if (CalculateDistance() <= 2.5f) // Checks if it is in range to attack the Player
        {
            currentState = State.Attack; // Change state to Attack
            Debug.Log("The current state is: " + currentState);
        }
        else if (CalculateDistance() >= viewDistance) // Checks if the Player is out of view
        {
            wanderingTime = 0.5f; // Sets up this variable so the Enemy can take on a new destination to wander to shortly
            currentState = State.Idle; // Change state to Idle
            Debug.Log("The current state is: " + currentState);
        }
    }

    /// <summary> Attacks the player at each interval, chases it if out of range or flees if it has low health </summary>
    private void AttackState()
    {
        if (Time.time - timeBetweenAttacks >= attackSpeed) // Checks if it time to attack
        {
            float damage = baseDamage + Mathf.Round(Random.Range(1.0f, 5.0f)); // Determines the total damage by adding a random value to the base
            playerHealth.ChangeHealthPoints(-damage); // Modifies the Player's health by the damage

            timeBetweenAttacks = Time.time; // Updates the timer for attacks
        }
        
        if (CalculateDistance() >= 2.5f) // Checks if it still is in range
        {
            currentState = State.Chase; // Change state to Chase
            Debug.Log("The current state is: " + currentState);
        }
        else if (CalculateDistance() >= viewDistance) // Checks if the Player is still in view distance
        {
            wanderingTime = 0.5f; // Sets up this variable so the Enemy can take on a new destination to wander to shortly
            currentState = State.Idle; // Change state to Idle
            Debug.Log("The current state is: " + currentState);
        }
        else if (enemyHealth.GetHealthPoints() < 15.0f) // Checks if health is low
        {
            currentState = State.Flee; // Change state to Flee
            Debug.Log("The current state is: " + currentState);
        }
    }

    /// <summary> Having low health Enemy decides to run away in opposite direction of the player </summary>
    private void FleeState()
    {
        enemyNavMeshAgent.speed = 10.0f;
        enemyNavMeshAgent.stoppingDistance = 0.0f; // Sets up the NavMeshAgent

        if (enemyNavMeshAgent.remainingDistance <= 3.0f) // Checks if it is close to the destination
        {
            Vector3 fleeDestination = transform.position;
            if (playerPosition.position.x > transform.position.x) // Checks the direction the player is in relation to the enemy
            {
                fleeDestination.x -= (10.0f + fleeDestination.x - transform.position.x); // And modify the flee destination accordingly
            }
            if (playerPosition.position.x < transform.position.x)
            {
                fleeDestination.x += (10.0f + fleeDestination.x - transform.position.x);
            }
            if (playerPosition.position.y > transform.position.y)
            {
                fleeDestination.y -= (10.0f + fleeDestination.y - transform.position.y);
            }
            if (playerPosition.position.y < transform.position.y)
            {
                fleeDestination.y += (10.0f + fleeDestination.y - transform.position.y);
            }
            
            SetDestination(fleeDestination);
        }
        
        if (CalculateDistance() >= viewDistance) // Checks if the player is in view
        {
            wanderingTime = 0.5f;
            currentState = State.Idle; // Change state to Idle
            Debug.Log("The current state is: " + currentState);
        }
    }

    /// <summary> Sets the new destination and flips the sprite if necessary </summary>
    private void SetDestination(Vector3 destination)
    {
        SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
        if (transform.position.x - destination.x < 0) // Checks which way the Enemy is heading
        {
            sprite.flipX = false; // And flips the sprite if necessary
        }
        else if (transform.position.x - destination.x > 0)
        {
            sprite.flipX = true;
        }

        enemyNavMeshAgent.SetDestination(destination);
    }

    /// <summary> Calculates a random position inside a circle of radius 20 units which centers at the enemy </summary>
    private Vector3 RandomPosition()
    {
        Vector3 randomPosition = Random.insideUnitSphere * 20.0f + enemyNavMeshAgent.transform.position;
        randomPosition.y = 1.0f;
        
        return randomPosition;
    }

    /// <summary> Calculates a distance from the player </summary>
    private float CalculateDistance()
    {
        return Vector3.Distance(enemyNavMeshAgent.transform.position, playerPosition.position);
    }
}
