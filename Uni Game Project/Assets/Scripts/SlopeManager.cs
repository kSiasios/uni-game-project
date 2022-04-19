using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopeManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PolygonCollider2D slopeCollider;
    [SerializeField] private SpriteRenderer slopeSpriteRenderer;

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
            if (hasJumped)
            {
                // If player jumps ==> activate collider and change sorting layer to 50
                slopeCollider.enabled = true;
                slopeSpriteRenderer.sortingOrder = 50;
            }
            else
            {
                // Else ==> Disable collider and change sorting layer to 30
                slopeCollider.enabled = false;
                slopeSpriteRenderer.sortingOrder = 30;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hasJumped = false;
    }
}
