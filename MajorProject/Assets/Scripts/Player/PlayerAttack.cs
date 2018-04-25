using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerIntensity))]
public class PlayerAttack : MonoBehaviour
{
    private float range = 2.5f;
    public float timeBetweenAttacks = 1.0f;
    private float timeSinceLastAttack;
    private float baseDamage = 10.0f;

    public LayerMask mask; // A Layer to ignore when raycasting

    private PlayerIntensity playerIntensity;
    private int intensityIncrease = 1;

    private void Start()
    {
        timeSinceLastAttack = Time.time;

        playerIntensity = GetComponent<PlayerIntensity>();
    }

    private void Update()
    {
        Attack();
    }

    /// <summary>
    /// Attempts to attack.
    /// The Player can only attack every timeBetweenAttacks.
    /// There is 12.5% chance that the Player will miss.
    /// The damage is applied to target Enemy health.
    /// </summary>
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0) && (Time.time - timeSinceLastAttack >= timeBetweenAttacks)) // Checks if the button was clicked
        {                                                                                           // and if it is the time to attack
            Debug.Log("Attempt to attack");

            timeSinceLastAttack = Time.time;
            // TODO: Change animation to attack

            GameObject enemy = EnemyClicked(); // Get anything that was clicked on

            if (enemy == null)
            {
                return; // Returns if the enemy is null
            }

            if (enemy.CompareTag("Enemy") && Vector3.Distance(enemy.transform.position, transform.position) <= range)
            {
                Debug.Log("Enemy in range");

                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

                int missValue = Random.Range(0, 8);

                if (missValue != 0) // Checks if the attack missed
                {
                    float damage = baseDamage + Mathf.Round(Random.Range(3.0f, 7.0f)); // Calculate the damage
                    enemyHealth.ChangeHealthPoints(-damage);

                    playerIntensity.Increase(intensityIncrease); // Increase intensity when hitting an Enemy
                }
                else
                {
                    Debug.Log("Player missed.");
                }
            }
        }
    }
    
    /// <summary>
    /// Gets the GameObject clicked by the Player
    /// </summary>
    /// <returns>GameObject</returns>
    private GameObject EnemyClicked()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Gets the input from the mouse

        if (Physics.Raycast(ray, out hit, mask)) // Checks if the ray hit anything
        {
            GameObject objectHit = hit.transform.gameObject; // If yes, then get the gameObject and return it
            return objectHit;
        }

        return null;
    }
}
