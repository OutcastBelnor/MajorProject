using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntensity : MonoBehaviour
{
    public int Intensity { get; set; }
    public bool IsInCombat { get; set; }

    public int MaxIntensity { get; set; }

    private void Start()
    {
        IsInCombat = false;

        MaxIntensity = 50;

        InvokeRepeating("Decrease", 1.0f, 1.0f); // Invokes repeating on decreasing the intensity while out of combat
    }

    /// <summary>
    /// Decreases the Intensity, when not in combat.
    /// </summary>
    private void Decrease()
    {
        if (!IsInCombat && Intensity > 0) // Checks if the Player is out of combat and the intensity isn't 0
        {
            Intensity--;
        }
    }

    /// <summary>
    /// Increases the Intensity.
    /// </summary>
    /// <param name="amount"></param>
    public void Increase(int amount)
    {
        Intensity += amount;

        if (Intensity >= MaxIntensity) // Checks if it doesn't go over the max
        {
            Intensity = MaxIntensity;
        }
    }
}
