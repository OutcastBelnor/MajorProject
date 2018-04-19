using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(EnemyAttack))]
public class Flocking : MonoBehaviour
{
    private Rigidbody rigidBody;

    private GameObject leader { get; set; }
    private Vector3 destination;
    
    public List<GameObject> neighbours; // Stores all nearby neighbours
    private float viewDistance = 10.0f;
    //public float timeBetweenAreaChecks = 1.0f;

    public float speed = 5.0f;
    private List<Vector3> previousVectors;

    private EnemyHealth enemyHealth;
    private EnemyAttack enemyAttack;
    private Transform playerPosition;
    private bool isInCombat = false;

    private void Awake()
    {        
        rigidBody = GetComponent<Rigidbody>();
        neighbours = new List<GameObject>();
        previousVectors = new List<Vector3>();

        enemyHealth = GetComponent<EnemyHealth>();
        enemyAttack = GetComponent<EnemyAttack>();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start ()
    {
        InvokeRepeating("CheckAreaInView", 0.5f, 0.5f);
    }

    /// <summary>
    /// On disabling this script, will cancel checking area in view
    /// for neighbours and the player.
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Sets the new Leader.
    /// </summary>
    /// <param name="newLeader"></param>
    public void SetLeader(GameObject newLeader)
    {
        leader = newLeader;
    }

    void Update ()
    {
        Vector3 flocking = CalculateFlocking();
        if (flocking != null)
        {
            //rigidBody.velocity = flocking;
            rigidBody.AddForce(CalculateFlocking() * speed);
        }

        //transform.position.Set(transform.position.x, 1.0f, transform.position.z);
        //transform.rotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Calculates a flocking velocity based on all the steering forces.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 CalculateFlocking()
    {
        Vector3 flockingVelocity = Vector3.zero;

        flockingVelocity += CalculateSeparation() * 0.25f; // Adds the steering forces with appropriate weights
        flockingVelocity += CalculateAlignment() * 0.25f;
        flockingVelocity += CalculateCohesion() * 0.25f;

        if (isInCombat)
        {
            if (enemyHealth.GetHealthPoints() < 15.0f)
            {
                flockingVelocity += CalculateFleeing() * 0.25f;
            }
            else
            {
                flockingVelocity += CalculateFollowing(playerPosition.position) * 0.25f;
            }
        }
        else
        {
            flockingVelocity += CalculateFollowing(leader.transform.position) * 0.25f;
        }


        flockingVelocity = Smoothing(flockingVelocity);

        return flockingVelocity;
    }

    /// <summary>
    /// This method prevents the enemies from "twitching" resulting from cancelling out forces.
    /// </summary>
    /// <param name="flockingVelocity"></param>
    /// <returns>Vector3</returns>
    private Vector3 Smoothing(Vector3 flockingVelocity)
    {
        previousVectors.Add(flockingVelocity); // Adds to the list of previousVectors

        if (previousVectors.Count > 5) // Checks if the list is full
        {
            previousVectors.RemoveAt(0); // If yes, then removes the first element
        }

        Vector3 smoothedVector = Vector3.zero; 
        foreach(Vector3 vector in previousVectors)
        {
            smoothedVector += vector;
        }
        smoothedVector /= previousVectors.Count; // Averages the vector

        return smoothedVector;
    }

    /// <summary>
    /// This method calculates the desired velocity towards the leader.
    /// It uses arrive steering behaviour.
    /// Returns Vector3.zero if it is too close.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 CalculateFollowing(Vector3 target)
    {
        Vector3 following = target - transform.position;

        float distanceToTarget = Vector3.Distance(target, transform.position);

        if (distanceToTarget > 1)
        {
            float speed = distanceToTarget / 0.3f;

            following *= speed / distanceToTarget;
            
            return following;
        }

        return Vector3.zero;
    }

    /// <summary>
    /// This method calculates a steering force away from the player.
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateFleeing()
    {
        Vector3 fleeing = transform.position - playerPosition.position; // Calculate a velocity away from the player
        fleeing.Normalize(); // Normalizes it to be a vector of length 1

        return fleeing;
    }

    /// <summary>
    /// This method calculates a direction from each neighbour and adds it to a separationForce.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 CalculateSeparation()
    {
        Vector3 separation = Vector3.zero;

        foreach(GameObject neighbour in neighbours) // Checks each neighbour
        {
            Vector3 direction = transform.position - neighbour.transform.position; // Gets the direction away from the neighbour

            separation += direction;//.normalized; // Adds a normalized version of this direction
        }
        
        return separation;
    }

    /// <summary>
    /// This method calculates average velocity of the neighbours and substracts gameobject's own velocity from it.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 CalculateAlignment()
    {
        Vector3 alignment = Vector3.zero;

        foreach(GameObject neighbour in neighbours) // Checks each neighbour
        {
            alignment += neighbour.GetComponent<Rigidbody>().velocity; // Adds the velocity of the neighbour to the alignment
        }

        alignment /= neighbours.Count; // Averages the sum of velocities
        alignment -= rigidBody.velocity; // Substracts own velocity from the average
        
        return alignment;
    }

    /// <summary>
    /// This method calculates the direction to the center of the group.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 CalculateCohesion()
    {
        Vector3 center = calculateGroupCenter(); // Gets the group center based on current neighbours

        Vector3 cohesion = center - transform.position; // Calculates the velocity needed to "seek" this center
        cohesion.Normalize();
        cohesion -= GetComponent<Rigidbody>().velocity; // Substracts own velocity from the cohesion
        
        return cohesion;
    }

    /// <summary>
    /// Returns a center of the current group.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 calculateGroupCenter()
    {
        Vector3 center = Vector3.zero;

        foreach (GameObject neighbour in neighbours)
        {
            center += neighbour.transform.position;
        }

        center /= neighbours.Count;

        return center;
    }

    /// <summary>
    /// Checks if there are any objects with colliders on them in viewDistance.
    /// If there are any Enemies, then add them to the neighbours.
    /// If there is Player, (TODO: notify the leader) and Attack.
    /// Any other tags ignore.
    /// </summary>
    private void CheckAreaInView()
    {
        List<Collider> collidersInRange = new List<Collider>(Physics.OverlapSphere(transform.position, viewDistance)); // Create a sphere that takes all of the colliders inside or touching it

        neighbours = new List<GameObject>(); // Clear the previous neighbours

        foreach (Collider collider in collidersInRange) 
        {
            if (collider.CompareTag("Enemy")) // Checks if it's an Enemy
            {
                neighbours.Add(collider.gameObject);

                if (collider.transform.parent != null && !transform.parent.Equals(collider.transform.parent)) // Check if the Enemy is part of a different group
                {
                    collider.transform.parent.gameObject.GetComponent<GroupManager>().RemoveMember(collider.gameObject); // If yes then remove it from that group
                    transform.parent.GetComponent<GroupManager>().AddMember(collider.gameObject); // And add it to the group of this Enemy
                }
            }
        }

        if (collidersInRange.Contains(playerPosition.gameObject.GetComponent<SphereCollider>() as Collider)) // Check if Player is in view distance
        {
            isInCombat = true;
            
            if (Vector3.Distance(transform.position, playerPosition.position) <= 2.5f) // Check if Player is in attack range
            {
                enemyAttack.StartAttacking(); // If yes then start attacking
            }
            else
            {
                enemyAttack.CancelInvoke(); // If not then stop attacking
            }
        }
        else
        {
            isInCombat = false; // If not then remove combat state
        }

        if (neighbours.Count.Equals(0))
        {
            transform.parent.GetComponent<GroupManager>().RemoveMember(gameObject);
            transform.parent = null;

            GetComponent<EnemyBehaviour>().enabled = true;
            GetComponent<Grouping>().enabled = true;
            this.enabled = false;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 10.0f);
    }
}
