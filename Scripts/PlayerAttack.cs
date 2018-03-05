using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by enemy!");

        if (Input.GetMouseButtonDown(1))
        {
            float damage = Mathf.Round(Random.Range(10, 25));
            other.GetComponent<EnemyHealth>().ChangeHealthPoints(damage);
        }
    }
}
