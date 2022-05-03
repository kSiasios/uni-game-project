using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopKeeper : InteractableCharacter
{

    [Header("- Shop Keeper Variables")]
    [Space]
    public List<ShopItem> inventory = new List<ShopItem>();
    [SerializeField] ShopManager shopManager;
    public GameObject inventoryItemPrefab;

    bool UIActive = false;
    private void Awake()
    {
        base.Awake();
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();
        }
        DeactivateShop();
        actionOnInteraction = ShopKeeperAction;
    }


    private void Update()
    {
        base.Update();
    }
    void ActivateShop()
    {
        shopManager.gameObject.SetActive(true);
    }

    void DeactivateShop()
    {
        shopManager.gameObject.SetActive(false);
    }

    void ShopKeeperAction()
    {
        //Debug.Log("Henlo from ShopKeeper!");
        EnableUI();
        if (GameManager.gameIsPaused)
        {
            // Resume Time
            Time.timeScale = 1f;
            GameManager.gameIsPaused = false;
        }
        else
        {
            Time.timeScale = 0f;
            GameManager.gameIsPaused = true;
        }
    }

    void EnableUI()
    {
        if (UIActive)
        {
            DeactivateShop();
            UIActive = false;
        }
        else
        {
            ActivateShop();
            InitializeShopPanel();
            UIActive = true;
        }
    }

    void InitializeShopPanel()
    {
        foreach (Transform child in shopManager.ShopGrid.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in inventory)
        {
            //Debug.Log($"Creating {item}");
            GameObject obj = Instantiate(inventoryItemPrefab, shopManager.ShopGrid.transform);
            InventoryItem objInfo = obj.GetComponent<InventoryItem>();
            //Debug.Log($"Amount: {item.Item.AmountOfItems}");
            ShopItem shopItemInfo = obj.GetComponent<ShopItem>();

            objInfo.AmountOfItems = item.Item.AmountOfItems;
            objInfo.ItemName = item.Item.ItemName;
            Image objImage = obj.transform.Find("Image").GetComponent<Image>();
            objImage.sprite = item.Item.ItemIcon != null ? item.Item.ItemIcon : objImage.sprite;

            shopItemInfo.Availability = item.Availability;
            shopItemInfo.Price = item.Price;
        }
    }
}