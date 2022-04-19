using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    private LayerMask currentlyTouching;
    [SerializeField] private LayerMask ignoreLayers;
    //[SerializeField] private LayerMask newLayerMask;

    LayerMask groundLayerMask = PlayerController.GetGroundLayerMask();

    [SerializeField] new Collider2D collider;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(9,9);
        //Physics.IgnoreLayerCollision(0, 1);
        //Physics.IgnoreLayerCollision(ignoreLayers);

        collider = GetComponent<Collider2D>();
        
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
        //currentlyTouching = collision.gameObject.layer;
        //Debug.Log(ignoreLayers.value);
        //if (ignoreLayers.ToString())
        //{

        //}
        //SetCurrentlyTouchingLayers(collision.gameObject.layer);
        //newLayerMask.value = 96;
        //Debug.Log("Layer INT: " + collision.gameObject.layer);

        int collisionLayerMask = (int)Mathf.Pow(2,collision.gameObject.layer);

        //if ((collisionLayerMask & ignoreLayers.value) == collisionLayerMask)
        //{
        //    Debug.Log("IGNORE!");
        //} else
        if (!GetIsTouching(LayerMask.GetMask(LayerMask.LayerToName(collision.gameObject.layer))))
        {
            SetCurrentlyTouchingLayers(collision.gameObject.layer);
        }
    }

    public bool GetIsTouching(LayerMask layers)
    {
        return collider.IsTouchingLayers(layers);
    }

    //public bool GetIsGrounded()
    //{
    //    //return GetIsTouching(groundLayerMask);
    //}
}
