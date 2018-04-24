using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(PlayerIntensity))]
public class PlayerAttack : MonoBehaviour
{
    private bool isInRange;
    private List<EnemyHealth> enemiesInRange;
    public float timeBetweenAttacks = 1.0f;
    private float timeSinceLastAttack;

    public LayerMask mask; // A Layer to ignore when raycasting

    private float baseDamage = 10.0f;

    private PlayerIntensity playerIntensity;
    private int intensityIncrease = 1;

    private void Start()
    {
        isInRange = false;
        enemiesInRange = new List<EnemyHealth>();

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
            timeSinceLastAttack = Time.time;
            // TODO: Change animation to attack

            if (isInRange)
            {
                GameObject enemy = EnemyClicked(); // Get anything that was clicked on

                if (enemy == null)
                {
                    return; // Returns if the enemy is null
                }

                if (enemy.CompareTag("Enemy"))
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

                    if (enemiesInRange.Contains(enemyHealth)) // Checks if the selected Enemy is in range
                    {
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

    /// <summary>
    /// Adds every Enemy that triggers the collider to the enemiesInRange list.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy")) // Checks if it is the enemy
        {
            isInRange = true;
            enemiesInRange.Add(other.gameObject.GetComponent<EnemyHealth>()); // If yes then add it to the list

            playerIntensity.Increase(intensityIncrease); // Increase Player's intensity for each new Enemy in range
        }        
    }

    /// <summary>
    /// Removes the Enemies from the enemiesInRange list when they leave the range.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && enemiesInRange.Contains(other.gameObject.GetComponent<EnemyHealth>())) // Checks if this is an Enemy
        {                                                                                                                  // and if it's on the list
            enemiesInRange.Remove(other.gameObject.GetComponent<EnemyHealth>()); // If yes, then remove it from the list
        }

        if (enemiesInRange.Count == 0) // Checks if there are any Enemies in range
        {   
            isInRange = false;
        }
    }
}
