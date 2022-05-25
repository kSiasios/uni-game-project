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

    InventoryItem previouslyBoughtItem;
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
        if (shopManager.JustBoughtItem != null)
        {
            // loop through inventory and decrease the amount of the given item
            foreach (var item in inventory)
            {
                if (item.Item.ItemName == shopManager.JustBoughtItem.ItemName)
                {
                    item.Availability = item.Availability - shopManager.JustBoughtItem.AmountOfItems;
                    if (shopManager.CurrentlyDisplaying != null)
                    {
                        shopManager.CurrentlyDisplaying.Availability = item.Availability;
                        shopManager.CurrentlyDisplaying.Item.ItemIcon = item.Item.ItemIcon;
                    }
                    EnableUI(true);
                    break;
                }
            }
            previouslyBoughtItem = shopManager.JustBoughtItem;
        }

        if (!GameManager.gameIsPaused && shopManager.gameObject.activeInHierarchy)
        {
            EnableUI(true);
        }
    }
    void ActivateShop()
    {
        shopManager.gameObject.SetActive(true);
        shopManager.FlushItemInfoPanel();

    }

    void DeactivateShop()
    {
        shopManager.gameObject.SetActive(false);
    }

    void ShopKeeperAction()
    {
        //Debug.Log("Henlo from ShopKeeper!");
        //EnableUI();
        //if (GameManager.gameIsPaused)
        //{
        //    // Resume Time
        //    Time.timeScale = 1f;
        //    GameManager.gameIsPaused = false;
        //}
        //else
        //{
        //    Time.timeScale = 0f;
        //    GameManager.gameIsPaused = true;
        //}

        if (!GameManager.gameIsPaused && shopManager.gameObject.activeInHierarchy)
        {
            EnableUI(true);
        }
        //if (GameManager.gameIsPaused && !shopManager.isActiveAndEnabled)
        //{
        //    EnableUI(false);
        //}

        if (!GameManager.gameIsPaused && shopManager.gameObject.activeInHierarchy)
        {
            EnableUI(true);
        }
        if (!GameManager.gameIsPaused || shopManager.gameObject.activeInHierarchy)
        {
            if (GameManager.gameIsPaused)
            {
                // Resume Time
                EnableUI(false);
            }
            else
            {
                EnableUI(true);
            }
        }
    }

    void EnableUI(bool enable)
    {

        if (!enable)
        {
            Time.timeScale = 1f;
            GameManager.gameIsPaused = false;
            DeactivateShop();
            UIActive = false;

        }
        else
        {
            Time.timeScale = 0f;
            GameManager.gameIsPaused = true;
            ActivateShop();
            InitializeShopPanel();
            UIActive = true;
        }
    }

    void InitializeShopPanel()
    {
        //Debug.Log("Initializing Shop Panel");

        foreach (Transform child in shopManager.ShopGrid.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var item in inventory)
        {
            if (item.Availability <= 0)
            {
                inventory.Remove(item);
                InitializeShopPanel();
                //shopManager.CurrentlyDisplaying.UpdateItemInfoUI();
                shopManager.CurrentlyDisplaying.UpdateItemInfoUI();
                continue;
            }
            //Debug.Log($"Creating {item}");
            GameObject obj = Instantiate(inventoryItemPrefab, shopManager.ShopGrid.transform);
            InventoryItem objInfo = obj.GetComponent<InventoryItem>();
            //Debug.Log($"Amount: {item.Item.AmountOfItems}");
            ShopItem shopItemInfo = obj.GetComponent<ShopItem>();

            objInfo.AmountOfItems = item.Item.AmountOfItems;
            objInfo.ItemName = item.Item.ItemName;
            objInfo.IsKey = item.Item.IsKey;
            objInfo.Serial = item.Item.Serial;

            objInfo.ItemIcon = item.Item.ItemIcon;

            Image objImage = obj.transform.Find("Image").GetComponent<Image>();
            //objImage.sprite = item.Item.ItemIcon != null ? item.Item.ItemIcon : objImage.sprite;
            objImage.sprite = objInfo.ItemIcon;
            //Debug.Log($"IMAGE: {item.Item.ItemIcon}");
            //Debug.Log($"SPRITE: {objImage.sprite}");

            shopItemInfo.Availability = item.Availability;
            shopItemInfo.Price = item.Price;
            shopItemInfo.Currency = item.Currency;
        }

        //shopManager.FlushItemInfoPanel();
    }
}
