using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : InteractableEntity
{
    [SerializeField] string serial = "0000";
    [SerializeField] Animator unlockableAnimator;

    private void Awake()
    {
        if (unlockableAnimator == null)
        {
            TryGetComponent(out unlockableAnimator);
        }

        actionOnInteraction = UnlockableFunction;
    }

    private void UnlockableFunction()
    {
        InventoryManager iManager = FindObjectOfType<InventoryManager>();

        //iManager.PrintInventory();

        bool foundKey = false;

        foreach (var item in iManager.inventory)
        {
            if (item.Serial != null && item.Serial == serial)
            {
                foundKey = true;
                Debug.Log("Opening Unlockable...");
                // Use item from inventory
                iManager.UseItem(item);

                if (playerGameObject != null)
                {
                    playerGameObject.transform.parent = null;
                }

                //transform.gameObject.SetActive(false);
                TriggerUnlockAnimation();
                break;
            }
        }

        if (!foundKey)
        {
            Debug.Log("You don't have a key!");
        }
    }

    private void TriggerUnlockAnimation()
    {
        if (unlockableAnimator != null)
        {
            unlockableAnimator.SetBool("unlock", true);
        }
    }

    private void DisableGameObject()
    {
        transform.gameObject.SetActive(false);
    }
}
