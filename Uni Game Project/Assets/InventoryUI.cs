using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Tooltip("Reference to the InventoryPanel object")]
    public GameObject inventoryUI;
    [Tooltip("References to the InventoryPanel child objects object, needed for updating the panel")]
    [SerializeField] Image itemImage;
    [SerializeField] TextMeshProUGUI itemNameUI;
    [SerializeField] TextMeshProUGUI itemAmountUI;

    private void Awake()
    {
        if (inventoryUI == null)
        {
            // if pauseMenuUI is not initialized, initialize it
            inventoryUI = transform.Find("GameplayUI").Find("InventoryPanel").gameObject;

            if (inventoryUI != null)
            {
                itemImage = inventoryUI.transform.Find("Backdrop").transform.Find("ItemInfo")
                    .transform.Find("Image").GetComponentInChildren<Image>();

                itemNameUI = inventoryUI.transform.Find("Backdrop").transform.Find("ItemInfo")
                    .transform.Find("ItemName").transform.Find("Value").GetComponent<TextMeshProUGUI>();

                itemAmountUI = inventoryUI.transform.Find("Backdrop").transform.Find("ItemInfo")
                    .transform.Find("ItemAmount").transform.Find("Value").GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("Something went wrong! UI element(s) missing. (InventoryUI.cs)");
            }
        }

        if (inventoryUI.activeInHierarchy)
        {
            inventoryUI.SetActive(false);
        }

        ResetInventoryPanel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gameIsPaused && inventoryUI.activeInHierarchy)
        {
            OpenInventory();
        }
        if (!GameManager.gameIsPaused || inventoryUI.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("Got key down");
                if (GameManager.gameIsPaused)
                {
                    ResumeGame();
                }
                else
                {
                    OpenInventory();
                }
            }
        }
        // If the condition above is not fulfilled,
        // This means that the game is paused from another script
        // Hence we will not allow the Inventory UI to show up

    }

    public void ResumeGame()
    {
        inventoryUI.SetActive(false);
        Time.timeScale = 1f;
        GameManager.gameIsPaused = false;
    }

    void OpenInventory()
    {
        ResetInventoryPanel();
        // Enable the pause menu gameobject to make it visible
        inventoryUI.SetActive(true);
        // Freeze time
        Time.timeScale = 0f;
        GameManager.gameIsPaused = true;
    }

    void ResetInventoryPanel()
    {
        itemImage.sprite = null;
        itemNameUI.text = "";
        itemAmountUI.text = "";
    }
}
