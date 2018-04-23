using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(EnemyStats))]
[RequireComponent (typeof(NavMeshAgent), typeof(EnemyAttack), typeof(EnemyHealth))]
public class EnemyBehaviour : MonoBehaviour
{
    private EnemyStats enemyStats;
    private EnemyHealth enemyHealth;
    private EnemyAttack enemyAttack;
    private NavMeshAgent enemyNavMeshAgent;

    private Transform playerPosition;
    private PlayerIntensity playerIntensity;
    private int intensityIncrease = 2;
    
    private enum State {Idle, Chase, Attack, Flee}; // States for the Finite State Machine
    private State currentState;

    private float wanderingTime; // Tracks how long enemy is "wandering"

    public bool isInCombat = false;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyAttack = GetComponent<EnemyAttack>();

        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        enemyNavMeshAgent.enabled = true;
        enemyNavMeshAgent.updateUpAxis = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPosition = player.transform;
            playerIntensity = player.GetComponent<PlayerIntensity>();
        }
    }

    private void Start()
    {
        enemyNavMeshAgent.enabled = true;

        currentState = State.Idle;

        wanderingTime = Time.time;
        SetDestination(RandomPosition()); // Sets up the enemy to "wander" to the first point
    }

    private void OnEnable()
    {
        enemyNavMeshAgent.enabled = true;
    }

    /// <summary>
    /// Called when this script is disabled.
    /// Disables NavMeshAgent and cancels attacking in EnemyAttack.
    /// </summary>
    private void OnDisable()
    {
        enemyNavMeshAgent.enabled = false;
        enemyAttack.CancelInvoke();
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

    /// <summary>
    /// Base state in which enemy "wanders" around, looking for the player
    /// </summary>
    private void IdleState()
    {
        enemyNavMeshAgent.speed = enemyStats.WalkingSpeed; // Set up the NavMeshAgent
        enemyNavMeshAgent.stoppingDistance = 0.0f; // Distance to stop from the destination

        if (Time.time - wanderingTime >= 5.0f) // Checks if it's the time to take a new destination to walk to
        {
            wanderingTime = Time.time;
            SetDestination(RandomPosition());
        }        
        
        if (CalculateDistance() < enemyStats.ViewDistance) // Checks if it "sees" the Player
        {
            playerIntensity.IsInCombat = true; // Puts a in combat status on Player

            if (enemyHealth.GetHealthPoints() <= 15.0f)
            {
                currentState = State.Flee; // Change state to Flee
            }
            else
            {
                currentState = State.Chase; // Change state to Chase

                playerIntensity.Increase(intensityIncrease);
            }
            
            Debug.Log("The current state is: " + currentState);
        }
    }

    /// <summary>
    /// Chasing after the Player, trying to get in range to attack him
    /// </summary>
    private void ChaseState()
    {
        SetDestination(playerPosition.position);
        enemyNavMeshAgent.speed = enemyStats.RunningSpeed;
        enemyNavMeshAgent.stoppingDistance = 2.5f; // Sets up NavMeshAgent
        
        if (CalculateDistance() <= 2.5f) // Checks if it is in range to attack the Player
        {
            currentState = State.Attack; // Change state to Attack
        }
        else if (CalculateDistance() >= enemyStats.ViewDistance + 10) // Checks if the Player is out of view
        {
            wanderingTime = 0.5f; // Sets up this variable so the Enemy can take on a new destination to wander to shortly
            currentState = State.Idle; // Change state to Idle
        
            playerIntensity.IsInCombat = false;
        }
    }

    /// <summary>
    /// Attacks the player at each interval, chases it if out of range or flees if it has low health
    /// </summary>
    private void AttackState()
    {
        if (!isInCombat) // Checks if it is in combat
        { 
            enemyAttack.StartAttacking(); // If not yet, but Player is in range then start attacking
            isInCombat = true;
        }
        
        if (CalculateDistance() > 2.5f) // Checks if it still is in range
        {
            enemyAttack.CancelInvoke();
            isInCombat = false;
            currentState = State.Chase; // Change state to Chase

            playerIntensity.Increase(intensityIncrease); // Increase intensity when Player starts to run away
        }
        else if (CalculateDistance() >= enemyStats.ViewDistance + 10.0f) // Checks if the Player is still in view distance
        {
            enemyAttack.CancelInvoke();
            isInCombat = false;
            playerIntensity.IsInCombat = false;

            wanderingTime = 0.5f; // Sets up this variable so the Enemy can take on a new destination to wander to shortly
            currentState = State.Idle; // Change state to Idle
        }
        else if (enemyHealth.GetHealthPoints() < 15.0f) // Checks if health is low
        {
            enemyAttack.CancelInvoke();
            isInCombat = false;
            currentState = State.Flee; // Change state to Flee
        }
    }

    /// <summary>
    /// Having low health Enemy decides to run away in opposite direction of the player
    /// </summary>
    private void FleeState()
    {
        enemyNavMeshAgent.speed = enemyStats.RunningSpeed;
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
        
        if (CalculateDistance() >= enemyStats.ViewDistance + 10.0f) // Checks if the player is in view
        {
            wanderingTime = 0.5f;
            currentState = State.Idle; // Change state to Idle

            playerIntensity.IsInCombat = false;
        }
    }

    /// <summary>
    /// Sets the new destination and flips the sprite if necessary
    /// </summary>
    /// <param name="destination"></param>
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

    /// <summary>
    /// Calculates a random position inside a circle of radius 20 units which centers at the enemy
    /// </summary>
    /// <returns></returns>
    private Vector3 RandomPosition()
    {
        Vector3 randomPosition;
        randomPosition = UnityEngine.Random.insideUnitSphere * 10.0f + enemyNavMeshAgent.transform.position;
        randomPosition.y = 1.0f;
        
        return randomPosition;
    }

    /// <summary>
    /// Calculates a distance from the player.
    /// </summary>
    /// <returns></returns>
    private float CalculateDistance()
    {
        return Vector3.Distance(enemyNavMeshAgent.transform.position, playerPosition.position);
    }
}
