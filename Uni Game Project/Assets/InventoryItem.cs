using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Custom class that represents the items that are stored in the inventory
public class InventoryItem : MonoBehaviour
{
    [Header("Personal references")]
    [Tooltip("A sprite that represents the item")]
    [SerializeField] Sprite itemIcon;
    [Tooltip("The image component that will control the sprite")]
    [SerializeField] Image imageComponent;
    
    [Header("References to UI")]
    [Tooltip("References to the ItemInfo UI element and its sub-components")]
    [SerializeField] GameObject itemInfo;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameUI;
    [SerializeField] TextMeshProUGUI itemAmountUI;

    private void Awake()
    {
        if (imageComponent == null)
        {
            imageComponent = this.gameObject.GetComponent<Image>();
        }

        if (itemIcon == null)
        {
            Debug.LogError("Error! itemIcon not provided... Script: InventoryItem.cs");
        }
        else
        {
            imageComponent.sprite = itemIcon;
        }

        if (itemInfo == null)
        {
            // Try and find itemInfo in the hierarchy
            itemInfo = this.transform.parent.parent.Find("ItemInfo").gameObject;

            // If found, populate local variables with their according values
            if (itemInfo != null)
            {
                itemImage = itemInfo.transform.Find("Image").GetComponentInChildren<Image>();
                itemNameUI = itemInfo.transform.Find("ItemName").transform.Find("Value").GetComponent<TextMeshProUGUI>();
                itemAmountUI = itemInfo.transform.Find("ItemAmount").transform.Find("Value").GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("ItemInfo not found!");
            }
        }
    }

    // Constructor with input
    public InventoryItem(int amount, string name, string serial, bool key)
    {
        AmountOfItems = amount;
        ItemName = name;
        Serial = serial;
        isKey = key;
    }

    // Constructor with no given input (Default constructor)
    public InventoryItem()
    {
        AmountOfItems = 0;
        ItemName = "New Item";
        Serial = "0000";
        isKey = false;
    }

    // Declaration of the class' attributes along with getters and setters
    public int AmountOfItems { get; set; }
    public string ItemName { get; set; }

    public string Serial { get; set; }

    public bool isKey { get; set; }

    public override string ToString()
    {
        return $"{ItemName}: {AmountOfItems}";
    }

    public void UpdateItemInfoUI()
    {
        itemImage.sprite = itemIcon;
        itemNameUI.text = ItemName;
        itemAmountUI.text = AmountOfItems.ToString();
    }
}
