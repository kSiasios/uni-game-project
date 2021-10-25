using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Core Elements")]
    [Tooltip("Player's Rigidbody")]
    [SerializeField] new Rigidbody2D rigidbody;
    [Tooltip("Collider that checks if the player is touching the ground layer")]
    [SerializeField] BoxCollider2D groundCheck;
    [Tooltip("The Ground Layer Mask")]
    [SerializeField] LayerMask groudLayerMask;

    [Tooltip("Is the player facing to the right")]
    [SerializeField] bool facingRight = true;

    [Header("Movement Specific Variables")]
    [Tooltip("How fast is the player moving")]
    [Range(0f, 10f)] [SerializeField] float speed = 10f;

    private void Awake()
    {
        // If the rigidbody is not initialized, try to find it in the current GameObject
        if (rigidbody == null)
        {
            rigidbody = transform.GetComponent<Rigidbody2D>();
            //rigidbody = FindObjectOfType<Rigidbody2D>();
        }

        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck").GetComponent<BoxCollider2D>();
        }
    }

    // Move player with the default speed
    public void MovePlayer(Vector2 direction, bool jumping)
    {
        //Debug.Log("Moving 1");
        //Debug.Log(direction);
        MovePlayer(direction, speed, jumping);
    }

    // Move player with custom speed
    public void MovePlayer(Vector2 direction, float speed, bool jumping)
    {
        // If player is moving to the left, flip them to face to the left
        if (direction.x < 0 && facingRight)
        {
            facingRight = false;
            transform.Rotate(0, -180, 0);
        }
        // If player is moving to the right, flip them to face to the right
        else if (direction.x > 0 && !facingRight)
        {
            facingRight = true;
            transform.Rotate(0, 180, 0);
        }

        //Debug.Log("Moving 2");
        rigidbody.velocity = new Vector2(direction.x * speed, rigidbody.velocity.y);
        // If the player is touching the ground, they can jump
        if (jumping && groundCheck.IsTouchingLayers(groudLayerMask)) rigidbody.velocity = new Vector2(rigidbody.velocity.x, speed);
    }

    public void TakeDamage(int amount)
    {
        // Function that manages health
    }
}
