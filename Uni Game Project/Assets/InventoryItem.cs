using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Custom class that represents the items that are stored in the inventory
public class InventoryItem : MonoBehaviour
{
    [Header("Personal references")]
    [Tooltip("The image component that will control the sprite")]
    [SerializeField] Image imageComponent;

    [Header("References to UI")]
    [Tooltip("References to the ItemInfo UI element and its sub-components")]
    [SerializeField] GameObject itemInfo;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameUI;
    [SerializeField] TextMeshProUGUI itemAmountUI;

    [Header("Item Attributes")]
    [SerializeField] private int amountOfItems;
    [SerializeField] private string itemName;
    [SerializeField] private string serial;
    [SerializeField] private bool isKey;
    [SerializeField] private Sprite itemIcon;

    private void Awake()
    {
        if (imageComponent == null)
        {
            imageComponent = gameObject.GetComponent<Image>();
        }

        if (imageComponent != null)
        {
            if (itemIcon != null)
            {
            //Debug.Log("itemIcon != null");
                imageComponent.sprite = itemIcon;
            }
        }

        if (itemInfo == null)
        {
            // Try and find itemInfo in the hierarchy
            //itemInfo = transform.parent.parent.Find("ItemInfo").gameObject;
            if (transform.parent != null)
            {

                if (transform.parent.name == "Grid")
                {
                    if (GetComponent<ShopItem>())
                    {
                        // we have a shop item, look for item info under shop
                        itemInfo = GameObject.Find("ShopPanel").transform.Find("Backdrop").transform.Find("ItemInfo").gameObject;
                    }
                    else
                    {
                        // we have a regular inventory item, look for item info under inventory
                        itemInfo = GameObject.Find("InventoryPanel").transform.Find("Backdrop").transform.Find("ItemInfo").gameObject;
                    }


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

        }
        else
        {
            itemImage = itemInfo.transform.Find("Image").GetComponentInChildren<Image>();
            itemNameUI = itemInfo.transform.Find("ItemName").transform.Find("Value").GetComponent<TextMeshProUGUI>();
            itemAmountUI = itemInfo.transform.Find("ItemAmount").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        }
    }

    // Constructor with input
    //public InventoryItem(int amount, string name, string serial, bool key, Sprite? sprite = null)
    //{
    //    AmountOfItems = amount;
    //    ItemName = name;
    //    Serial = serial;
    //    isKey = key;
    //    itemIcon = sprite;
    //}

    // Constructor with no given input (Default constructor)
    //public InventoryItem()
    //{
    //    AmountOfItems = 0;
    //    ItemName = "New Item";
    //    Serial = "0000";
    //    isKey = false;
    //    itemIcon = null;
    //}

    // Declaration of the class' attributes along with getters and setters
    public int AmountOfItems { get => amountOfItems; set => amountOfItems = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public string Serial { get => serial; set => serial = value; }
    public bool IsKey { get => isKey; set => isKey = value; }

    //[Tooltip("A sprite that represents the item")]
    //[SerializeField] 
    public Sprite ItemIcon { get => itemIcon; set => itemIcon = value; }

    public override string ToString()
    {
        return $"Name: {ItemName}\n" +
            $"Amount: {AmountOfItems}\t" +
            $"IsKey: {IsKey}\t" +
            $"Serial: {Serial}\t" +
            $"Icon: {ItemIcon}\t";
    }

    public void UpdateItemInfoUI()
    {
        //Debug.Log(itemIcon);
        itemImage.sprite = ItemIcon;
        itemImage.color = new Color(255f, 255f, 255f, 100f);
        itemNameUI.text = ItemName;
        itemAmountUI.text = AmountOfItems.ToString();
    }

    // Setters
    public void Setter(int amount, string name, string serial, bool isKey, Sprite sprite)
    {
        AmountOfItems = amount;
        ItemName = name;
        Serial = serial;
        IsKey = isKey;
        ItemIcon = sprite;

        //imageComponent.sprite = ItemIcon;
    }

    public void SetAmount(int value)
    {
        amountOfItems = value;
        return;
    }


}
