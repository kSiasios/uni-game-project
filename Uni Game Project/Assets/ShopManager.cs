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
    [Header("\"Not Enough\" Dialog Object")]
    [Space]
    [SerializeField] GameObject notEnoughDialog;
    [SerializeField] TextMeshProUGUI notEnoughDialogText;
    [SerializeField] Button notEnoughOKButton;

    [SerializeField] ShopItem currentlyDisplaying;
    [SerializeField] InventoryItem justBoughtItem;

    [SerializeField] AudioClip purchaseSound;

    InventoryManager inventoryManager;

    public int currentAmount = 1;
    string originalConfirmDialogText;

    string originalNotEnoughDialogText;

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
        if (CurrentlyDisplaying != null)
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
            if (currentAmount >= CurrentlyDisplaying.Availability)
            {
                // disable add button
                IncreaseAmountButton.interactable = false;
            }
            else
            {
                IncreaseAmountButton.interactable = true;
            }
            BuyButton.interactable = true;
        }
        else
        {
            BuyButton.interactable = false;
        }
    }
    
    private void FixedUpdate()
    {
        // Purge AudioSources that don't play
        AudioSource[] audioSources = GetComponents<AudioSource>();
        foreach (var item in audioSources)
        {
            if (!item.isPlaying)
            {
                Destroy(item);
            }
        }
    }
    void AddAmount()
    {
        int newAmount = CurrentlyDisplaying.Availability >= currentAmount + 1 ? currentAmount + 1 : currentAmount;
        currentAmount = newAmount;
        ItemAmount.text = newAmount.ToString();
    }

    void SubtractAmount()
    {
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
        BuyItem();
    }

    void TriggerTransaction()
    {
        // Activate Dialog
        string text = ConfirmDialogText.text;

        // Replace {number} with the actual amount in the input field
        // Replace {item} with the item's name
        // Replace {amount} with the cost * amount of items
        // Replace {currency} with the currency used to buy the item
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
        Time.timeScale = 1f;
        GameManager.gameIsPaused = false;
        gameObject.SetActive(false);

    }

    void BuyItem()
    {
        //Debug.Log(CurrentlyDisplaying);
        //Debug.Log(currentAmount);
        //CurrentlyDisplaying.Item.AmountOfItems = currentAmount;
        // Find currency and subtract from it.
        foreach (var item in inventoryManager.inventory)
        {
            // if the inventory contains the currency at which the item is sold
            if (item.ItemName.ToLower() == CurrentlyDisplaying.Currency.ToString().ToLower())
            {
                Debug.Log($"item.AmountOfItems({item.AmountOfItems - Mathf.RoundToInt(CurrentlyDisplaying.Price * currentAmount)}) = item.AmountOfItems({item.AmountOfItems}) - Mathf.RoundToInt(CurrentlyDisplaying.Price * currentAmount)({Mathf.RoundToInt(CurrentlyDisplaying.Price * currentAmount)});");
                item.AmountOfItems = item.AmountOfItems - Mathf.RoundToInt(CurrentlyDisplaying.Price * currentAmount);
                //Debug.Log($"Item: {item}");
                //inventoryManager.AddItem(item);
                item.SetAmount(item.AmountOfItems);
                //inventoryManager.EditItem(item);
                inventoryManager.UpdateItemCounter(item.AmountOfItems);
                CurrentlyDisplaying.Availability = CurrentlyDisplaying.Availability - currentAmount;
                CurrentlyDisplaying.Item.AmountOfItems = currentAmount;
                //Debug.Log($"Currently displaying {CurrentlyDisplaying}");
                //inventoryManager.AddItem(CurrentlyDisplaying.Item);
                inventoryManager.AddItem(
                    CurrentlyDisplaying.Item.AmountOfItems,
                    CurrentlyDisplaying.Item.ItemName,
                    CurrentlyDisplaying.Item.Serial,
                    CurrentlyDisplaying.Item.IsKey,
                    CurrentlyDisplaying.Item.ItemIcon);
                //inventoryManager.AddItem();
                CurrentlyDisplaying.UpdateItemInfoUI();

                if (JustBoughtItem == null)
                {
                    JustBoughtItem = CurrentlyDisplaying.Item;
                }
                if (JustBoughtItem.ItemName == CurrentlyDisplaying.Item.ItemName)
                {
                    JustBoughtItem.AmountOfItems = CurrentlyDisplaying.Item.AmountOfItems;
                }
                else
                {
                    JustBoughtItem = CurrentlyDisplaying.Item;
                }

                currentAmount = 1;
                ConfirmDialog.SetActive(false);
                break;
            }
        }
        //Debug.Log(CurrentlyDisplaying.Item);
        //inventoryManager.AddItem(CurrentlyDisplaying.Item);

        FlushItemInfoPanel();
        CurrentlyDisplaying = null;
        //Debug.Log($"Availability: {CurrentlyDisplaying.Availability}");

        // Play Sound
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(purchaseSound);
    }

    public void FlushItemInfoPanel()
    {
        ItemImage.sprite = null;
        ItemImage.color = new Color(0f, 0f, 0f, 0f);

        ItemName.text = "";
        ItemPrice.text = "";
        currentAmount = 1;
        ItemAmount.text = "0";
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
