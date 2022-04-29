using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractableCharacter : InteractableEntity
{
    [SerializeField] private Sprite speakerImage;
    [SerializeField] private string[] dialogLines;
    [SerializeField] private int index = -1;
    //[SerializeField] [Range(0f, 0.5f)] private float printDelay;

    [SerializeField] private GameObject dialogSystem;
    [SerializeField] private GameObject dialogBox;
    //[SerializeField] private TextMeshProUGUI dialogText;
    //[SerializeField] private Image dialogImage;
    [SerializeField] private Button nextDialogButton;

    [SerializeField] private DialogSystem dialogSystemFunctions;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(0, 2);
        Physics.IgnoreLayerCollision(4, 9);

        if (dialogLines.Length > 0)
        {
            // we have dialog lines, so we will need reference to the dialog system

            //dialogSystem = FindObjectOfType<DialogSystem>();
            
            dialogSystem = FindObjectOfType<Canvas>().transform.Find("GameplayUI").transform.Find("DialogSystem").gameObject;
            dialogBox = dialogSystem.transform.Find("DialogBox").gameObject;
            //dialogText = dialogBox.transform.Find("Dialog").GetComponent<TextMeshProUGUI>();
            //dialogImage = dialogBox.transform.Find("SpeakerImage").GetComponent<Image>();
            nextDialogButton = dialogBox.transform.Find("NextDialogButton").GetComponent<Button>();
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && collidingWithPlayer)
        {
            // Enable dialog system
            //GameManager.canGetGameplayInput = false;
            GameManager.interacting = true;
            EnableDialogSystem();
            dialogSystemFunctions.NextDialog(dialogLines);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        // Disable the dialog system
        dialogSystemFunctions.StopDialog();
        DisableDialogSystem();

        //GameManager.canGetGameplayInput = true;
        GameManager.interacting = false;
    }

    

    private void EnableDialogSystem()
    {
        dialogSystem.gameObject.SetActive(true);

        dialogSystemFunctions = dialogSystem.GetComponent<DialogSystem>();

        DialogSpeaker thisSpeaker = new DialogSpeaker("");
        dialogSystemFunctions.initializeDialogSystem(thisSpeaker);
        nextDialogButton.onClick.AddListener(() =>
        {
            dialogSystemFunctions.NextDialog(dialogLines);
        });
    }

    private void DisableDialogSystem()
    {
        nextDialogButton.onClick.RemoveAllListeners();
        dialogSystem.gameObject.SetActive(false);
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