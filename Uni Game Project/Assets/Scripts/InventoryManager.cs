using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] List<InventoryItem> inventory = new List<InventoryItem>();
    [Tooltip("The collider responsible for detecting objects")]
    [SerializeField] CircleCollider2D col;

    [SerializeField] TextMeshProUGUI uiItemCounter;

    [SerializeField] NotificationManager notificationManager;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectable"))
        {
            // Add object to inventory
            InventoryItem item = new InventoryItem();
            item.SetAmount(1);
            
            // Update the UI
            uiItemCounter.text = "1";

            // Send Notification
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                SendNotification(collectable.ToString());
            } else
            {
                SendNotification("1" + " x " + collision.gameObject.name);
            }

            item.SetName(collision.name);
            Add(item);
            // Destroy object after collected
            Destroy(collision.gameObject);
        }
    }

    void SendNotification(string notificationText)
    {
        notificationManager.NewNotification(notificationText);
    }

    void Add(InventoryItem newItem)
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
                inventory[i].SetAmount(inventory[i].AmountOfItems + newItem.AmountOfItems);

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

        PrintList(inventory);
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
                inventory[index].SetAmount(inventory[index].AmountOfItems - amount);

                // Update the UI
                uiItemCounter.text = inventory[index].AmountOfItems.ToString();
            }
            else
            {
                inventory[index].SetAmount(0);

                // Update the UI
                uiItemCounter.text = "0";
            }

        }
    }

    void PrintList(List<InventoryItem> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            print(list[i].ToString());
        }
    }
}

// Custom class that represents the items that are stored in the inventory
public class InventoryItem
{
    public InventoryItem(int amount, string name)
    {
        AmountOfItems = amount;
        ItemName = name;
    }

    public InventoryItem()
    {
        AmountOfItems = 0;
        ItemName = "New Item";
    }

    public int AmountOfItems { get; set; }
    public string ItemName { get; set; }

    public void SetAmount(int amount)
    {
        Debug.Log("Setting Amount to: " + amount);
        AmountOfItems = amount;
    }

    public void SetName(string name)
    {
        ItemName = name;
    }

    public override string ToString()
    {
        return $"{ItemName}: {AmountOfItems}";
    }
}