using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    public float speed; // a variable that will control the speed of the character    
    private Animator animator;

	private void Start ()
    {
        animator = GetComponentInChildren<Animator>();
	}
	
	private void Update ()
    {
        LimitPosition();
        KeyboardMovement();
    }

    /// <summary> Movement done with arrows or WSAD keys </summary>
    private void KeyboardMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // Get the input from the keyboard
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0) // Checks if the current position is not the destination 
        {
            Transform parent = gameObject.GetComponentInChildren<SpriteRenderer>().transform.parent;
            FlipSprite(horizontal, parent);

            Vector3 movement = transform.position + new Vector3(horizontal, 0.0f, vertical); // calculate player's movement
            transform.position = Vector3.Lerp(transform.position, movement, speed); // assign the new position with the speed

            animator.SetBool("isMoving", true);
            return;
        }

        animator.SetBool("isMoving", false);
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
