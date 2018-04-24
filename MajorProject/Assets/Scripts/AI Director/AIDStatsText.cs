using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDStatsText : MonoBehaviour
{
    private AIDirector aIDirector;
    private List<Text> texts;

    private bool isTurnedOn;

	private void Start ()
    {
        aIDirector = GameObject.FindGameObjectWithTag("AIDirector").GetComponent<AIDirector>();
        texts = new List<Text>(transform.GetComponentsInChildren<Text>());

        isTurnedOn = true;
	}
	
	private void Update ()
    {
        if (Input.GetKeyDown(KeyCode.F12)) // Checks if the button is pressed
        {
            isTurnedOn = !isTurnedOn; 
        }

		foreach (Text text in texts)
        {
            if (text.gameObject.Equals(gameObject)) // Turn off if not enabled
            {
                continue;
            }

            text.enabled = isTurnedOn;

            if (text.name.Equals("EnemyCountText"))
            {
                text.text = "Enemy Count: " + aIDirector.ActiveEnemies; // Update EnemyCountText
            }
            else if (text.name.Equals("MaxEnemyText"))
            {
                text.text = "Max Enemies: " + aIDirector.MaxEnemies; // Update MaxEnemyText
            }
        }
	}
}
