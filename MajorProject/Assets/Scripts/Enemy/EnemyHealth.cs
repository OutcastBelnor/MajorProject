using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float enemyHealthPoints;

    private void Start()
    {
        enemyHealthPoints = 50;
    }

    /// <summary>
    /// Returns amount of the Enemy health points.
    /// </summary>
    /// <returns>float</returns>
    public float GetHealthPoints()
    {
        return enemyHealthPoints;
    }

    /// <summary>
    /// Modifies the enemy health points by the parameter.
    /// If an amount must be substracted then the parameter must be negative.
    /// </summary>
    /// <param name="modifier"></param>
    public void ChangeHealthPoints(float modifier)
    {
        enemyHealthPoints += modifier;
        Debug.Log("Enemy health: " + enemyHealthPoints);

        if (enemyHealthPoints <= 0) // Checks if the Enemy is still alive
        { 
            this.gameObject.SetActive(false); // If not then deactivate the enemy TODO: Change the parent (for object pooling)
        }
    }
}