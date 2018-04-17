using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grouping : MonoBehaviour
{
    public GameObject enemyGroupPrefab;

    public float viewDistance = 10.0f;

    // Update is called once per frame
    void Update ()
    {
        CheckForNeighbours();
    }

    private void CheckForNeighbours()
    {
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, viewDistance); // Create a sphere that takes all of the colliders inside or touching it

        List<GameObject> neighbours = new List<GameObject>(); // Clear the previous neighbours

        foreach (Collider collider in collidersInRange)
        {
            if (collider.CompareTag("Enemy") && !gameObject.Equals(collider.gameObject)) // Checks if it's an Enemy and is not the current Enemy
            {
                neighbours.Add(collider.gameObject);
            }
        }

        if (!neighbours.Count.Equals(0))
        {
            GameObject enemyGroup = Instantiate(enemyGroupPrefab, transform.position, transform.rotation) as GameObject;
            enemyGroup.GetComponent<GroupManager>().AcquireMembers(neighbours);
        }
    }
}
