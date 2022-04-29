using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Tooltip("The script that controls the player")]
    [SerializeField] PlayerController controller;

    private float horizontalAxis = 0;       // -1 if the player is moving to the left, 1 if player is moving to the right
    private float verticalAxis = 0;         // -1 if the player is moving downwards, 1 if player is moving upwards
    private bool jumping = false;
    private float defaultPlayerSpeed;
    private float slowSpeed = 2;

    private void Awake()
    {
        // If the player controller is not initialized, try to find it in the current GameObject
        if (controller == null)
        {
            controller = FindObjectOfType<PlayerController>();
        }

        defaultPlayerSpeed = controller.GetSpeed();
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            FindObjectOfType<GameManager>().SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            FindObjectOfType<GameManager>().LoadGame();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            controller.Reset();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            controller.SetSpeed(slowSpeed);
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            controller.SetSpeed(defaultPlayerSpeed);
        }

        //if (Input.GetKeyDown(KeyCode.F))
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1"))
        {
            if (!GameManager.interacting)
            {
                controller.SetAnimatorBool("isShooting", true);
            }
        }
        else
        {
            controller.SetAnimatorBool("isShooting", false);
        }

        //if(Mathf.Abs(horizontalAxis) < 0.1 && Mathf.Abs(verticalAxis) < 0.1)
        //{
        //    controller.stopPlayer();
        //    Debug.Log("STOPPING PLAYER");
        //}
    }

    // FixedUpdate is called a fixed amount of times per second
    void FixedUpdate()
    {
        // Move the player using the controller script
        controller.MovePlayer(new Vector2(horizontalAxis, verticalAxis), jumping);
        jumping = false;
    }
}
