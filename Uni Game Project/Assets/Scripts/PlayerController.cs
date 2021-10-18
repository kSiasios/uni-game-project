using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Core Elements")]
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] BoxCollider2D groundCheck;
    [SerializeField] LayerMask groudLayerMask;

    [Header("Movement Specific Variables")]
    [Range(0f, 10f)] [SerializeField] float speed = 10f;

    private void Awake()
    {
        // If the rigidbody is not initialized, try to find it in the current GameObject
        if (rigidbody == null)
        {
            rigidbody = FindObjectOfType<Rigidbody2D>();
        }

        if (groundCheck == null)
        {
            groundCheck = transform.FindChild("GroundCheck").GetComponent<BoxCollider2D>();
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
        //Debug.Log("Moving 2");
        rigidbody.velocity = new Vector2(direction.x * speed, rigidbody.velocity.y);
        if (jumping && groundCheck.IsTouchingLayers(groudLayerMask)) rigidbody.velocity = new Vector2(rigidbody.velocity.x, speed);
    }
}
