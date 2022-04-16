using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footstepGravel;
    [SerializeField] private AudioClip footstepMetal;
    [SerializeField] private AudioClip footstepWood;

    [SerializeField] private BoxCollider2D groundCheck;
    private GroundCheck groundCheckFunctions;

    [SerializeField] private LayerMask touchingLayer;

    private LayerMask metal;
    private LayerMask wood;
    private LayerMask ground;

    private void Awake()
    {
        metal = LayerMask.NameToLayer("Metal");
        wood = LayerMask.NameToLayer("Wood");
        ground = LayerMask.NameToLayer("Ground");
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck").GetComponent<BoxCollider2D>();
            groundCheckFunctions = groundCheck.GetComponent<GroundCheck>();
        }
    }

    public void PlaySound(string soundName)
    {
        //Debug.Log("Playing sound: " + soundName);
        //audioSource.Stop();
        //audioSource.Play();
        if (soundName == "footstep")
        {
                Debug.Log("Touching: " + LayerMask.LayerToName(groundCheckFunctions.GetCurrentlyTouchingLayers()));
            if (checkLayer(ground))
            {
                audioSource.PlayOneShot(footstepGravel);
            }
            else if (checkLayer(wood))
            {
                audioSource.PlayOneShot(footstepWood);
            }
            else
            {
                audioSource.PlayOneShot(footstepMetal);
                //audioSource.Play();
            }
        }
    }

    // Function that returns true if the player is touching the given layerMask
    private bool checkLayer(LayerMask target)
    {
        if (groundCheckFunctions.GetCurrentlyTouchingLayers() == target)
        {
            return true;
        }
        return false;
    }
}
