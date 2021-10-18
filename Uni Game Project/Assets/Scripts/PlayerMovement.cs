using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] PlayerController controller;

    private float horizontalAxis = 0;
    private float verticalAxis = 0;
    private bool jumping = false;

    private void Awake()
    {
        // If the player controller is not initialized, try to find it in the current GameObject
        if (controller == null)
        {
            controller = FindObjectOfType<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        horizontalAxis = Input.GetAxisRaw("Horizontal");
        verticalAxis = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Jump"))
        {
            jumping = true;
        }
    }

    void FixedUpdate() {
        //rb.velocity = new Vector2(horizontalAxis * speed, rb.velocity.y);
        //if (jumping) {
        //    Debug.Log("Jumping");
        //    rb.velocity = new Vector2(rb.velocity.x, speed);
        //    jumping = false;
        //}

        controller.MovePlayer(new Vector2(horizontalAxis, verticalAxis), jumping);
        jumping = false;
    }
}
