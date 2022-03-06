using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public enum ElevatorDirection
    {
        vertical, horizontal
    }

    private Vector3 defaultElevatorPosition;

    [Tooltip("The point towards which the elevator will travel")]
    [SerializeField] private Vector3 travelPoint;
    [Tooltip("The axis at which the elevator will travel on")]
    [SerializeField] private ElevatorDirection elevatorDirection = ElevatorDirection.vertical;
    [Tooltip("How fast should the elevator go?")]
    [SerializeField] [Range(1f, 50f)] private float elevatorSpeed = 1;
    private float stopDistance;
    private Vector3 travelPosition;

    [Tooltip("Is the elevator moving?")]
    [SerializeField] private bool moving = false;
    [Tooltip("Did the elevator just start moving?")]
    [SerializeField] private bool aboutToMove = false;
    [Tooltip("Is the elevator colliding with the player?")]
    [SerializeField] private bool collidingWithPlayer = false;

    private void Awake()
    {
        defaultElevatorPosition = transform.position;
        stopDistance = elevatorSpeed / 100;

        // Fix the axis that is not used to the corresponding value of the elevator
        if (elevatorDirection == ElevatorDirection.vertical) {
            travelPoint.x = transform.position.x;
        } else {
            travelPoint.y = transform.position.y;
        }
    }

    private void Update()
    {

        if (Vector3.Distance(transform.position, defaultElevatorPosition) < stopDistance || Vector3.Distance(transform.position, travelPoint) < stopDistance)
        {
            if (!aboutToMove)
            {
                // Prevent elevator from shutting down before reaching its destination
                moving = false;
            }

            if (Input.GetKeyDown(KeyCode.E) && collidingWithPlayer)
            {
                // Activate elevator
                if (Vector3.Distance(transform.position, travelPoint) < stopDistance)
                {
                    // We are in the destination point
                    if (elevatorDirection == ElevatorDirection.vertical)
                    {
                        travelPosition = new Vector3(transform.position.x, defaultElevatorPosition.y);
                    }
                    else
                    {
                        travelPosition = new Vector3(defaultElevatorPosition.x, transform.position.y);
                    }
                    moving = true;
                    aboutToMove = true;
                }
                else if (Vector3.Distance(transform.position, defaultElevatorPosition) < stopDistance)
                {
                    // We are in the deafult point
                    if (elevatorDirection == ElevatorDirection.vertical)
                    {
                        travelPosition = new Vector3(transform.position.x, travelPoint.y);
                    }
                    else
                    {
                        travelPosition = new Vector3(travelPoint.x, transform.position.y);
                    }
                    moving = true;
                    aboutToMove = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (moving)
        {
            transform.position = transform.position + (transform.position - travelPosition).normalized * Time.deltaTime * -1 * elevatorSpeed;
            aboutToMove = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision with " + collision.gameObject.name);
        collision.gameObject.transform.parent = this.gameObject.transform;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collidingWithPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.transform.parent = null;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collidingWithPlayer = false;
        }
    }
}
