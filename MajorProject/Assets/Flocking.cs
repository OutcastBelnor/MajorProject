using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    private GameObject leader;
    private Vector3 destination;

    public List<GameObject> otherMembers;
    public List<GameObject> neighbours; // Stores all nearby neighbours
    private float viewDistance = 10.0f;
    public float timeBetweenAreaChecks = 1.0f;

    public float maxDistanceFromNeighbours = 5.0f;

    private void Start ()
    {
        //leader = gameObject.GetComponentInParent<GroupManager>().GetLeader();
        //otherMembers = gameObject.GetComponentInParent<GroupManager>().GetMembers(); // DEBUG
        neighbours = new List<GameObject>();

        InvokeRepeating("CheckAreaInView", 0.5f, timeBetweenAreaChecks);
	}
	
	void Update ()
    {
        CalculateFlocking();
	}

    private void CalculateFlocking()
    {
        CalculateFollowing();
        CalculateSeparation();
        CalculateAlignment();
        CalculateCohesion();
    }

    private void CalculateFollowing()
    {
        
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
        alignment -= GetComponent<Rigidbody>().velocity; // Substracts own velocity from the average

        return alignment;
    }

    private void CalculateCohesion()
    {
        throw new NotImplementedException();
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
    /// TODO: If there is Player, notify the leader and Attack.
    /// Any other tags ignore.
    /// </summary>
    private void CheckAreaInView()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, viewDistance);

        foreach (Collider collider in collidersInRange)
        {
            if (collider.CompareTag("Enemy"))
            {
                neighbours.Add(collider.gameObject);
            }
        }
    }
}
