using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed; // a variable that will control the speed of the character

    private Rigidbody rb;

    public LayerMask mask; // A Layer to ignore when raycasting

    private bool isMoving; // a variable that will track if the character is moving
    private Vector3 destination; // the destination of the character provided by the
    private NavMeshAgent navMeshAgent;

	private void Start ()
    {
        rb = GetComponent<Rigidbody>();

        navMeshAgent = GetComponent<NavMeshAgent>(); // Sets up NavMeshAgent
        navMeshAgent.updateUpAxis = false; // Prevents NavMeshAgent from rotating the camera
        navMeshAgent.SetDestination(rb.position); // First destination, which is the current position
	}
	
	private void Update ()
    {
        KeyboardMovement();
        MouseMovement();

        LimitPosition();
    }

    /// <summary> Movement done with arrows or WSAD keys </summary>
    private void KeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical"); // Get the input from the keyboard

        Vector3 movement = rb.transform.position + new Vector3(horizontal, 0.0f, vertical); // calculate player's movement
        if (!movement.Equals(rb.transform.position)) // Checks if the current position is not the destination
        {
            if (navMeshAgent.enabled) // Checks if the NavMeshAgent is active
            {
                navMeshAgent.SetDestination(rb.position);
                navMeshAgent.enabled = false;
            }

            rb.position = movement * speed; // assign the new position with the speed
        }
        
    }

    /// <summary> Movement done with mouse </summary>
    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown(1)) // Checks if the right mouse button is clicked
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Creates a ray that is cast from the camera at the mouse cursor

            if (Physics.Raycast(ray, out hit, mask)) // Checks if the ray hit anything
            {
                Vector3 moveTo = hit.point; // Gets the intersection point
                moveTo.y = 1.0f; // Adjusts the height

                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(moveTo); // Sets the destination
            }
        }
    }

    /// <summary> Limits the position to the boundaries of the level and adjusts the height </summary>
    private void LimitPosition()
    {
        Vector3 currentPosition = rb.transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -220.0f, 220.0f); // clamp the position of the player
        currentPosition.z = Mathf.Clamp(currentPosition.z, -220.0f, 220.0f); // so it doesn't go out of the screen
        currentPosition.y = 1.0f; // ensure that the character is at the appropriate height

        rb.transform.position = currentPosition;
    }
}
