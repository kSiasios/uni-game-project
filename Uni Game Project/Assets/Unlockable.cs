using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : InteractableEntity
{
    [Header("- Unlockable Script Variables")]
    [Space(20f)]
    [SerializeField] string serial = "0000";
    [SerializeField] protected Animator unlockableAnimator;

    [SerializeField] protected CustomAudioClip clipOnUnlock;

    protected void Awake()
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
            //Debug.Log("You don't have a key!");
            iManager.SendNotification("You don't have a key!", null);
        }
    }

    protected void TriggerUnlockAnimation()
    {
        if (unlockableAnimator != null)
        {
            unlockableAnimator.SetBool("unlock", true);
        }

        clipOnUnlock.PlayClip(gameObject);
    }

    protected void DisableGameObject()
    {
        transform.gameObject.SetActive(false);
    }
}
