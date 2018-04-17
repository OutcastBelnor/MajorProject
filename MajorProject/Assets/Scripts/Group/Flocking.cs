using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    private Rigidbody rigidbody;

    private GameObject leader;
    private Vector3 destination;
    
    public List<GameObject> neighbours; // Stores all nearby neighbours
    private float viewDistance = 10.0f;
    //public float timeBetweenAreaChecks = 1.0f;

    public float speed = 5.0f;

    private List<Vector3> previousVectors; // Stores 5 previous flocking vectors for averaging

    private void Start ()
    {
        rigidbody = GetComponent<Rigidbody>();
        neighbours = new List<GameObject>();
        previousVectors = new List<Vector3>();
	}
	
	void Update ()
    {
        CheckAreaInView();
        if (neighbours.Count.Equals(0))
        {
            return;
        }

        Vector3 flocking = CalculateFlocking();
        Debug.Log("In Update: " + flocking);
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(flocking * speed, ForceMode.Force);
        
        //transform.position.Set(transform.position.x, 1.0f, transform.position.z);
        //transform.rotation = Quaternion.Euler(45.0f, 0.0f, 0.0f);
	}

    /// <summary>
    /// Calculates the overall vector of the flocking, based on the vectors of other steering behaviours.
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 CalculateFlocking()
    {
        Vector3 flockingVelocity = Vector3.zero;

        flockingVelocity += CalculateFollowing() * 0.25f; // Adds all the steering forces with appropriate weights
        flockingVelocity += CalculateSeparation() * 0.25f;
        flockingVelocity += CalculateAlignment() * 0.25f;
        flockingVelocity += CalculateCohesion() * 0.25f;

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
    private Vector3 CalculateFollowing()
    {
        Vector3 following = leader.transform.position - transform.position;

        float distanceToLeader = Vector3.Distance(leader.transform.position, transform.position);

        if (distanceToLeader > 1)
        {
            float speed = distanceToLeader / 0.3f;

            following *= speed / distanceToLeader;
            
            return following;
        }

        return Vector3.zero;
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

            separation += direction.normalized; // Adds a normalized version of this direction
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
        alignment -= rigidbody.velocity; // Substracts own velocity from the average
        
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
    /// This method sets the new leader.
    /// </summary>
    /// <param name="newLeader"></param>
    public void SetLeader(GameObject newLeader)
    {
        leader = newLeader;
    }

    /// <summary>
    /// Checks if there are any objects with colliders on them in viewDistance.
    /// If there are any Enemies, then add them to the neighbours.
    /// TODO: If there is Player, notify the leader and Attack.
    /// Any other tags ignore.
    /// </summary>
    private void CheckAreaInView()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, viewDistance);

        neighbours.Clear();

        foreach (Collider collider in collidersInRange)
        {
            if (collider.CompareTag("Enemy") && !gameObject.Equals(collider.gameObject))
            {
                neighbours.Add(collider.gameObject);
            }
        }
    }

    /// <summary>
    /// DEBUG: Draws the view distance of each flocking Enemy.
    /// </summary>
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}
