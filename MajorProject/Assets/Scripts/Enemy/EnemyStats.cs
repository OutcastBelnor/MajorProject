using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds all the statistics for the Enemy.
/// </summary>
public class EnemyStats : MonoBehaviour
{
    // Health
    public float EnemyHealthPoints { get; set; }

    // Attack
    public float BaseDamage { get; set; }
    public float AttackSpeed { get; set; }

    // Movement
    public float WalkingSpeed { get; set; }
    public float RunningSpeed { get; set; }
    public float ViewDistance { get; set; }

    private void Start()
    {
        EnemyHealthPoints = 50.0f;

        BaseDamage = 5.0f;
        AttackSpeed = 2.5f;

        WalkingSpeed = 2.0f;
        RunningSpeed = 10.0f;
        ViewDistance = 10.0f;
    }
}
