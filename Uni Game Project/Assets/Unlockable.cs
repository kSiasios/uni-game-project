using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : InteractableEntity
{
    [SerializeField] string serial = "0000";

    private void Awake()
    {
        actionOnInteraction = UnlockableFunction;
    }

    private void UnlockableFunction()
    {
        InventoryManager iManager = FindObjectOfType<InventoryManager>();

        //iManager.PrintInventory();

        foreach (var item in iManager.inventory)
        {
            if (item.Serial != null && item.Serial == serial)
            {
                Debug.Log("Opening Unlockable...");

                if (playerGameObject != null)
                {
                    playerGameObject.transform.parent = null;
                }

                transform.gameObject.SetActive(false);
                break;
            }
        }

        if (transform.gameObject.activeInHierarchy)
        {
            Debug.Log("You don't have a key!");
        }
    }
}
