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
        float horizontal = Input.GetAxisRaw("Horizontal"); // Get the input from the keyboard
        float vertical = Input.GetAxisRaw("Vertical");

        SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        FlipSprite(horizontal, sprites);

        Vector3 movement = rb.transform.position + new Vector3(horizontal, 0.0f, vertical); // calculate player's movement
        if (!movement.Equals(rb.transform.position)) // Checks if the current position is not the destination
        {
            // TODO: Change animation to walking

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
            float screenCenter = Screen.width / 2;
            float mousePosX = Input.mousePosition.x;

            SpriteRenderer[] sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
            FlipSprite(mousePosX - screenCenter, sprites);

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
        }
    }

    /// <summary>
    /// Flips the sprites in the appropriate direction.
    /// If it's an arm of legs, changes its relative position.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="sprites"></param>
    private static void FlipSprite(float direction, SpriteRenderer[] sprites)
    {
        foreach (SpriteRenderer sprite in sprites)
        {
            if (direction > 0)
            {
                Vector3 scale = sprite.transform.localScale;
                if (scale.x < 0)
                { 
                    scale.x *= -1.0f;
                    sprite.transform.localScale = scale;
                }
                /*sprite.flipX = false;
                if (sprite.name.Equals("sword_hand"))
                {
                    sprite.transform.localPosition = new Vector3(-10.0f, -4.5f, 0.0f);
                }
                else if (sprite.name.Equals("legs"))
                {
                    sprite.transform.localPosition = new Vector3(1.0f, -23.5f, 0.0f);
                }*/
            }
            else if (direction < 0)
            {
                Vector3 scale = sprite.transform.localScale;
                if (scale.x > 0)
                {
                    scale.x *= -1.0f;
                    sprite.transform.localScale = scale;
                }
                /*sprite.flipX = true;
                if (sprite.name.Equals("sword_hand"))
                {
                    sprite.transform.localPosition = new Vector3(10.0f, -4.5f, 0.0f); // Changes the relative position of the arm
                }
                else if (sprite.name.Equals("legs"))
                {
                    sprite.transform.localPosition = new Vector3(-1.0f, -23.5f, 0.0f); // Changes the relative position of legs
                }*/
            }
        }
    }

    /// <summary> Limits the position to the boundaries of the level and adjusts the height </summary>
    private void LimitPosition()
    {
        Vector3 currentPosition = rb.transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, -220.0f, 220.0f); // clamp the position of the player
        currentPosition.z = Mathf.Clamp(currentPosition.z, -220.0f, 220.0f); // so it doesn't go out of the screen
        currentPosition.y = 0.85f; // ensure that the character is at the appropriate height

        rb.transform.position = currentPosition;
    }
}
