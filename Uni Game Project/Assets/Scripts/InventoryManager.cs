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

    [SerializeField] AudioClip collectClip;

    [SerializeField] Sprite[] potentialItemSprites;

    //[SerializeField] Gam

    public enum Currencies
    {
        coins, energy
    }

    Currencies defaultCurrency = Currencies.energy;

    int amountPlaceholder = 0;

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

    private void FixedUpdate()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (var item in audioSources)
        {
            if (!item.isPlaying)
            {
                Destroy(item);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollectItem(collision);
    }

    private void CollectItem(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collectable"))
        {


            // Add object to inventory
            InventoryItem item = Instantiate(inventoryItemPrefab).GetComponent<InventoryItem>();

            amountPlaceholder = item.AmountOfItems;

            item.AmountOfItems = 1;

            // Update the UI

            // Send Notification
            Collectable collectable = collision.gameObject.GetComponent<Collectable>();
            if (collectable != null)
            {
                //Debug.Log($"Collected {collectable.GetName()}");
                item.AmountOfItems = collectable.GetAmount();
                //Debug.Log(AssetDatabase.GetAssetPath(collectable.GetIcon()));
                item.ItemIcon = collectable.GetIcon();
                //Debug.Log(item.AmountOfItems);

                notificationManager.NewNotification(collectable.ToString(), item.ItemIcon);
                if (collectable.GetSerial() != null && collectable.isKey)
                {
                    // It is a key collectable, initialize the serial with the correct value
                    item.Serial = collectable.GetSerial();
                }
                else
                {
                    //uiItemCounter.text = (int.Parse(uiItemCounter.text) + item.AmountOfItems).ToString();
                    item.Serial = null;
                }
                // Change the state of the collectable to collected
                collectable.SetState(Collectable.CollectableState.collected);
            }
            else
            {
                notificationManager.NewNotification($"1 x {collision.gameObject.name}", null);
            }

            item.ItemName = collectable.GetName();
            AddItem(item);
            //PrintList(inventory);

            // Play Collect Sound
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.PlayOneShot(collectClip);
        }
    }

    public void AddItem(InventoryItem newItem)
    {
        //Debug.Log($"Sending sprite path: {newItem.ItemIcon}");
        Sprite itemSprite = null;
        if (newItem.ItemName == "Green Key")
        {
            itemSprite = potentialItemSprites[0];
        }
        if (newItem.ItemName == "Red Key")
        {
            itemSprite = potentialItemSprites[1];
        }
        if (newItem.ItemName == "Blue Key")
        {
            itemSprite = potentialItemSprites[2];
        }
        if (newItem.ItemName == "Energy")
        {
            itemSprite = potentialItemSprites[3];
        }
        AddItem(newItem.AmountOfItems, newItem.ItemName, newItem.Serial, newItem.IsKey, itemSprite);
    }

    public void AddItem(int newAmount, string newName, string newSerial, bool newIsKey, Sprite newSprite)
    {
        //Debug.Log($"New Item: {newItem}");
        // Function that handles adding items to the inventory
        Debug.Log($"Adding Item ==> Name: '{newName}', Amount: '{newAmount}', Sprite: '{newSprite}'");
        bool alreadyExists = false;
        // Iterate through the inventory to see if there already is an item of the same type
        foreach (var item in inventory)
        {
            if (item.ItemName.ToLower() == newName.ToLower())
            {
                //Debug.Log($"Item {item.ItemName} exists in the inventory");
                //Debug.Log($"Exists");
                // If there is an item of the same type in the inventory, just icrease its amount
                alreadyExists = true;
                //Debug.Log("Old value: " + (amountPlaceholder));
                //Debug.Log("New value: " + (newItem.AmountOfItems));
                //Debug.Log("Item exists: " + (item.AmountOfItems + newItem.AmountOfItems));
                //item.AmountOfItems = item.AmountOfItems + newItem.AmountOfItems;
                EditItem(newAmount, newName, newSerial, newIsKey, newSprite);
                //PrintInventory();
                // Update the UI
                if (item.ItemName.ToLower() == defaultCurrency.ToString().ToLower())
                {
                    //uiItemCounter.text = item.AmountOfItems.ToString();
                    //uiItemCounter.text = (int.Parse(uiItemCounter.text) + item.AmountOfItems).ToString();
                    uiItemCounter.text = item.AmountOfItems.ToString();
                }

                break;
            }
        }
        if (!alreadyExists)
        {
            // If there is not an item of this type in the inventory, create one
            InventoryItem newItem = Instantiate(inventoryItemPrefab).GetComponent<InventoryItem>();
            newItem.Setter(newAmount, newName, newSerial, newIsKey, newSprite);
            inventory.Add(newItem);
            //foreach (var inventoryItem in inventory)
            //{
            //    if (inventoryItem.ItemName == newName)
            //    {
            //        Debug.Log($"Found {inventoryItem.ItemName} with sprite: ({inventoryItem.ItemIcon})");
            //    }
            //}
            if (newName.ToLower() == defaultCurrency.ToString().ToLower())
            {
                uiItemCounter.text = newAmount.ToString();
            }
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
        EditItem(newValues.AmountOfItems, newValues.ItemName, newValues.Serial, newValues.IsKey, newValues.ItemIcon);
    }

    private void EditItem(int newAmount, string newName, string newSerial, bool newIsKey, Sprite newSprite)
    {
        // Function that handles editing items to the inventory
        //bool alreadyExists = false;
        // Iterate through the inventory to see if there already is an item of the same type
        foreach (var item in inventory)
        {
            if (item.ItemName == newName)
            {
                // If there is an item of the same type in the inventory, just icrease its amount
                //alreadyExists = true;
                //Debug.Log("Item exists: " + (item.AmountOfItems + newItem.AmountOfItems));

                if (amountPlaceholder > newAmount)
                {
                    Debug.Log($"***** Editing {item.ItemName}, New Amount = item.AmountOfItems({item.AmountOfItems}) + amountPlaceholder({amountPlaceholder}) == {item.AmountOfItems + amountPlaceholder}");
                    item.AmountOfItems += amountPlaceholder;
                }
                else
                {
                    Debug.Log($"***** Editing {item.ItemName}, New Amount = item.AmountOfItems({item.AmountOfItems}) + newAmount({newAmount}) == {item.AmountOfItems + newAmount}");
                    item.AmountOfItems += newAmount;
                }

                item.IsKey = newIsKey;
                item.ItemIcon = newSprite;
                item.Serial = newSerial;
                //item.= newItem.AmountOfItems;

                // Update the UI
                if (item.ItemName.ToLower() == defaultCurrency.ToString().ToLower())
                {
                    UpdateItemCounter(item.AmountOfItems);
                }
                InitializeInventoryPanel();
                return;
            }
        }
    }
    public void UpdateItemCounter(int newAmount)
    {
        uiItemCounter.text = newAmount.ToString();
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
            //Debug.Log($"ItemIcon: {item.ItemIcon}");
            objImage.sprite = item.ItemIcon != null ? item.ItemIcon : objImage.sprite;
            objInfo.ItemIcon = objImage.sprite;
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

        Debug.Log($"Inventory: {inventory}");
        foreach (var item in inventory)
        {
            Debug.Log($"--------- Printing Inventory ====> {item}");
        }

        InventoryData[] saveData = new InventoryData[inventory.Count];
        int i = 0;
        foreach (var item in inventory)
        {
            //Debug.Log($"Saving {item}, ItemIcon: {item.ItemIcon}");
            InventoryData saveItem = new InventoryData(item.ItemName, item.Serial, item.AmountOfItems, item.IsKey, AssetDatabase.GetAssetPath(item.ItemIcon));
            //Debug.Log($"Saved Path {saveItem.iconPath}");
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
            if (item == null)
            {
                continue;
            }
            //Debug.Log($"IconPath = ({item.iconPath})");

            ////InventoryItem newItem = new InventoryItem(item.amount, item.name, item.serial, item.isKey, (Sprite)AssetDatabase.LoadAssetAtPath(item.iconPath, typeof(Sprite)));
            //GameObject newItemGO = Instantiate(inventoryItemPrefab, inventoryGrid.transform);
            InventoryItem newItem = Instantiate(inventoryItemPrefab).GetComponent<InventoryItem>();
            ////InventoryItem newItem = newItemGO.AddComponent<InventoryItem>();
            //Debug.Log($"Loading from path ({item.iconPath})");
            //Sprite itemSprite = (Sprite)AssetDatabase.LoadAssetAtPath(item.iconPath, typeof(Sprite));
            Debug.Log($"Loading Item: {item}");
            //Debug.Log($"Item Reference = ({item.name})");
            newItem.Setter(item.amount, item.name, item.serial, item.isKey, (Sprite)AssetDatabase.LoadAssetAtPath(item.iconPath, typeof(Sprite)));
            //Debug.Log($"Sprite Path AFTER = ({newItem.ItemIcon})");

            AddItem(newItem);
            //Destroy(newItemGO);
        }
        InitializeInventoryPanel();
        RefreshUI();

        // purge extra inventory items
        InventoryItem[] extraInventoryItems = FindObjectsOfType<InventoryItem>();
        foreach (var item in extraInventoryItems)
        {
            Transform parentTransform = item.transform.parent;
            if (parentTransform != null)
            {
                continue;
            }

            Destroy(item.gameObject);
        }
    }
    // to delete
    void RefreshUI()
    {
        //foreach (var item in inventory)
        //{
        //    if (!item.IsKey)
        //    {
        //        uiItemCounter.text = item.AmountOfItems.ToString();
        //    }
        //}
    }

    // to delete
    public void SendNotification(string notificationText, Sprite icon)
    {
        // Push new notification in the notification panel
        notificationManager.NewNotification(notificationText, icon);
    }
}