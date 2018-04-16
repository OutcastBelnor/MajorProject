using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    private GameObject leader;
    private Vector3 destination;

    public List<GameObject> otherMembers;
    public List<GameObject> neighbours; // Stores all nearby neighbours
    private float viewDistance;

    public float timeBetweenAreaChecks = 1.0f;

    private void Start ()
    {
        //leader = gameObject.GetComponentInParent<GroupManager>().GetLeader();
        //otherMembers = gameObject.GetComponentInParent<GroupManager>().GetMembers(); // DEBUG
        neighbours = new List<GameObject>();

        InvokeRepeating("CheckAreaInView", 0.5f, timeBetweenAreaChecks);
	}
	
	void Update ()
    {
		
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
    /// <returns></returns>
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
