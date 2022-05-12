using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> inventory = new List<InventoryItem>();
    [Tooltip("The collider responsible for detecting objects")]
    [SerializeField] CapsuleCollider2D col;

    [SerializeField] TextMeshProUGUI uiItemCounter;

    [SerializeField] NotificationManager notificationManager;

    [SerializeField] GameObject inventoryGrid;
    [SerializeField] GameObject inventoryItemPrefab;

    [SerializeField] InventoryItem placeholderInventoryItem;

    public enum Currencies
    {
        coins, energy
    }

    Currencies defaultCurrency = Currencies.coins;

    private void Awake()
    {
        if (col == null)
        {
            col = GetComponent<CapsuleCollider2D>();
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

        placeholderInventoryItem = GetComponentInChildren<InventoryItem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectable"))
        {
            // Add object to inventory
            //InventoryItem item = new InventoryItem();
            //item.AmountOfItems = 1;

            InventoryItem item = placeholderInventoryItem;
            item.AmountOfItems = 1;

            // Update the UI

            // Send Notification
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                item.AmountOfItems = collectable.GetAmount();
                //if (collectable.GetIcon() != null)
                //{
                //    item.itemIcon = collectable.GetIcon();
                //} else
                //{
                //    item.itemIcon = null;
                //}
                //Debug.Log(AssetDatabase.GetAssetPath(collectable.GetIcon()));
                item.ItemIcon = collectable.GetIcon();
                //Debug.Log(item.itemIcon);

                SendNotification(collectable.ToString(), item.ItemIcon);
                if (collectable.GetSerial() != null && collectable.isKey)
                {
                    // It is a key collectable, initialize the serial with the correct value
                    item.Serial = collectable.GetSerial();
                }
                else
                {
                    uiItemCounter.text = item.AmountOfItems.ToString();
                    item.Serial = null;
                }
                // Change the state of the collectable to collected
                collectable.SetState(Collectable.CollectableState.collected);
            }
            else
            {
                SendNotification("1" + " x " + collision.gameObject.name, null);
            }

            item.ItemName = collectable.GetName();
            AddItem(item);
            //PrintList(inventory);
        }
    }

    void SendNotification(string notificationText, Sprite icon)
    {
        // Push new notification in the notification panel
        notificationManager.NewNotification(notificationText, icon);
    }

    public void AddItem(InventoryItem newItem)
    {
        //Debug.Log($"New Item: {newItem}");
        // Function that handles adding items to the inventory
        bool alreadyExists = false;
        // Iterate through the inventory to see if there already is an item of the same type
        foreach (var item in inventory)
        {
            if (item.ItemName == newItem.ItemName)
            {
                //Debug.Log($"Same name");
                // If there is an item of the same type in the inventory, just icrease its amount
                alreadyExists = true;
                //Debug.Log("Item exists: " + (item.AmountOfItems + newItem.AmountOfItems));
                item.AmountOfItems = item.AmountOfItems + newItem.AmountOfItems;
                EditItem(item);
                PrintInventory();
                // Update the UI
                if (item.ItemName.ToLower() == defaultCurrency.ToString().ToLower())
                {
                    uiItemCounter.text = item.AmountOfItems.ToString();
                }

                break;
            }
        }
        if (!alreadyExists)
        {
            // If there is not an item of this type in the inventory, create one
            inventory.Add(newItem);
        }

        InitializeInventoryPanel();

        //PrintList(inventory);
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

    public void PrintInventory()
    {
        PrintList(inventory);
    }

    public void EditItem(InventoryItem newValues)
    {
        // Function that handles editing items to the inventory
        //bool alreadyExists = false;
        // Iterate through the inventory to see if there already is an item of the same type
        foreach (var item in inventory)
        {
            if (item.ItemName == newValues.ItemName)
            {
                // If there is an item of the same type in the inventory, just icrease its amount
                //alreadyExists = true;
                //Debug.Log("Item exists: " + (item.AmountOfItems + newItem.AmountOfItems));
                item.AmountOfItems = newValues.AmountOfItems;
                item.IsKey = newValues.IsKey;
                item.ItemIcon = newValues.ItemIcon;
                item.Serial = newValues.Serial;
                //item.= newItem.AmountOfItems;

                // Update the UI
                if (item.ItemName.ToLower() == defaultCurrency.ToString().ToLower())
                {
                    uiItemCounter.text = item.AmountOfItems.ToString();
                }
                InitializeInventoryPanel();
                return;
            }
        }
    }

    void InitializeInventoryPanel()
    {
        //Debug.Log("Initializing Inventory Panel");
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
            Image objImage = obj.transform.Find("Image").GetComponent<Image>();
            objImage.sprite = item.ItemIcon != null ? item.ItemIcon : objImage.sprite;
        }
    }

    void RefreshUI()
    {
        foreach (var item in inventory)
        {
            if (!item.IsKey)
            {
                uiItemCounter.text = item.AmountOfItems.ToString();
            }
        }
    }

    public void UseItem(InventoryItem givenItem)
    {
        Debug.Log("Using Item");
        PrintInventory();
        //Remove(item, amount);
        //Remove(item);
        foreach (var item in inventory)
        {
            if (givenItem.ItemName == item.ItemName)
            {
                inventory.Remove(item);
                break;
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
            InventoryData saveItem = new InventoryData(item.ItemName, item.Serial, item.AmountOfItems, item.IsKey, AssetDatabase.GetAssetPath(item.ItemIcon));
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
            InventoryItem newItem = new InventoryItem(item.amount, item.name, item.serial, item.isKey, (Sprite)AssetDatabase.LoadAssetAtPath(item.iconPath, typeof(Sprite)));
            AddItem(newItem);
        }
        InitializeInventoryPanel();
        RefreshUI();
    }
}