using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableEntity : MonoBehaviour
{
    [Header("- Interactable Entity Variables")]
    [Space]
    [Tooltip("Is the character colliding with the player?")]
    [SerializeField] protected bool collidingWithPlayer = false;
    [Tooltip("The key the player need to press to interact with this entity.")]
    [SerializeField] protected KeyCode interactionKey = KeyCode.E;
    [Tooltip("Is the player currently interacting with the entity?")]
    [SerializeField] protected bool currentlyInteracting = false;

    protected delegate void FunctionOnInteraction();

    protected FunctionOnInteraction actionOnInteraction;

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collidingWithPlayer = true;
            collision.gameObject.transform.parent = gameObject.transform;

            InteractionPrompt.SetPromptKey(interactionKey.ToString());
            InteractionPrompt.EnableInteractPrompt();
        }
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            collidingWithPlayer = false;
            collision.gameObject.transform.parent = null;
            InteractionPrompt.DisableInteractPrompt();
        }
    }
}
