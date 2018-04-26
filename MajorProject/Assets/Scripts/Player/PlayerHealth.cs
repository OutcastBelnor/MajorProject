using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider slider;

    private float healthPoints;

	private void Start ()
    {
        slider = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>(); // Gets the HealthBar from the UI

        healthPoints = 100;
        slider.value = healthPoints; // Initially set the value on HealthBar
	}

    /// <summary>
    /// Returns healthPoints
    /// </summary>
    /// <returns>float</returns>
    public float GetHealthPoints()
    {
        return healthPoints;
    }

    /// <summary>
    /// Changes the healthPoints with the parameter and updates value on HealthBar.
    /// </summary>
    /// <param name="modifier"></param>
    public void ChangeHealthPoints(float modifier)
    {
        healthPoints += modifier;
        slider.value = healthPoints; // Updates value;

        if (healthPoints <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}
