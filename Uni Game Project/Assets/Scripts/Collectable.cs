using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public enum CollectableState
    {
        enabled, disabled, collected
    }

    [SerializeField] string itemName = "Item";
    [SerializeField] int amount = 1;
    [SerializeField] CollectableState state = CollectableState.enabled;
    [SerializeField] Sprite collectableIcon;

    public bool isKey = false;
    [SerializeField] string keySerial;
    public string ToString()
    {
        //Debug.Log("ToString() called from Collectable.cs");

        if(isKey)
        {
            return amount + " x " + itemName + ", Serial: " + keySerial;
        }

        return amount + " x " + itemName;
    }

    //public GenericSaveData Save()
    //{
    //    // Save position
    //    // Save amount
    //    // Save state

    //    GenericSaveData saveData = new GenericSaveData(amount, (int)state, transform.position);

    //    return saveData;
    //}

    public void Load(SerializableCollectable data)
    {
        amount = Mathf.RoundToInt(data.amount);
        state = (CollectableState)data.state;
        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        keySerial = data.keySerial;
    }

    public Sprite GetIcon()
    {
        return collectableIcon;
    }

    void Update()
    {
        if ((state == CollectableState.collected || state == CollectableState.disabled) && this.transform.gameObject.activeInHierarchy)
        {
            transform.gameObject.SetActive(false);
        }

        if (state == CollectableState.enabled && !this.transform.gameObject.activeInHierarchy)
        {
            transform.gameObject.SetActive(true);
        }
    }
    
    // Getters
    public int GetAmount()
    {
        return amount;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public CollectableState GetState()
    {
        return state;
    }

    public string GetName()
    {
        return itemName;
    }

    public string GetSerial()
    {
        return keySerial;
    }

    public void SetState(CollectableState value)
    {
        state = value;
    }
}

[System.Serializable]
public class SerializableCollectable
{
    public float amount;
    public float[] position;
    public int state;
    public string keySerial;

    public SerializableCollectable(Collectable collectable)
    {
        amount = collectable.GetAmount();

        position = new float[3];
        Vector3 v3Pos = collectable.GetPosition();
        position[0] = v3Pos.x;
        position[1] = v3Pos.y;
        position[2] = v3Pos.z;

        state = (int)collectable.GetState();

        keySerial = collectable.GetSerial();
    }
}