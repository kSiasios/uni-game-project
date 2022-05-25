using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorButton : InteractableEntity
{
    [SerializeField] Elevator elevator;

    AudioSource audioSource;
    [SerializeField] AudioClip buttonClick;

    Animator animator;

    Vector3 position;

    bool interactable = true;


    private void Awake()
    {
        //elevator = transform.parent.GetComponentInChildren<Elevator>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        actionOnInteraction = CallElevator;
    }

    void CallElevator()
    {
        animator.SetBool("Call", true);
        if (interactable)
        {
            //Debug.Log($"Call elevator to position: {position}");
            //elevator.MoveToPosition(position);
            elevator.MoveElevator();
        }
        else
        {
            Debug.Log("Disabled button");
        }
    }

    public void ResetAnimator()
    {
        animator.SetBool("Call", false);
    }

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonClick);
    }

    // Setters
    public Vector3 Position { set => position = value; }
    public Elevator Elevator { set => elevator = value; }
    public bool Interactable { get => interactable; set => interactable = value; }
}
