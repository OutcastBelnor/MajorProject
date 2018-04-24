using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject player;

    private GameObject[] spawned; // These will be holding current enemies and despawned enemies
    private GameObject[] despawned; // For object pooling
    public int maxEnemies; // Maximum number of enemies in the scene
    public int ActiveEnemies { get; set; }

    private float visibleAreaSize = 20.0f; // Area visible by the player
    public float activeAreaSize = 70.0f; // Area surrounding the player where the spawning takes places

    private void Start()
    {
        //Instantiate(playerPrefab, transform.position, Quaternion.identity); DEBUG: Player will be placed in the scene manually for now
        //player = GameObject.FindGameObjectWithTag("Player"); // DEBUG: since the camera is attached to it

        maxEnemies = UnityEngine.Random.Range(30, 75);
        ActiveEnemies = 0;

        spawned = new GameObject[maxEnemies];
        despawned = new GameObject[maxEnemies];

        InvokeRepeating("Spawning", 0.5f, 1.0f);
    }

    private void Spawning()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0) // Checks if the Player doesn't move
        {
            Vector3 spawnPoint = Vector3.zero;
            do
            {
                spawnPoint = UnityEngine.Random.insideUnitSphere + player.transform.position;
                spawnPoint *= activeAreaSize; // Creates a random point inside the ActiveAreaSet
            }
            while (Vector3.Distance(player.transform.position, spawnPoint) <= visibleAreaSize); // Repeat until the spawnPoint is out of Player's view

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity) as GameObject; // Spawn the Enemy on the spawnPoint
            newEnemy.transform.parent = this.transform; // Change its position in the hierarchy

            spawned[ActiveEnemies] = newEnemy; // Add to the spawned Enemies
            ActiveEnemies++;
        }
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.transform.position, activeAreaSize);
    }
}
