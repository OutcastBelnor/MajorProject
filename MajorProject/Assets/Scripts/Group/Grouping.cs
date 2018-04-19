using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grouping : MonoBehaviour
{
    public GameObject enemyGroupPrefab;

    public float viewDistance = 10.0f;
    public List<GameObject> neighbours;

    void Start ()
    {
        InvokeRepeating("CheckForNeighbours", 0.5f, 0.5f);
    }

    /// <summary>
    /// Called on disabling this script, will cancel checking for neighbours.
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Checks the area in view distance for all colliders,
    /// then checks all the enemies among them,
    /// for groups.
    /// If there are no groups then create a new group with all of the enemies.
    /// </summary>
    private void CheckForNeighbours()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, viewDistance); // Create a sphere that takes all of the colliders inside or touching it

        neighbours = new List<GameObject>(); // Clear the previous neighbours

        foreach (Collider collider in collidersInRange)
        {
            GameObject neighbour = collider.gameObject;

            if (collider.CompareTag("Enemy") && !collider.gameObject.Equals(gameObject)) // Checks if it's an Enemy and not the current one
            {
                if (collider.gameObject.transform.parent != null) // Checks if they have a group
                {
                    neighbour.transform.parent.GetComponent<GroupManager>().AddMember(gameObject); // If yes, then add this Enemy to their group
                    return;
                }
                else
                {
                    neighbours.Add(collider.gameObject); // If not then add to the list of neighbours
                }
            }
        }

        if (!neighbours.Count.Equals(0)) // Checks if there is at least one Enemy in range without a group
        {
            neighbours.Add(gameObject); // Adds itself to the list so that it is included in the list

            GameObject enemyGroup = Instantiate(enemyGroupPrefab, transform.position, transform.rotation) as GameObject; // Create a new Group GameObject
            enemyGroup.GetComponent<GroupManager>().AcquireMembers(neighbours); // Add all of the possible Enemies to this group
        }
    }
}
