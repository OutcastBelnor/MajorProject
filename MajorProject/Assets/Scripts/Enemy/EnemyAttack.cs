using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyStats))]
public class EnemyAttack : MonoBehaviour
{
    private EnemyStats enemyStats;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        enemyStats = GetComponent<EnemyStats>();
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }

    /// <summary>
    /// This method calls InvokeRepeating to start attacking the Player.
    /// </summary>
    public void StartAttacking()
    {
        InvokeRepeating("Attack", 0.5f, enemyStats.AttackSpeed);
    }

    private void Attack()
    {
        int missValue = Random.Range(0, 4);

        if (missValue != 0)
        {
            float damage = enemyStats.BaseDamage + Mathf.Round(UnityEngine.Random.Range(1.0f, 5.0f)); // Determines the total damage by adding a random value to the base
            playerHealth.ChangeHealthPoints(-damage); // Modifies the Player's health by the damage
            Debug.Log(gameObject.name + " hit the player for " + damage + " damage.");
        }
        else
        {
            Debug.Log(gameObject.name + " missed.");
        }
    }
}
