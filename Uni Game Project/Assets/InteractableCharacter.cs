using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractableCharacter : InteractableEntity
{
    [Header("- Interactable Character Variables")]
    [Space]
    [Tooltip("The image of the speaker that will appear on the dialog box.")]
    [SerializeField] private Sprite speakerImage;
    [Tooltip("A list of all the dialog lines of the character.")]
    [SerializeField] private string[] dialogLines;
    // The index that shows which dialog line the character is currently performing
    private int index = -1;

    [Tooltip("The gameobject that is responsible for the dialog system.")]
    [SerializeField] private GameObject dialogSystem;
    [Tooltip("The gameobject that contains the dialog box.")]
    [SerializeField] private GameObject dialogBox;
    [Tooltip("The button that is responsible for advancing the dialog.")]
    [SerializeField] private Button nextDialogButton;
    [Tooltip("The script that has the functionality of the dialog system.")]
    [SerializeField] private DialogSystem dialogSystemFunctions;

    protected void Awake()
    {
        Physics.IgnoreLayerCollision(0, 2);
        Physics.IgnoreLayerCollision(4, 9);

        if (dialogLines.Length > 0)
        {
            // we have dialog lines, so we will need reference to the dialog system

            //dialogSystem = FindObjectOfType<DialogSystem>();

            dialogSystem = GameObject.Find("GameplayUI").transform.Find("DialogSystem").gameObject;
            dialogBox = dialogSystem.transform.Find("DialogBox").gameObject;
            //dialogText = dialogBox.transform.Find("Dialog").GetComponent<TextMeshProUGUI>();
            //dialogImage = dialogBox.transform.Find("SpeakerImage").GetComponent<Image>();
            nextDialogButton = dialogBox.transform.Find("NextDialogButton").GetComponent<Button>();
        }

        actionOnInteraction = InteractableCharacterAction;
    }

    //protected void Update()
    //{
    //    if (Input.GetKeyDown(interactionKey) && collidingWithPlayer)
    //    {
    //        // Enable dialog system
    //        //GameManager.canGetGameplayInput = false;
    //        actionOnInteraction();
    //    }
    //}

    protected void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        // Disable the dialog system
        if (dialogSystem != null)
        {
            dialogSystemFunctions.StopDialog();
            DisableDialogSystem();
        }
        //GameManager.canGetGameplayInput = true;
        GameManager.interacting = false;
    }



    private void EnableDialogSystem()
    {
        dialogSystem.gameObject.SetActive(true);

        dialogSystemFunctions = dialogSystem.GetComponent<DialogSystem>();

        DialogSpeaker thisSpeaker = new DialogSpeaker("");
        dialogSystemFunctions.initializeDialogSystem(thisSpeaker);
        nextDialogButton.onClick.RemoveAllListeners();
        nextDialogButton.onClick.AddListener(() =>
        {
            //dialogSystemFunctions.NextDialog(dialogLines);
            //if (dialogSystem != null)
            //{
            //    EnableDialogSystem();
            //    dialogSystemFunctions.NextDialog(dialogLines);
            //}
            InteractableCharacterAction();
        });
    }

    private void DisableDialogSystem()
    {
        nextDialogButton.onClick.RemoveAllListeners();
        dialogSystem.gameObject.SetActive(false);
    }

    private void InteractableCharacterAction()
    {
        Debug.Log("Henlo from InteractableCharacter!");
        GameManager.interacting = true;
        if (dialogSystem != null)
        {
            EnableDialogSystem();
            dialogSystemFunctions.NextDialog(dialogLines);
        }
    }
}

public class DialogItem
{
    private string dialogLine;
    private Sprite speakerImage;

    public DialogItem(string line, Sprite image = null)
    {
        dialogLine = line;
        speakerImage = image;
    }

    public string DialogLine { get => dialogLine; set => dialogLine = value; }
    public Sprite SpeakerImage { get => speakerImage; set => speakerImage = value; }
}