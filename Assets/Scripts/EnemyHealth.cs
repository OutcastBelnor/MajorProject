using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private float enemyHealthPoints;

    private void Start()
    {
        enemyHealthPoints = 50;
    }

    public void ChangeHealthPoints(float modifier)
    {
        enemyHealthPoints += modifier;
        Debug.Log("Enemy health: " + enemyHealthPoints);

        if (enemyHealthPoints <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
