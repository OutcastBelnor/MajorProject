using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private bool isInRange;
    private List<GameObject> enemiesInRange;

    private void Start()
    {
        isInRange = false;
        enemiesInRange = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Attacking!");
            if (isInRange)
            {
                Debug.Log(enemiesInRange[0].name);
                float damage = Mathf.Round(Random.Range(10, 25));

                foreach(GameObject enemy in enemiesInRange)
                {
                    enemy.GetComponent<EnemyHealth>().ChangeHealthPoints(-damage);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Triggered by enemy!");
            isInRange = true;
            enemiesInRange.Add(other.gameObject);
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if (enemiesInRange.Contains(other.gameObject))
        {
            enemiesInRange.Remove(other.gameObject);
        }

        if (enemiesInRange.Count == 0)
        {
            isInRange = false;
        }
    }
}
