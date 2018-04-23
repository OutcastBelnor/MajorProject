using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIntensity : MonoBehaviour
{
    private int intensity = 0;
    private bool isInCombat;

    private int maxIntensity = 100;

    private void Update()
    {
        if (!isInCombat)
        {
            InvokeRepeating("Decrease", 0.0f, 1.0f);
        }
        else
        {
            CancelInvoke();
        }
    }

    /// <summary>
    /// Decreases the intensity, when not in combat.
    /// </summary>
    private void Decrease()
    {
        if (intensity > 0)
        {
            intensity--;
        }
    }

    /// <summary>
    /// Increases the intensity.
    /// </summary>
    /// <param name="amount"></param>
    public void Increase(int amount)
    {
        intensity += amount;

        if (intensity >= maxIntensity) // Checks if it doesn't go over the max
        {
            intensity = maxIntensity;
        }
    }
}
