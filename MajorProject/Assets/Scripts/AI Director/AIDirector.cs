using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIDirector : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public GameObject player;
    private PlayerIntensity playerIntensity;

    public enum State { BuildUp, Sustain, Fade, Relax }; // States for the AI Director
    public State CurrentState { get; set; }

    private List<GameObject> spawned; // These will be holding current enemies and despawned enemies
    private List<GameObject> despawned; // For object pooling
    public int MaxEnemies { get; set; } // Maximum number of enemies in the scene
    public int ActiveEnemies { get; set; }
    private int minPopulation = 20;
    private int maxPopulation = 40;

    private float visibleAreaSize = 20.0f; // Area visible by the player
    public float activeAreaSize = 70.0f; // Area surrounding the player where the spawning takes places

    private float spawnRate = 0.25f;
    private float despawnRate = 1.0f;

    private float timePassed; // This variable will be used to keep track of time
    private float sustainTime; // This will hold the length of Sustain time
    private float relaxTime; // This will hold the length of Relax time

    private float intensityFade; // This will hold a randomized value in Fade state for intensity to reach

    private void Start()
    {
        //Instantiate(playerPrefab, transform.position, Quaternion.identity); DEBUG: Player will be placed in the scene manually for now
        //player = GameObject.FindGameObjectWithTag("Player"); // DEBUG: since the camera is attached to it
        playerIntensity = player.GetComponent<PlayerIntensity>(); // Gets the PlayerIntensity

        CurrentState = State.BuildUp; // Sets the state to the default

        ActiveEnemies = 0;
        spawned = new List<GameObject>();
        despawned = new List<GameObject>();

        //InvokeRepeating("Spawning", 0.5f, 1.0f);
        InvokeRepeating("CheckActiveEnemies", 0.5f, 1.0f);
    }

    private void Update()
    {
        switch (CurrentState) // Responsible for calling out the State methods
        {
            case State.BuildUp:
                BuildUpState();
                break;

            case State.Sustain:
                SustainState();
                break;

            case State.Fade:
                FadeState();
                break;

            case State.Relax:
                RelaxState();
                break;
        }
    }

    /// <summary>
    /// Populates the ActiveAreaSet until either the maximum population or maximum intensity has been reached
    /// </summary>
    private void BuildUpState()
    {
        if (playerIntensity.Intensity < playerIntensity.MaxIntensity)
        {
            if (!IsInvoking("Spawn"))
            { 
                MaxEnemies = UnityEngine.Random.Range(minPopulation, maxPopulation); // Randomize max population every cycle

                InvokeRepeating("Spawn", 0.1f, spawnRate); // If the maximum intensity haven't been reached then spawn more enemies
            }
        }
        else
        {
            CurrentState = State.Sustain; // When maximum Intensity is reached change state to Sustain

            sustainTime = UnityEngine.Random.Range(10, 15); // Randomize the sustain time for this cycle
            timePassed = Time.time; // Get the current time
        }
    }

    /// <summary>
    /// This state maintains maximum population for sustainTime seconds,
    /// and then switches the CurrentState to Fade.
    /// </summary>
    private void SustainState()
    {
        if (Time.time - timePassed >= sustainTime) // If time is reached
        {
            CancelInvoke("Spawn"); // Stop spawning
            CurrentState = State.Fade; // Change state to fade

            intensityFade = UnityEngine.Random.Range(20, 50); // Randomize target value that intensity 
        }                                                     // needs to drop to until next state change
    }

    /// <summary>
    /// This state starts to despawn until the intensity has decreased below intensityFade. 
    /// </summary>
    private void FadeState()
    {
        if (!IsInvoking("Despawn"))
        {
            InvokeRepeating("Despawn", 0.1f, despawnRate); // Start despawning enemies until minimum population is reached
        }

        if (playerIntensity.Intensity <= intensityFade) // If intensity has dropped enough
        {
            CurrentState = State.Relax; // Change state to Relax

            relaxTime = UnityEngine.Random.Range(20, 30); // Pick a random time for Relax length
            timePassed = Time.time; // Get the current time
        }
    }

    private void RelaxState()
    {
        if (Time.time - timePassed >= relaxTime) // Checks if relaxTime has passed
        {
            CancelInvoke("Despawn"); // Stop despawning
            CurrentState = State.BuildUp; // Change state to BuildUp
        }
    }

    /// <summary>
    /// This method handles spawning of the enemies.
    /// </summary>
    private void Spawn()
    {
        if (ActiveEnemies >= MaxEnemies) // Checks if there are maximum enemies spawned
        {
            Debug.Log("Max enemies reached");
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 spawnPoint = Vector3.zero;
        //if (horizontal == 0 && vertical == 0) // Checks if the Player doesn't move
        //{
            float distance = 0;
            do
            {
                Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * activeAreaSize;
                spawnPoint = new Vector3(randomPoint.x, 0.0f, randomPoint.y) + player.transform.position; // Creates a random point inside the ActiveAreaSet

                distance = Vector3.Distance(player.transform.position, spawnPoint);
            }
            while (distance <= visibleAreaSize); // Repeat until the spawnPoint is out of Player's view
            spawnPoint.y = 1.0f;            
        /*}
        else
        {
            if (horizontal == 1 && vertical == 1)
            {

            }
            else if (horizontal == -1 && vertical == -1)
            {

            }
            else if (horizontal == 1 && vertical == 1)
            {

            }
            else if (horizontal == 1 && vertical == 1)
            {

            }
        }*/

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPoint, out hit, 3.0f, NavMesh.AllAreas))
        {
            spawnPoint = hit.position; // Change the position to the valid position on Navmesh
        }

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

    private void Despawn()
    {
        if (ActiveEnemies > minPopulation)
        {
            GameObject enemy;
            do
            {
                int randomIndex = UnityEngine.Random.Range(0, ActiveEnemies); // Choose random enemy from the spawned list to despawn
                enemy = spawned[randomIndex];
            }
            while (Vector3.Distance(player.transform.position, enemy.transform.position) > visibleAreaSize); // Repeat until enemy is out sight of the Player
            
            spawned.Remove(enemy); // Remove it from the spawned list
            ActiveEnemies--; // Update ActiveEnemies count

            despawned.Add(enemy); // Add it to the despawned list for reuse
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
