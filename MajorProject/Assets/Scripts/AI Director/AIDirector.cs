using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject player;

    private List<GameObject> spawned; // These will be holding current enemies and despawned enemies
    private List<GameObject> despawned; // For object pooling
    public int MaxEnemies { get; set; } // Maximum number of enemies in the scene
    public int ActiveEnemies { get; set; }

    private float visibleAreaSize = 20.0f; // Area visible by the player
    public float activeAreaSize = 70.0f; // Area surrounding the player where the spawning takes places

    private void Start()
    {
        //Instantiate(playerPrefab, transform.position, Quaternion.identity); DEBUG: Player will be placed in the scene manually for now
        //player = GameObject.FindGameObjectWithTag("Player"); // DEBUG: since the camera is attached to it

        MaxEnemies = UnityEngine.Random.Range(20, 40);
        ActiveEnemies = 0;

        spawned = new List<GameObject>();
        despawned = new List<GameObject>();

        InvokeRepeating("Spawning", 0.5f, 1.0f);
        InvokeRepeating("CheckActiveEnemies", 0.5f, 1.0f);
    }

    /// <summary>
    /// This method handles spawning of the enemies.
    /// </summary>
    private void Spawning()
    {
        if (ActiveEnemies >= MaxEnemies) // Checks if there are maximum enemies spawned
        {
            Debug.Log("Max enemies reached");
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0) // Checks if the Player doesn't move
        {
            Vector3 spawnPoint = Vector3.zero;
            do
            {
                Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;
                spawnPoint = new Vector3(randomPoint.x, 0.0f, randomPoint.y) + player.transform.position;
                spawnPoint *= activeAreaSize; // Creates a random point inside the ActiveAreaSet
            }
            while (Vector3.Distance(player.transform.position, spawnPoint) <= visibleAreaSize); // Repeat until the spawnPoint is out of Player's view
            spawnPoint.y = 1.0f;

            GameObject newEnemy;
            if (despawned.Count != 0) // Checks if there are any available objects
            {
                newEnemy = despawned[0]; // Takes the first enemy from the despawned enemies
                newEnemy.SetActive(true); // Activates it
                newEnemy.transform.position = spawnPoint; // Sets the position to the spawnPoint
                despawned.Remove(newEnemy); // Removes it from the despawned enemies
            }
            else // If not, then create a new one
            {
                newEnemy = Instantiate(enemyPrefab, spawnPoint, enemyPrefab.transform.rotation) as GameObject; // Spawn the Enemy on the spawnPoint
            }

            newEnemy.transform.parent = this.transform; // Change its position in the hierarchy

            spawned.Add(newEnemy); // Add to the spawned Enemies
            ActiveEnemies++;
        }
    }

    /// <summary>
    /// This method checks if the spawned enemies are inside of the ActiveAreaSet.
    /// If not then the enemies are despawned.
    /// </summary>
    private void CheckActiveEnemies()
    {
        foreach (GameObject enemy in spawned.ToArray())
        {
            if (Vector3.Distance(enemy.transform.position, player.transform.position) >= activeAreaSize) // Checks the distance to the player
            {
                enemy.SetActive(false); // Deactivate enemy
                despawned.Add(enemy); // Add it to the despawned enemies
                spawned.Remove(enemy); // Remove it from the spawned enemies

                if (!enemy.transform.parent.Equals(transform)) // Checks if it is in the group
                {
                    enemy.transform.parent.GetComponent<GroupManager>().RemoveMember(enemy); // Removes it from the group
                    enemy.transform.parent = transform; // Changes the hierarchy to be the child of AIDirector
                }

                ActiveEnemies--;
            }
        }
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.transform.position, activeAreaSize);
    }
}
