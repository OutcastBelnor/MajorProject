using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed; // a variable that will control the speed of the character
    
    private Animator animator;

    public LayerMask mask; // A Layer to ignore when raycasting

    private bool isMoving; // a variable that will track if the character is moving
    private Vector3 destination; // the destination of the character provided by the
    private NavMeshAgent navMeshAgent;

	private void Start ()
    {
        animator = GetComponentInChildren<Animator>();

        navMeshAgent = GetComponent<NavMeshAgent>(); // Sets up NavMeshAgent
        navMeshAgent.updateUpAxis = false; // Prevents NavMeshAgent from rotating the camera
        navMeshAgent.SetDestination(transform.position); // First destination, which is the current position
	}
	
	private void Update ()
    {
        Debug.Log("Local position" + transform.InverseTransformDirection(GameObject.FindGameObjectWithTag("Enemy").transform.position));

        LimitPosition();

        KeyboardMovement();
        MouseMovement();
    }

    /// <summary> Movement done with arrows or WSAD keys </summary>
    private void KeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // Get the input from the keyboard
        float vertical = Input.GetAxisRaw("Vertical");

        Transform parent = gameObject.GetComponentInChildren<SpriteRenderer>().transform.parent;
        FlipSprite(horizontal, parent);

        Vector3 movement = transform.position + new Vector3(horizontal, 0.0f, vertical); // calculate player's movement
        if (!movement.Equals(transform.position)) // Checks if the current position is not the destination
        {
            // TODO: Change animation to walking

            if (navMeshAgent.enabled) // Checks if the NavMeshAgent is active
            {
                navMeshAgent.SetDestination(transform.position);
                navMeshAgent.enabled = false;
            }

            transform.position = Vector3.Lerp(transform.position, movement, speed); // assign the new position with the speed

            animator.SetBool("isMoving", true);
            return;
        }

        animator.SetBool("isMoving", false);
    }

    /// <summary> Movement done with mouse </summary>
    private void MouseMovement()
    {
        if (Input.GetMouseButtonDown(1)) // Checks if the right mouse button is clicked
        {
            //animator.SetBool("isMoving", true);

            float screenCenter = Screen.width / 2;
            float mousePosX = Input.mousePosition.x;

            Transform parent = gameObject.GetComponentInChildren<SpriteRenderer>().transform.parent;
            FlipSprite(mousePosX - screenCenter, parent);

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Creates a ray that is cast from the camera at the mouse cursor

            if (Physics.Raycast(ray, out hit, mask)) // Checks if the ray hit anything
            {
                Vector3 moveTo = hit.point; // Gets the intersection point
                moveTo.y = 1.0f; // Adjusts the height

                /*if (!moveTo.Equals(transform.position))
                {
                    // TODO: Change animation to walking
                }*/

                navMeshAgent.enabled = true;                    
                navMeshAgent.SetDestination(moveTo); // Sets the destination
            }

            return;
        }

        //animator.SetBool("isMoving", false);
    }

    /// <summary>
    /// Flips the sprites in the appropriate direction.
    /// If it's an arm of legs, changes its relative position.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="sprites"></param>
    private static void FlipSprite(float direction, Transform parent)
    {
        Vector3 scale = parent.transform.localScale;

        if (direction > 0)
        {            
            if (scale.x < 0)
            { 
                scale.x *= -1.0f;
                parent.transform.localScale = scale;
            }
        }
        else if (direction < 0)
        {
            if (scale.x > 0)
            {
                scale.x *= -1.0f;
                parent.transform.localScale = scale;
            }
        }
    }

    /// <summary> Limits the position to the boundaries of the level and adjusts the height </summary>
    private void LimitPosition()
    {
        Vector3 currentPosition = transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -220.0f, 220.0f); // clamp the position of the player
        currentPosition.z = Mathf.Clamp(currentPosition.z, -220.0f, 220.0f); // so it doesn't go out of the screen
        currentPosition.y = 0.85f; // ensure that the character is at the appropriate height

        transform.position = currentPosition;
    }
}
