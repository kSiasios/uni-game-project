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
    [SerializeField] GameObject itemInfo;
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemPrice;
    [SerializeField] TMP_InputField itemAmount;
    [SerializeField] Button increaseAmountButton;
    [SerializeField] Button decreaseAmountButton;
    [SerializeField] Button buyButton;
    [Header("Dialog Object")]
    [SerializeField] GameObject confirmDialog;
    [SerializeField] TextMeshProUGUI confirmDialogText;
    [SerializeField] Button cancelButton;
    [SerializeField] Button confirmButton;

    [SerializeField] ShopItem currentlyDisplaying;

    InventoryManager inventoryManager;

    public int currentAmount = 1;
    string originalConfirmDialogText;
    private void Awake()
    {
        if (shopGrid == null)
        {
            shopGrid = transform.Find("Backdrop").transform.Find("Grid").gameObject;
        }

        if (itemInfo == null)
        {
            itemInfo = transform.Find("Backdrop").transform.Find("ItemInfo").gameObject;
        }

        if (itemImage == null)
        {
            itemImage = itemInfo.transform.Find("Image").GetComponent<Image>();
        }

        if (itemName == null)
        {
            itemName = itemInfo.transform.Find("ItemName").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        }

        if (itemPrice == null)
        {
            itemPrice = itemInfo.transform.Find("ItemPrice").transform.Find("Value").GetComponent<TextMeshProUGUI>();
        }

        if (itemAmount == null)
        {
            itemAmount = itemInfo.transform.Find("AmountSetter").transform.Find("AmountInput").GetComponent<TMP_InputField>();
            //itemAmount = itemInfo.transform.Find("AmountSetter").transform.Find("AmountInput").transform.Find("Text Area").transform.Find("Text").GetComponent<TMP_InputField>();
        }

        if (decreaseAmountButton == null)
        {
            decreaseAmountButton = itemInfo.transform.Find("AmountSetter").transform.Find("ButtonSubtract").GetComponent<Button>();
        }

        if (increaseAmountButton == null)
        {
            increaseAmountButton = itemInfo.transform.Find("AmountSetter").transform.Find("ButtonAdd").GetComponent<Button>();
        }

        if (buyButton == null)
        {
            buyButton = itemInfo.transform.Find("BuyButton").GetComponent<Button>();
        }

        if (confirmDialog == null)
        {
            confirmDialog = transform.Find("ConfirmDialog").gameObject;
        }

        if (confirmDialogText == null)
        {
            confirmDialogText = confirmDialog.transform.Find("DialogText").GetComponent<TextMeshProUGUI>();
        }

        if (cancelButton == null)
        {
            cancelButton = confirmDialog.transform.Find("ButtonCancel").GetComponent<Button>();
        }

        if (confirmButton == null)
        {
            confirmButton = confirmDialog.transform.Find("ButtonConfirm").GetComponent<Button>();
        }

        confirmDialog.SetActive(false);

        decreaseAmountButton.onClick.AddListener(SubtractAmount);
        increaseAmountButton.onClick.AddListener(AddAmount);

        confirmButton.onClick.AddListener(ConfirmTransaction);
        cancelButton.onClick.AddListener(CancelTransaction);

        buyButton.onClick.AddListener(TriggerTransaction);

        originalConfirmDialogText = confirmDialogText.text;

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
    }

    void TriggerTransaction()
    {
        //Debug.Log("Triggered Transaction");
        // Activate Dialog
        ConfirmDialog.SetActive(true);

        string text = ConfirmDialogText.text;
        // Replace {number} with the actual amount in the input field
        text = text.Replace("{number}", currentAmount.ToString());
        // Replace {item} with the item's name
        text = text.Replace("{item}", CurrentlyDisplaying.Item.ItemName);
        // Replace {amount} with the cost * amount of items
        text = text.Replace("{amount}", (CurrentlyDisplaying.Price * currentAmount).ToString());
        // Replace {currency} with the currency used to buy the item
        text = text.Replace("{currency}", CurrentlyDisplaying.Currency.ToString());

        foreach (var item in inventoryManager.inventory)
        {
            if (item.ItemName.ToLower() == CurrentlyDisplaying.Currency.ToString().ToLower())
            {
                Debug.Log($"Item: {item.ItemName}, Amount: {item.AmountOfItems}");
            }
        }

        ConfirmDialogText.text = text;
    }

    public void DeactivatePanel()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
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
}
