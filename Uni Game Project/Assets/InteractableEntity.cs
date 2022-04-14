using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntity : MonoBehaviour
{
    [Tooltip("Is the character colliding with the player?")]
    [SerializeField] protected bool collidingWithPlayer = false;
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collidingWithPlayer = true;
            collision.gameObject.transform.parent = this.gameObject.transform;
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collidingWithPlayer = false;
            collision.gameObject.transform.parent = null;
        }
    }
}
