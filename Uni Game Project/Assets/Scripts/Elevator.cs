using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : InteractableEntity
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
    [SerializeField][Range(1f, 50f)] private float elevatorSpeed = 1;
    [SerializeField] private float stopDistance;
    [SerializeField] private Vector3 travelPosition;

    [Tooltip("Is the elevator moving?")]
    [SerializeField] private bool moving = false;
    [Tooltip("Did the elevator just start moving?")]
    [SerializeField] private bool aboutToMove = false;

    [SerializeField] private float distanceFromEnd;
    [SerializeField] private float distanceFromStart;

    [SerializeField] private Animator elevatorAnimator;

    private void Awake()
    {
        //defaultElevatorPosition = transform.position;
        defaultElevatorPosition = transform.localPosition;
        stopDistance = elevatorSpeed / 100;

        // Fix the axis that is not used to the corresponding value of the elevator
        if (elevatorDirection == ElevatorDirection.vertical)
        {
            //travelPoint.x = transform.position.x;
            travelPoint.x = transform.localPosition.x;
        }
        else
        {
            //travelPoint.y = transform.position.y;
            travelPoint.y = transform.localPosition.y;
        }

        if (elevatorAnimator == null)
        {
            elevatorAnimator = GetComponent<Animator>();
        }

        Physics.IgnoreLayerCollision(0, 2);
        Physics.IgnoreLayerCollision(4, 9);
    }

    private void Update()
    {
        if (elevatorDirection == ElevatorDirection.vertical)
        {
            //distanceFromEnd = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, travelPoint.y));
            distanceFromEnd = Vector2.Distance(new Vector2(0, transform.localPosition.y), new Vector2(0, travelPoint.y));
            //distanceFromStart = Vector2.Distance(new Vector2(0, transform.position.y), new Vector2(0, defaultElevatorPosition.y));
            distanceFromStart = Vector2.Distance(new Vector2(0, transform.localPosition.y), new Vector2(0, defaultElevatorPosition.y));
            //distanceFromStart = Vector2.Distance(transform.position, defaultElevatorPosition);
        }
        else
        {
            //distanceFromEnd = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(travelPoint.x, 0));
            distanceFromEnd = Vector2.Distance(new Vector2(transform.localPosition.x, 0), new Vector2(travelPoint.x, 0));
            //distanceFromStart = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(defaultElevatorPosition.x, 0));
            distanceFromStart = Vector2.Distance(new Vector2(transform.localPosition.x, 0), new Vector2(defaultElevatorPosition.x, 0));
            //distanceFromEnd = Vector2.Distance(transform.position, travelPoint);
            //distanceFromStart = Vector2.Distance(transform.position, defaultElevatorPosition);
        }

        if (distanceFromStart < stopDistance || distanceFromEnd < stopDistance)
        {
            if (!aboutToMove)
            {
                // Prevent elevator from shutting down before reaching its destination
                moving = false;
            }

            if (Input.GetKeyDown(KeyCode.E) && collidingWithPlayer)
            {
                // Activate elevator
                if (distanceFromEnd < stopDistance)
                {
                    // We are in the destination point
                    if (elevatorDirection == ElevatorDirection.vertical)
                    {
                        travelPosition = new Vector3(transform.localPosition.x, defaultElevatorPosition.y, transform.localPosition.z);
                    }
                    else
                    {
                        travelPosition = new Vector3(defaultElevatorPosition.x, transform.localPosition.y, transform.localPosition.z);
                    }
                    moving = true;
                    aboutToMove = true;
                }
                else if (distanceFromStart < stopDistance)
                {
                    // We are in the deafult point
                    if (elevatorDirection == ElevatorDirection.vertical)
                    {
                        travelPosition = new Vector3(transform.localPosition.x, travelPoint.y, transform.localPosition.z);
                    }
                    else
                    {
                        travelPosition = new Vector3(travelPoint.x, transform.localPosition.y, transform.localPosition.z);
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
            //transform.position = transform.position + (transform.position - travelPosition).normalized * Time.deltaTime * -1 * elevatorSpeed;
            //transform.position = transform.position + (transform.position - travelPosition) * Time.deltaTime * -1 * elevatorSpeed;
            //transform.position += (travelPosition - transform.position).normalized * Time.deltaTime  * elevatorSpeed;
            //transform.position = Vector2.MoveTowards(transform.position, travelPosition, elevatorSpeed * Time.deltaTime);
            if (elevatorDirection == ElevatorDirection.vertical)
            {
                if (transform.localPosition.y - travelPosition.y < 0)
                {
                    // About to move upwards
                    elevatorAnimator.SetBool("goingUp", true);
                    elevatorAnimator.SetBool("goingDown", false);
                    elevatorAnimator.SetBool("goingLeft", false);
                    elevatorAnimator.SetBool("goingRight", false);
                }
                else
                {
                    // About to move downwards
                    elevatorAnimator.SetBool("goingUp", false);
                    elevatorAnimator.SetBool("goingDown", true);
                    elevatorAnimator.SetBool("goingLeft", false);
                    elevatorAnimator.SetBool("goingRight", false);
                }
            }
            else
            {
                if (transform.localPosition.x - travelPosition.x < 0)
                {
                    // About to move to the right
                    elevatorAnimator.SetBool("goingDown", false);
                    elevatorAnimator.SetBool("goingUp", false);
                    elevatorAnimator.SetBool("goingLeft", false);
                    elevatorAnimator.SetBool("goingRight", true);
                }
                else
                {
                    // About to move to the left
                    elevatorAnimator.SetBool("goingDown", false);
                    elevatorAnimator.SetBool("goingUp", false);
                    elevatorAnimator.SetBool("goingLeft", true);
                    elevatorAnimator.SetBool("goingRight", false);
                }
            }
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, travelPosition, elevatorSpeed * Time.deltaTime);
            aboutToMove = false;
        }
        else
        {
            elevatorAnimator.SetBool("goingUp", false);
            elevatorAnimator.SetBool("goingDown", false);
            elevatorAnimator.SetBool("goingLeft", false);
            elevatorAnimator.SetBool("goingRight", false);
        }
    }
}
