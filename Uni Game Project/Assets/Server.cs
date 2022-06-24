using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Server : InteractableEntity
{
    GameManager gm;
    [SerializeField] GameObject savePrompt;
    [SerializeField] Button confirmButton;
    [SerializeField] Button cancelButton;
    Animator anim;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();

        actionOnInteraction = SaveGame;

        if (savePrompt == null)
        {
            savePrompt = GameManager.FindInActiveObjectByName("SavePrompt");
        }

        if (confirmButton == null)
        {
            if (savePrompt != null)
            {
                confirmButton = savePrompt.transform.Find("ButtonConfirm").GetComponent<Button>();
            }
        }
        confirmButton.onClick.AddListener(ConfirmSave);

        if (cancelButton == null)
        {
            if (savePrompt != null)
            {
                cancelButton = savePrompt.transform.Find("ButtonCancel").GetComponent<Button>();
            }
        }
        cancelButton.onClick.AddListener(CancelSave);

        anim = GetComponent<Animator>();
    }

    protected void SaveGame()
    {
        Debug.Log("Are you sure?");
        savePrompt.SetActive(true);
    }

    protected void ConfirmSave()
    {
        anim.SetTrigger("Save");
        Debug.Log("Confirm Save");
        gm.SaveGame();
        savePrompt.SetActive(false);
    }

    protected void CancelSave()
    {
        Debug.Log("Cancel Save");
        savePrompt.SetActive(false);
    }
}
