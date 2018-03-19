using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed; // a variable that will control the speed of the character

    private Rigidbody rb;

    private bool isMoving; // a variable that will track if the character is moving
    private Vector3 destination; // the destination of the character provided by the
    private NavMeshAgent navMeshAgent;

	private void Start ()
    {
        rb = GetComponent<Rigidbody>();

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.SetDestination(rb.position);
	}
	
	private void Update ()
    {
        KeyboardMovement();
        MouseMovement();

        LimitPosition();
    }

    private void KeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = rb.transform.position + new Vector3(horizontal, 0.0f, vertical); // calculate player's movement
        if (!movement.Equals(rb.transform.position))
        {
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(rb.position);
                navMeshAgent.enabled = false;
            }

            rb.position = movement * speed; // assign the new position with the speed
        }
        
    }

    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 moveTo = hit.point;
                moveTo.y = 1.0f;

                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(moveTo);
            }
        }
    }

    private void LimitPosition()
    {
        Vector3 currentPosition = rb.transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -220.0f, 220.0f); // clamp the position of the player
        currentPosition.z = Mathf.Clamp(currentPosition.z, -220.0f, 220.0f); // so it doesn't go out of the screen
        currentPosition.y = 1.0f; // ensure that the character is at the appropriate height

        rb.transform.position = currentPosition;
    }
}
