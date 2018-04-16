using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for the group of enemies.
/// When spawned it will appoint a leader of the group and make other enemies followers.
/// </summary>
public class GroupManager : MonoBehaviour
{
    private List<GameObject> members;
    public GameObject leader;

	void Start ()
    {
        AcquireMembers();

        AppointLeader();
    }

    /// <summary>
    /// Gets current leader of the group.
    /// </summary>
    /// <returns>leader</returns>
    public GameObject GetLeader()
    {
        return leader;
    }

    /// <summary>
    /// Adds all the members of the group to the list members.
    /// </summary>
    private void AcquireMembers()
    {
        members = new List<GameObject>();
        foreach (Transform child in transform)
        {
            members.Add(child.gameObject);
        }
    }

    /// <summary>
    /// This method generates a random index that chooses the leader from members list.
    /// </summary>
    private void AppointLeader()
    {
        int randomIndex = UnityEngine.Random.Range(0, members.Count);

        leader = members[randomIndex];
    }
    
    void Update ()
    {
        CheckGroup();

        CheckLeader();
	}

    /// <summary>
    /// Checks if there are any members in this group.
    /// If no, then destroy the group.
    /// </summary>
    private void CheckGroup()
    {
        if (members.Count.Equals(0))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Checks if the leader is dead.
    /// If yes, then appoint new leader.
    /// </summary>
    private void CheckLeader()
    {
        if (!leader.activeSelf)
        {
            AppointLeader();
        }
    }
}
