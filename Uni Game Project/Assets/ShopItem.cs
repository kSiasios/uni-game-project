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
        if (Availability > 0)
        {
            shopManager.ItemImage.sprite = Item.ItemIcon;
            shopManager.ItemName.text = Item.ItemName;
            shopManager.ItemPrice.text = Price.ToString();
            shopManager.currentAmount = 1;
            shopManager.ItemAmount.text = shopManager.currentAmount.ToString();
        }
        else
        {
            shopManager.ItemImage.sprite = null;
            shopManager.ItemName.text = "";
            shopManager.ItemPrice.text = "";
            shopManager.currentAmount = 1;
            shopManager.ItemAmount.text = "0";
        }
    }

    public void FlushItemInfoPanel()
    {
        shopManager.ItemImage.sprite = null;
        shopManager.ItemName.text = "";
        shopManager.ItemPrice.text = "";
        shopManager.currentAmount = 1;
        shopManager.ItemAmount.text = "0";
    }

    public void UpdateItemInfoPanel()
    {
        shopManager.CurrentlyDisplaying = this;
        UpdateItemInfoUI();
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
            GetComponent<Button>().onClick.AddListener(UpdateItemInfoPanel);
        }
    }

    public override string ToString()
    {
        return $"Availability: {Availability}, Currency: {Currency}, Price: {Price}, Item: {Item}";
    }
}