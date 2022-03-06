using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();
    [Tooltip("The collider responsible for detecting objects")]
    [SerializeField] CircleCollider2D col;

    [SerializeField] TextMeshProUGUI uiItemCounter;

    [SerializeField] NotificationManager notificationManager;

    [SerializeField] GameObject inventoryGrid;
    [SerializeField] GameObject inventoryItemPrefab;

    private void Awake()
    {
        if (col == null)
        {
            col = transform.GetComponent<CircleCollider2D>();
        }

        if (uiItemCounter == null)
        {
            uiItemCounter = GameObject.Find("CollectableCounter").GetComponentInChildren<TextMeshProUGUI>();
            uiItemCounter.text = "0";
        }

        if (notificationManager == null)
        {
            notificationManager = GameObject.Find("NotificationPanel").GetComponentInChildren<NotificationManager>();
        }

        if (inventoryGrid == null)
        {
            inventoryGrid = GameObject.Find("InventoryPanel")
                .transform.Find("Backdrop")
                .transform.Find("Grid").transform.gameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectable"))
        {
            // Add object to inventory
            InventoryItem item = new InventoryItem();
            item.AmountOfItems = 1;

            // Update the UI

            // Send Notification
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                SendNotification(collectable.ToString());
                if (collectable.GetSerial() != null && collectable.isKey)
                {
                    // It is a key collectable, initialize the serial with the correct value
                    item.Serial = collectable.GetSerial();
                }
                else
                {
                    uiItemCounter.text = "1";
                    item.Serial = null;
                }
                // Change the state of the collectable to collected
                collectable.SetState(Collectable.CollectableState.collected);
            }
            else
            {
                SendNotification("1" + " x " + collision.gameObject.name);
            }

            item.ItemName = collectable.GetName();
            AddItem(item);
            PrintList(inventory);
        }
    }

    void SendNotification(string notificationText)
    {
        // Push new notification in the notification panel
        notificationManager.NewNotification(notificationText);
    }

    void AddItem(InventoryItem newItem)
    {
        // Function that handles adding items to the inventory
        bool alreadyExists = false;
        // Iterate through the inventory to see if there already is an item of the same type
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemName == newItem.ItemName)
            {
                // If there is an item of the same type in the inventory, just icrease its amount
                alreadyExists = true;
                Debug.Log("Item exists: " + (inventory[i].AmountOfItems + newItem.AmountOfItems));
                inventory[i].AmountOfItems += newItem.AmountOfItems;

                // Update the UI
                uiItemCounter.text = inventory[i].AmountOfItems.ToString();

                break;
            }
        }
        if (!alreadyExists)
        {
            // If there is not an item of this type in the inventory, create one
            inventory.Add(newItem);
        }

        InitializeInventoryPanel();
    }

    // Remove the whole stack of this item
    void Remove(InventoryItem item)
    {
        inventory.Remove(item);
    }

    // Remove an amount from the stack of this item
    void Remove(InventoryItem item, int amount)
    {
        int index = inventory.BinarySearch(item);
        if (index >= 0)
        {
            if (inventory[index].AmountOfItems >= amount)
            {
                inventory[index].AmountOfItems -= amount;

                // Update the UI
                uiItemCounter.text = inventory[index].AmountOfItems.ToString();
            }
            else
            {
                inventory[index].AmountOfItems = 0;

                // Update the UI
                uiItemCounter.text = "0";
            }

        }
    }

    void PrintList(List<InventoryItem> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            print(list[i].ToString());
        }
    }

    void InitializeInventoryPanel()
    {
        foreach (Transform child in inventoryGrid.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in inventory)
        {
            GameObject obj = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
            InventoryItem objInfo = obj.GetComponent<InventoryItem>();
            objInfo.AmountOfItems = item.AmountOfItems;
            objInfo.ItemName = item.ItemName;
        }
    }

    void RefreshUI()
    {
        foreach (var item in inventory)
        {
            if (!item.isKey)
            {
                uiItemCounter.text = item.AmountOfItems.ToString();
            }
        }
    }

    //public List<InventoryData> Save()
    //{
    //    // Save each item along with their amount
    //    List<InventoryData> saveData = new List<InventoryData>();

    //    foreach (var item in inventory)
    //    {
    //        InventoryData saveItem = new InventoryData(item.ItemName, item.Serial, item.AmountOfItems);
    //        saveData.Add(saveItem);
    //    }

    //    return saveData;
    //}
    public InventoryData[] Save()
    {
        // Save each item along with their amount
        //List<InventoryData> saveData = new List<InventoryData>();

        InventoryData[] saveData = new InventoryData[inventory.Count];
        int i = 0;
        foreach (var item in inventory)
        {
            InventoryData saveItem = new InventoryData(item.ItemName, item.Serial, item.AmountOfItems, item.isKey);
            saveData[i] = saveItem;
            //saveData.Add(saveItem);
        }

        return saveData;
    }

    public void Load(InventoryData[] data)
    {
        inventory.Clear();

        foreach (var item in data)
        {
            InventoryItem newItem = new InventoryItem(item.amount,item.name, item.serial, item.isKey);
            AddItem(newItem);
        }
        InitializeInventoryPanel();
        RefreshUI();
    }
}