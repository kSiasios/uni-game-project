using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private InventoryItem item;
    [SerializeField] private int availability;
    [SerializeField] private float price;
    [SerializeField] private Currencies currency;

    [Header("UI References")]
    [SerializeField] ShopManager shopManager;
    //[SerializeField] private InventoryItem item;
    //[SerializeField] private int availability;
    //[SerializeField] private float price;

    public enum Currencies
    {
        coins, energy
    }

    public ShopItem(InventoryItem item, int availability, float price)
    {
        this.item = item;
        this.availability = availability;
        this.price = price;
    }

    public InventoryItem Item { get => item; set => item = value; }
    public int Availability { get => availability; set => availability = value; }
    public float Price { get => price; set => price = value; }
    public Currencies Currency { get => currency; set => currency = value; }

    public void UpdateItemInfoUI()
    {
        //Debug.Log(Item.ItemIcon);
        //Debug.Log($"Price: {price}");
        if (Availability > 0)
        {
            shopManager.CurrentlyDisplaying = this;
            shopManager.ItemImage.sprite = Item.ItemIcon;
            //itemImage.sprite = imageComponent.sprite;
            shopManager.ItemName.text = Item.ItemName;
            //itemNameUI.text = ItemName;
            //shopManager.ItemAmount.text = Availability.ToString();

            shopManager.ItemPrice.text = Price.ToString();

            shopManager.currentAmount = 1;
            shopManager.ItemAmount.text = shopManager.currentAmount.ToString();
            //shopManager.ItemAmount.text = shopManager.currentAmount.ToString();
            //itemAmountUI.text = AmountOfItems.ToString();
        }
        else
        {
            shopManager.CurrentlyDisplaying = null;
            shopManager.ItemImage.sprite = null;
            //itemImage.sprite = imageComponent.sprite;
            shopManager.ItemName.text = "";
            //itemNameUI.text = ItemName;
            //shopManager.ItemAmount.text = Availability.ToString();

            shopManager.ItemPrice.text = "";

            shopManager.currentAmount = 1;
            shopManager.ItemAmount.text = "0";
            //shopManager.ItemAmount.text = shopManager.currentAmount.ToString();
            //itemAmountUI.text = AmountOfItems.ToString();
        }

    }

    private void Awake()
    {
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();
        }

        if (item == null)
        {
            item = GetComponent<InventoryItem>();
        }

        if (transform.parent.name == "Grid")
        {
            GetComponent<Button>().onClick.AddListener(UpdateItemInfoUI);
        }
    }
}