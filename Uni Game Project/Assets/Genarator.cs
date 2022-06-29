using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genarator : Unlockable
{
    [Header("- Generator Script Variables")]
    [Space(20f)]
    //[SerializeField] InventoryItem generatorKey;
    [SerializeField] Collectable generatorKey;

    public enum GeneratorState
    {
        disabled,
        broken,
        enabled
    }

    [SerializeField] GeneratorState generatorState = GeneratorState.broken;

    [SerializeField] GameObject redLight;
    [SerializeField] GameObject greenLight;
    [SerializeField] GameObject whiteLight;

    private new void Awake()
    {
        if (unlockableAnimator == null)
        {
            TryGetComponent(out unlockableAnimator);
        }

        actionOnInteraction = GeneratorFunction;
    }

    private void GeneratorFunction()
    {
        InventoryManager iManager = FindObjectOfType<InventoryManager>();

        //iManager.PrintInventory();

        bool foundKey = false;

        foreach (var item in iManager.inventory)
        {
            if (item.ItemName == generatorKey.GetName() && item.AmountOfItems >= generatorKey.GetAmount())
            {
                foundKey = true;
                Debug.Log("Opening Unlockable...");
                // Use item from inventory
                //iManager.UseItem(item);
                item.AmountOfItems -= generatorKey.GetAmount();
                //iManager.EditItem(item);
                iManager.UpdateItem(item);

                if (playerGameObject != null)
                {
                    playerGameObject.transform.parent = null;
                }

                //transform.gameObject.SetActive(false);
                TriggerUnlockAnimation();
                generatorState = GeneratorState.enabled;

                ActivateEndGame();
                if(shouldProgressStory)
                {
                    GameManager.gameState++;
                }
                FindObjectOfType<GameManager>(true).SaveGame();
                break;
            }
        }

        if (!foundKey)
        {
            //Debug.Log("You don't have a key!");
            iManager.SendNotification($"You don't have enough {generatorKey.GetName()}!", null);
        }
    }

    private void FixedUpdate()
    {
        switch (generatorState)
        {
            case GeneratorState.disabled:
                greenLight.SetActive(false);
                redLight.SetActive(false);
                whiteLight.SetActive(false);
                break;
            case GeneratorState.broken:
                greenLight.SetActive(false);
                redLight.SetActive(true);
                whiteLight.SetActive(true);
                break;
            case GeneratorState.enabled:
                greenLight.SetActive(true);
                redLight.SetActive(false);
                whiteLight.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void ActivateEndGame()
    {
        // Find and activate object that triggers endgame
        FindObjectOfType<EndGameTrigger>(true).gameObject.SetActive(true);
    }

    public void SetState(GeneratorState state)
    {
        generatorState = state;
    }

    //private void TriggerUnlockAnimation()
    //{
    //    if (unlockableAnimator != null)
    //    {
    //        unlockableAnimator.SetBool("unlock", true);
    //    }

    //    clipOnUnlock.PlayClip(gameObject);
    //}

    //private void DisableGameObject()
    //{
    //    transform.gameObject.SetActive(false);
    //}
}
