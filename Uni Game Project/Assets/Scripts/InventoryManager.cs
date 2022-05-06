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
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].ItemName == newValues.ItemName)
            {
                // If there is an item of the same type in the inventory, just icrease its amount
                //alreadyExists = true;
                //Debug.Log("Item exists: " + (inventory[i].AmountOfItems + newItem.AmountOfItems));
                inventory[i].AmountOfItems = newValues.AmountOfItems;
                inventory[i].IsKey = newValues.IsKey;
                inventory[i].ItemIcon = newValues.ItemIcon;
                inventory[i].Serial = newValues.Serial;
                //inventory[i].= newItem.AmountOfItems;

                // Update the UI
                uiItemCounter.text = inventory[i].AmountOfItems.ToString();

                break;
            } else
            {
                return;
            }
        }

        InitializeInventoryPanel();
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
            InventoryItem newItem = new InventoryItem(item.amount,item.name, item.serial, item.isKey, (Sprite)AssetDatabase.LoadAssetAtPath(item.iconPath, typeof(Sprite)));
            AddItem(newItem);
        }
        InitializeInventoryPanel();
        RefreshUI();
    }
}