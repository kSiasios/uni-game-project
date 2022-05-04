using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Item Grid")]
    [SerializeField] GameObject shopGrid;
    [Header("Item Info Panel")]
    [Space]
    [SerializeField] GameObject itemInfo;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemPrice;
    [SerializeField] TMP_InputField itemAmount;
    [SerializeField] Button increaseAmountButton;
    [SerializeField] Button decreaseAmountButton;
    [SerializeField] Button buyButton;
    [Header("Confirm Dialog Object")]
    [Space]
    [SerializeField] GameObject confirmDialog;
    [SerializeField] TextMeshProUGUI confirmDialogText;
    [SerializeField] Button cancelButton;
    [SerializeField] Button confirmButton;
    [Header("Confirm Dialog Object")]
    [Space]
    [SerializeField] GameObject notEnoughDialog;
    [SerializeField] TextMeshProUGUI notEnoughDialogText;
    [SerializeField] Button notEnoughOKButton;

    [SerializeField] ShopItem currentlyDisplaying;

    InventoryManager inventoryManager;

    public int currentAmount = 1;
    string originalConfirmDialogText;

    string originalNotEnoughDialogText;

    InventoryItem justBoughtItem;
    private void Awake()
    {
        if (ShopGrid == null)
        {
            ShopGrid = transform.Find("Backdrop").transform.Find("Grid").gameObject;
        }

        if (ItemInfo == null)
        {
            ItemInfo = transform.Find("Backdrop").transform.Find("ItemInfo").gameObject;
        }

        if (ItemImage == null)
        {
            ItemImage = ItemInfo.transform.Find("Image").GetComponent<Image>();
        }

        if (ItemName == null)
        {
            ItemName = ItemInfo.transform.Find("ItemName").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        }

        if (ItemPrice == null)
        {
            ItemPrice = ItemInfo.transform.Find("ItemPrice").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        }

        if (ItemAmount == null)
        {
            ItemAmount = ItemInfo.transform.Find("AmountSetter").transform.Find("AmountInput").GetComponent<TMP_InputField>();
            //ItemAmount = ItemInfo.transform.Find("AmountSetter").transform.Find("AmountInput").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_InputField>();
        }

        if (DecreaseAmountButton == null)
        {
            DecreaseAmountButton = ItemInfo.transform.Find("AmountSetter").transform.Find("ButtonSubtract").GetComponent<Button>();
        }

        if (IncreaseAmountButton == null)
        {
            IncreaseAmountButton = ItemInfo.transform.Find("AmountSetter").transform.Find("ButtonAdd").GetComponent<Button>();
        }

        if (BuyButton == null)
        {
            BuyButton = ItemInfo.transform.Find("BuyButton").GetComponent<Button>();
        }

        if (ConfirmDialog == null)
        {
            ConfirmDialog = transform.Find("ConfirmDialog").gameObject;
        }

        if (ConfirmDialogText == null)
        {
            ConfirmDialogText = confirmDialog.transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        }

        if (CancelButton == null)
        {
            CancelButton = confirmDialog.transform.Find("ButtonCancel").GetComponent<Button>();
        }

        if (ConfirmButton == null)
        {
            ConfirmButton = confirmDialog.transform.Find("ButtonConfirm").GetComponent<Button>();
        }

        if (NotEnoughDialog == null)
        {
            NotEnoughDialog = transform.Find("NotEnoughDialog").gameObject;
        }

        if (NotEnoughDialogText == null)
        {
            NotEnoughDialogText = notEnoughDialog.transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        }

        if (NotEnoughOKButton == null)
        {
            NotEnoughOKButton = notEnoughDialog.transform.Find("ButtonOK").GetComponent<Button>();
        }

        ConfirmDialog.SetActive(false);
        NotEnoughDialog.SetActive(false);

        DecreaseAmountButton.onClick.AddListener(SubtractAmount);
        IncreaseAmountButton.onClick.AddListener(AddAmount);

        ConfirmButton.onClick.AddListener(ConfirmTransaction);
        CancelButton.onClick.AddListener(CancelTransaction);

        BuyButton.onClick.AddListener(TriggerTransaction);

        NotEnoughOKButton.onClick.AddListener(NotEnoughConfirm);

        originalConfirmDialogText = ConfirmDialogText.text;
        originalNotEnoughDialogText = NotEnoughDialogText.text;

        inventoryManager = FindObjectOfType<InventoryManager>();
    }

    private void Update()
    {
        if (currentAmount == 1)
        {
            // disable subtract button
            DecreaseAmountButton.interactable = false;
        }
        else
        {
            DecreaseAmountButton.interactable = true;
        }

        if (CurrentlyDisplaying != null)
        {
            if (currentAmount == CurrentlyDisplaying.Availability)
            {
                // disable add button
                IncreaseAmountButton.interactable = false;
            }
            else
            {
                IncreaseAmountButton.interactable = true;
            }
        }
    }

    void AddAmount()
    {
        //Debug.Log("Increasing amount");
        //int currentAmount = int.Parse(ItemAmount.text);
        int newAmount = CurrentlyDisplaying.Availability >= currentAmount + 1 ? currentAmount + 1 : currentAmount;
        currentAmount = newAmount;
        ItemAmount.text = newAmount.ToString();
    }

    void SubtractAmount()
    {
        //Debug.Log("Decreasing amount");
        //int currentAmount = int.Parse(ItemAmount.text);
        int newAmount = currentAmount > 1 ? currentAmount - 1 : currentAmount;
        currentAmount = newAmount;
        ItemAmount.text = newAmount.ToString();
    }

    void CancelTransaction()
    {
        Debug.Log("Cancel Transaction");
        ConfirmDialog.SetActive(false);

        // reset variables
        currentAmount = 1;
        ItemAmount.text = currentAmount.ToString();
        ConfirmDialogText.text = originalConfirmDialogText;
    }

    void ConfirmTransaction()
    {
        Debug.Log("Confirm Transaction");
        BuyItem();
    }

    void TriggerTransaction()
    {
        //Debug.Log("Triggered Transaction");
        // Activate Dialog

        string text = ConfirmDialogText.text;
        //// Replace {number} with the actual amount in the input field
        //text = text.Replace("{number}", currentAmount.ToString());
        //// Replace {item} with the item's name
        //text = text.Replace("{item}", CurrentlyDisplaying.Item.ItemName);
        //// Replace {amount} with the cost * amount of items
        //text = text.Replace("{amount}", (CurrentlyDisplaying.Price * currentAmount).ToString());
        //// Replace {currency} with the currency used to buy the item
        //text = text.Replace("{currency}", CurrentlyDisplaying.Currency.ToString());

        text = text.Replace("{number}", currentAmount.ToString())
            .Replace("{item}", CurrentlyDisplaying.Item.ItemName)
            .Replace("{amount}", (CurrentlyDisplaying.Price * currentAmount).ToString())
            .Replace("{currency}", CurrentlyDisplaying.Currency.ToString());

        ConfirmDialog.SetActive(true);
        ConfirmDialogText.text = text;

        bool found = false;
        foreach (var item in inventoryManager.inventory)
        {
            // if the inventory contains the currency at which the item is sold
            if (item.ItemName.ToLower() == CurrentlyDisplaying.Currency.ToString().ToLower())
            {
                found = true;
                Debug.Log($"Item: {item.ItemName}, Amount: {item.AmountOfItems}");
                if (item.AmountOfItems < CurrentlyDisplaying.Price * currentAmount)
                {
                    // not enough of currency, trigger notEnoughDialog
                    TriggerNotEnoughDialog();
                }
                return;
            }
        }

        if (!found)
        {
            TriggerNotEnoughDialog();
        }
    }

    void NotEnoughConfirm()
    {
        // close NotEnoughDialog
        NotEnoughDialogText.text = originalNotEnoughDialogText;
        NotEnoughDialog.gameObject.SetActive(false);

        ConfirmDialogText.text = originalConfirmDialogText;
        ConfirmDialog.gameObject.SetActive(false);
    }

    void TriggerNotEnoughDialog()
    {
        // open NotEnoughDialog
        notEnoughDialog.gameObject.SetActive(true);

        string newText = notEnoughDialogText.text;
        notEnoughDialogText.text = newText.Replace("{currency}", CurrentlyDisplaying.Currency.ToString().ToLower());
    }

    public void DeactivatePanel()
    {
        //Time.timeScale = 1f;
        //gameObject.SetActive(false);

        Time.timeScale = 1f;
        GameManager.gameIsPaused = false;
        gameObject.SetActive(false);

    }

    void BuyItem()
    {
        CurrentlyDisplaying.Item.AmountOfItems = currentAmount;
        inventoryManager.AddItem(CurrentlyDisplaying.Item);
        // Find currency and subtract from it.
        //foreach (var item in inventoryManager.inventory)
        //{
        //    if (item.ItemName.ToLower() == CurrentlyDisplaying.Item.ItemName.ToLower())
        //    {
        //        Debug.Log($"Old Amount: {item.AmountOfItems}");
        //        // Found currency, decrease its amount
        //        item.AmountOfItems = item.AmountOfItems - Mathf.RoundToInt(CurrentlyDisplaying.Price * currentAmount);
        //        Debug.Log($"New Amount: {item.AmountOfItems}");
        //        inventoryManager.EditItem(item);
        //        break;
        //    }
        //}
        foreach (var item in inventoryManager.inventory)
        {
            // if the inventory contains the currency at which the item is sold
            if (item.ItemName.ToLower() == CurrentlyDisplaying.Currency.ToString().ToLower())
            {
                item.AmountOfItems = item.AmountOfItems - Mathf.RoundToInt(CurrentlyDisplaying.Price * currentAmount);
                //Debug.Log($"New Amount: {item.AmountOfItems}");
                inventoryManager.EditItem(item);
                CurrentlyDisplaying.UpdateItemInfoUI();
                break;
            }
        }

        justBoughtItem = CurrentlyDisplaying.Item;
        justBoughtItem.AmountOfItems = currentAmount;

        ConfirmDialog.SetActive(false);
    }

    public GameObject ShopGrid { get => shopGrid; set => shopGrid = value; }
    public GameObject ItemInfo { get => itemInfo; set => itemInfo = value; }
    public Image ItemImage { get => itemImage; set => itemImage = value; }
    public TextMeshProUGUI ItemName { get => itemName; set => itemName = value; }
    public TextMeshProUGUI ItemPrice { get => itemPrice; set => itemPrice = value; }
    public TMP_InputField ItemAmount { get => itemAmount; set => itemAmount = value; }
    public Button IncreaseAmountButton { get => increaseAmountButton; set => increaseAmountButton = value; }
    public Button DecreaseAmountButton { get => decreaseAmountButton; set => decreaseAmountButton = value; }
    public Button BuyButton { get => buyButton; set => buyButton = value; }
    public GameObject ConfirmDialog { get => confirmDialog; set => confirmDialog = value; }
    public TextMeshProUGUI ConfirmDialogText { get => confirmDialogText; set => confirmDialogText = value; }
    public Button CancelButton { get => cancelButton; set => cancelButton = value; }
    public Button ConfirmButton { get => confirmButton; set => confirmButton = value; }
    public ShopItem CurrentlyDisplaying { get => currentlyDisplaying; set => currentlyDisplaying = value; }
    public GameObject NotEnoughDialog { get => notEnoughDialog; set => notEnoughDialog = value; }
    public TextMeshProUGUI NotEnoughDialogText { get => notEnoughDialogText; set => notEnoughDialogText = value; }
    public Button NotEnoughOKButton { get => notEnoughOKButton; set => notEnoughOKButton = value; }
    public InventoryItem JustBoughtItem { get => justBoughtItem; set => justBoughtItem = value; }
}
