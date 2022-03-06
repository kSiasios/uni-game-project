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

        if (Input.GetKeyDown(KeyCode.P))
        {
            FindObjectOfType<GameManager>().SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            FindObjectOfType<GameManager>().LoadGame();
        }
    }

    // FixedUpdate is called a fixed amount of times per second
    void FixedUpdate() {
        // Move the player using the controller script
        controller.MovePlayer(new Vector2(horizontalAxis, verticalAxis), jumping);
        jumping = false;
    }
}
