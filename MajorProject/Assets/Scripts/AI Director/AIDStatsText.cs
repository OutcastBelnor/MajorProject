using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDStatsText : MonoBehaviour
{
    private AIDirector aIDirector;
    private List<Text> texts;

	private void Start ()
    {
        aIDirector = GameObject.FindGameObjectWithTag("AIDirector").GetComponent<AIDirector>();
        texts = new List<Text>(transform.GetComponentsInChildren<Text>());
	}
	
	private void Update ()
    {
		foreach (Text text in texts)
        {
            if (text.name.Equals("EnemyCount"))
            {
                text.text = ("Enemy Count: " + aIDirector.ActiveEnemies);
            }
        }
	}
}
