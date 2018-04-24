﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is responsible for the group of enemies.
/// When spawned it will appoint a leader of the group and make other enemies followers.
/// </summary>
public class GroupManager : MonoBehaviour
{
    public List<GameObject> members;
    public GameObject leader;

    private void OnEnable()
    {
        InvokeRepeating("CheckGroup", 0.5f, 1.0f);
    }

    /// <summary>
    /// Called on disabling this script, will cancel checking the group
    /// and the leader.
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
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
    /// Gets the members of the group.
    /// </summary>
    /// <returns>members</returns>
    public List<GameObject> GetMembers()
    {
        return members;
    }

    /// <summary>
    /// Adds a new member to the group, disables unnecessary scripts,
    /// and enables flocking.
    /// </summary>
    /// <param name="member"></param>
    public void AddMember(GameObject member)
    {
        members.Add(member);

        if (!member.transform.parent.name.Equals("AIDirector")) // If this member is part of another group
        {
            member.transform.parent.GetComponent<GroupManager>().RemoveMember(member); // Remove it from this group
        }

        member.transform.parent = gameObject.transform; // Adds it as a child of the group in the hierarchy

        member.GetComponent<Grouping>().enabled = false;
        member.GetComponent<EnemyBehaviour>().enabled = false;

        member.GetComponent<Flocking>().enabled = true;
        member.GetComponent<Flocking>().SetLeader(leader);
    }

    /// <summary>
    /// Adds all the members of the group to the list members.
    /// </summary>
    public void AcquireMembers(List<GameObject> newMembers)
    {
        members = newMembers;

        foreach(GameObject member in members)
        {
            member.transform.parent = transform; // Change the member to be a child of this group in hierarchy
            member.GetComponent<Grouping>().enabled = false; // Disables grouping script
        }

        AppointLeader();

        InvokeRepeating("CheckGroup", 0.1f, 0.5f);
    }

    /// <summary>
    /// This method generates a random index that chooses the leader from members list.
    /// </summary>
    private void AppointLeader()
    {
        int randomIndex = UnityEngine.Random.Range(0, members.Count); // Randomly assign the leader role to a member
        if (randomIndex >= members.Count)
        {
            return;
        }
        leader = members[randomIndex];

        leader.GetComponent<Flocking>().enabled = false; // Leader needs to be steered by the EnemyBehaviour script
        leader.GetComponent<EnemyBehaviour>().enabled = true; // So the Flocking script is not needed

        foreach (GameObject member in members)
        {
            if (member.Equals(leader))
            {
                continue;
            }

            member.GetComponent<EnemyBehaviour>().enabled = false; // Member needs to be steered by the Flocking script
            member.GetComponent<Grouping>().enabled = false; // So disable all the unnecessary scripts
            member.GetComponent<NavMeshAgent>().enabled = false;

            member.GetComponent<Flocking>().enabled = true; // So the EnemyBehaviour is not needed
            member.GetComponent<Flocking>().SetLeader(leader);
        }
    }

    /// <summary>
    /// Removes the gameobject from the members list.
    /// </summary>
    /// <param name="member"></param>
    public void RemoveMember(GameObject member)
    {
        members.Remove(member);

        member.GetComponent<EnemyBehaviour>().enabled = true;
        member.GetComponent<Grouping>().enabled = true;

        CheckLeader();
    }

    /// <summary>
    /// Removes all the gameobjects from the parameter list,
    /// if they are in the members list.
    /// </summary>
    /// <param name="enemies"></param>
    public void RemoveMembers(List<GameObject> enemies)
    {
        foreach (GameObject enemy in enemies)
        {
            if (members.Contains(enemy))
            {
                members.Remove(enemy);
            }
        }
    }

    /// <summary>
    /// Checks if there are any members in this group.
    /// If there is one, then remove it from the group and reset its parent object.
    /// If there are none, then destroy the group.
    /// </summary>
    private void CheckGroup()
    {
        if (members.Count.Equals(1)) // If there is only one member, then remove it from the group
        {
            leader.transform.parent = GameObject.FindGameObjectWithTag("AIDirector").transform;
            RemoveMember(leader);
        }

        if (members.Count.Equals(0)) // If there are no members then destroy the group
        {
            Debug.Log("Empty group");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Checks if the leader is dead.
    /// If yes, then appoint new leader.
    /// </summary>
    private void CheckLeader()
    {
        if (leader.Equals(null) || !members.Contains(leader)) // Check if there is a leader
        {
            AppointLeader(); // If not appoint a new leader
        }
    }
}
