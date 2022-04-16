using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private LayerMask currentlyTouching;
    private void Awake()
    {
        Physics.IgnoreLayerCollision(9,9);
    }
    public LayerMask GetCurrentlyTouchingLayers()
    {
        return currentlyTouching;
    }

    public void SetCurrentlyTouchingLayers(LayerMask value)
    {
        currentlyTouching = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentlyTouching = collision.gameObject.layer;
    }
}
