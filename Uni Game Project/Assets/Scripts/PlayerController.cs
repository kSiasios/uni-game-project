using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Core Elements")]
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] BoxCollider2D groundCheck;
    [SerializeField] LayerMask groudLayerMask;

    [SerializeField] bool facingRight = true;

    [Header("Movement Specific Variables")]
    [Range(0f, 10f)] [SerializeField] float speed = 10f;

    private int direction = 1;

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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MovePlayer(Vector2 direction, bool jumping)
    {
        //Debug.Log("Moving 1");
        //Debug.Log(direction);
        MovePlayer(direction, speed, jumping);
    }

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
        if (jumping && groundCheck.IsTouchingLayers(groudLayerMask)) rigidbody.velocity = new Vector2(rigidbody.velocity.x, speed);
    }

    public int GetFacingDirection ()
    {
        if (transform.localScale.x < 0)
        {
            direction = -1;
        } else
        {
            direction = 1;
        }

        return direction;
    }

    public void TakeDamage(int amount)
    {
        // Function that manages health
    }
}
