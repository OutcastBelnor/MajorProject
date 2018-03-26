using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isInRange;
    private List<EnemyHealth> enemiesInRange;

    private float baseDamage = 10.0f;

    private void Start()
    {
        isInRange = false;
        enemiesInRange = new List<EnemyHealth>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (isInRange)
            {
                Debug.Log(enemiesInRange[0].name);
                float damage = baseDamage + Mathf.Round(Random.Range(3.0f, 7.0f));

                enemiesInRange[0].ChangeHealthPoints(-damage);
            }
        }
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
