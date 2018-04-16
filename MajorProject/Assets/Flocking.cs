using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    private GameObject leader;
    private Vector3 destination;

	void Start ()
    {
        leader = gameObject.GetComponentInParent<GroupManager>().GetLeader();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
