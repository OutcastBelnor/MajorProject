using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider slider;

    private float healthPoints;

	// Use this for initialization
	private void Start ()
    {
        healthPoints = 100;
        slider.value = healthPoints;
	}

    public float GetHealthPoints()
    {
        return healthPoints;
    }

    public void ChangeHealthPoints(float modifier)
    {
        healthPoints += modifier;
        slider.value = healthPoints;
    }
}
