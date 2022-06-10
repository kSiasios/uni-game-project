using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogSystem : MonoBehaviour
{
    [SerializeField][Range(0f, 0.5f)] private float printDelay;
    [SerializeField] private int index = -1;
    
    [SerializeField] private GameObject dialogBox;
    [SerializeField] private TextMeshProUGUI dialogText;
    [SerializeField] private Image dialogImage;
    [SerializeField] private AudioSource audioSource;
    [Tooltip("Audi clip played when a letter is written")]
    [SerializeField] private AudioClip audioClip;

    private DialogSpeaker currentSpeaker;

    Coroutine customPrintCoroutine;

    private void Awake()
    {
        dialogBox = transform.Find("DialogBox").gameObject;
        dialogText = dialogBox.transform.Find("Dialog").GetComponent<TextMeshProUGUI>();
        dialogImage = dialogBox.transform.Find("SpeakerImage").GetComponent<Image>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        index = -1;
    }
    public void NextDialog(string[] dialogLines)
    {
        if (customPrintCoroutine != null)
        {
            //Debug.Log("Custom Coroutine NOT null");
            StopCoroutine(customPrintCoroutine);
        }

        if (dialogLines.Length - index <= 1)
        {
            DisableSystem();
            return;
        }
        
        index++;
        //Debug.Log(dialogLines[index]);
        customPrintCoroutine = StartCoroutine(SlowWrite(dialogLines[index]));
        //Debug.Log($"Called Next Dialog! Index: {index}");
    }

    private IEnumerator SlowWrite(string printMsg)
    {
        for (int i = 0; i < printMsg.Length; i++)
        {
            //for (int j = 0; j <= i; j++)
            //{
            //    Debug.Log(printMsg[j]);
            //}

            // Create new dialog item object
            DialogItem dialogItem = new DialogItem(printMsg.Substring(0, i + 1), currentSpeaker.Image);

            //Debug.Log(dialogItem);

            // Populate dialog system
            WriteDialog(dialogItem);

            // Play audio clip
            audioSource.PlayOneShot(audioClip);

            //Debug.Log(printMsg.Substring(0, i + 1));
            yield return new WaitForSeconds(printDelay);
        }
    }

    private void WriteDialog(DialogItem item)
    {
        //Debug.Log(item.DialogLine);

        dialogImage.sprite = item.SpeakerImage;
        dialogText.text = item.DialogLine;
    }

    public void StopDialog()
    {
        if (customPrintCoroutine != null)
        {
            StopCoroutine(customPrintCoroutine);
        }
        index = -1;
    }

    public void initializeDialogSystem(DialogSpeaker speaker)
    {
        //index = -1;
        currentSpeaker = new DialogSpeaker(speaker);
    }

    public void DisableSystem()
    {
        Debug.Log("Called DisableSystem");
        StopDialog();
        gameObject.SetActive(false);
    }
}

public class DialogSpeaker
{
    string name;
    Sprite image;

    public DialogSpeaker(DialogSpeaker speaker)
    {
        name = speaker.name;
        image = speaker.image;
    }

    public DialogSpeaker(string name, Sprite image = null)
    {
        this.name = name;
        this.image = image;
    }

    public Sprite Image { get => image; set => image = value; }
    public string Name { get => name; set => name = value; }
}