using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeManager : MonoBehaviour
{
    [Tooltip("The script that controls the player.")]
    [SerializeField] private PlayerController playerController;
    [Tooltip("The Collider of the slope.")]
    [SerializeField] private PolygonCollider2D slopeCollider;
    [Tooltip("The Sprite Renderer responsible for rendering the slope.")]
    [SerializeField] private SpriteRenderer slopeSpriteRenderer;
    [Tooltip("Should the slope be enabled or disabled on Start? (enabled by default).")]
    [SerializeField] private SlopeStartingState startingState = SlopeStartingState.enabled;
    [Tooltip("How deep is the slope on the Z-axis? (middle by default)")]
    [SerializeField] private SlopeDepthDistance slopeDepthDistance = SlopeDepthDistance.middle;

    [SerializeField] private bool canBeActivated = true;

    private enum SlopeStartingState {enabled, disabled};
    private enum SlopeDepthDistance { middle, back, front };

    private bool hasJumped;
    private void Awake()
    {
        if (slopeCollider == null)
        {
            slopeCollider = gameObject.transform.parent.GetComponent<PolygonCollider2D>();
        }

        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        if (slopeSpriteRenderer == null)
        {
            slopeSpriteRenderer = gameObject.transform.parent.GetComponent<SpriteRenderer>();
        }

        if (startingState == SlopeStartingState.disabled)
        {
            DisableCollider();
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("TRIGGER SLOPE");
        if (collision.gameObject == playerController.gameObject)
        {
            //Debug.Log("TRIGGER Player is Jumping: " + playerController.GetIfJumping());
            // If colliding with player
            if (playerController.GetIfJumping())
            {
                hasJumped = true;
            }
            if (hasJumped && canBeActivated)
            {
                // If player jumps ==> activate collider and change sorting layer to 50
                EnableCollider();
            }
            else
            {
                // Else ==> Disable collider and change sorting layer to 30
                DisableCollider();
            }
        }
    }

    private void EnableCollider()
    {
        slopeCollider.enabled = true;
        switch (slopeDepthDistance)
        {
            case SlopeDepthDistance.front:
                slopeSpriteRenderer.sortingOrder = 52;
                break;
            case SlopeDepthDistance.middle:
                slopeSpriteRenderer.sortingOrder = 51;
                break;
            case SlopeDepthDistance.back:
                slopeSpriteRenderer.sortingOrder = 50;
                break;
            default:
                break;
        }
    }

    private void DisableCollider()
    {
        slopeCollider.enabled = false;
        //slopeSpriteRenderer.sortingOrder = 30;
        switch (slopeDepthDistance)
        {
            case SlopeDepthDistance.front:
                slopeSpriteRenderer.sortingOrder = 30;
                break;
            case SlopeDepthDistance.middle:
                slopeSpriteRenderer.sortingOrder = 29;
                break;
            case SlopeDepthDistance.back:
                slopeSpriteRenderer.sortingOrder = 28;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hasJumped = false;
    }
}
