using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isInRange;
    private List<EnemyHealth> enemiesInRange;

    public LayerMask mask; // A Layer to ignore when raycasting

    private float baseDamage = 10.0f;

    private void Start()
    {
        isInRange = false;
        enemiesInRange = new List<EnemyHealth>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isInRange)
            {
                /*Debug.Log(enemiesInRange[0].name);
                float damage = baseDamage + Mathf.Round(Random.Range(3.0f, 7.0f));

                enemiesInRange[0].ChangeHealthPoints(-damage);*/

                GameObject enemy = EnemyClicked();

                if (enemy == null)
                {
                    return; // Returns if the enemy is null
                }
                
                if (enemy.CompareTag("Enemy"))
                {
                    EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

                    if (enemiesInRange.Contains(enemyHealth)) // Checks if the selected Enemy is in range
                    {
                        Debug.Log("Enemy is in range");
                        float damage = baseDamage + Mathf.Round(Random.Range(3.0f, 7.0f)); // Calculate the damage

                        enemyHealth.ChangeHealthPoints(-damage);
                    }
                }
            }
        }
    }

    /// <summary> Gets the GameObject clicked by the Player </summary>
    private GameObject EnemyClicked()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Gets the input from the mouse

        if (Physics.Raycast(ray, out hit, mask)) // Checks if the ray hit anything
        {
            GameObject objectHit = hit.transform.gameObject;
            return objectHit;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            isInRange = true;
            enemiesInRange.Add(other.gameObject.GetComponent<EnemyHealth>());
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemiesInRange.Contains(other.gameObject.GetComponent<EnemyHealth>()))
        {
            enemiesInRange.Remove(other.gameObject.GetComponent<EnemyHealth>());
        }

        if (enemiesInRange.Count == 0)
        {
            isInRange = false;
        }
    }
}
