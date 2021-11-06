using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    enum CollectableState
    {
        enabled, disabled, collected
    }

    [SerializeField] string itemName = "Item";
    [SerializeField] int amount = 1;
    [SerializeField] CollectableState state = CollectableState.enabled;

    public string ToString()
    {
        Debug.Log("ToString() called from Collectable.cs");
        return amount + " x " + itemName;
    }

}
