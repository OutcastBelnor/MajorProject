using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(EnemyStats))]
public class EnemyHealth : MonoBehaviour
{
    private EnemyStats enemyStats;

    private void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
    }

    /// <summary>
    /// Returns amount of the Enemy health points.
    /// </summary>
    /// <returns>float</returns>
    public float GetHealthPoints()
    {
        return enemyStats.EnemyHealthPoints;
    }

    /// <summary>
    /// Modifies the enemy health points by the parameter.
    /// If an amount must be substracted then the parameter must be negative.
    /// </summary>
    /// <param name="modifier"></param>
    public void ChangeHealthPoints(float modifier)
    {
        enemyStats.EnemyHealthPoints += modifier;
        Debug.Log("Enemy health: " + enemyStats.EnemyHealthPoints);

        if (enemyStats.EnemyHealthPoints <= 0) // Checks if the Enemy is still alive
        { 
            this.gameObject.SetActive(false); // If not then deactivate the enemy TODO: Change the parent (for object pooling)
        }
    }
}