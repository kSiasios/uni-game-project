using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPrompt : MonoBehaviour
{
    [Tooltip("The message that will be displayed. To display the corresponding button every time, write {key}.")]
    [SerializeField] string promptText;
    [Tooltip("The TextMeshPro component that contains the prompt.")]
    [SerializeField] TextMeshPro textContainer;
    [Tooltip("The default interaction key")]
    [SerializeField] KeyCode defaultKey = KeyCode.E;

    private void Awake()
    {
        if (textContainer == null)
        {
            textContainer = GetComponentInChildren<TextMeshPro>();
        }
        SetText();
        //DisableInteractPrompt();
        SetChildrenEnabled(false);
    }

    private void FixedUpdate()
    {
        if (transform.lossyScale.x < 0)
        {
            // ensure that the text is always facing the right way
            Vector3 newTransform = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            transform.localScale = newTransform;
        }

        if (transform.lossyScale.y < 0)
        {
            // ensure that the text is always facing the right way
            Vector3 newTransform = new Vector3(transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
            transform.localScale = newTransform;
        }
    }

    public static void EnableInteractPrompt()
    {
        InteractionPrompt prompt = FindObjectOfType<InteractionPrompt>();
        //prompt.transform.childCount;

        //for (int i = 0; i < prompt.transform.childCount - 1; i++)
        //{
        //    prompt.transform.GetChild(i).gameObject.SetActive(true);
        //}

        //foreach (Transform child in prompt.transform)
        //{
        //    //Debug.Log("Disabling " + child);
        //    child.gameObject.SetActive(true);
        //}
        prompt.SetChildrenEnabled(true);
        prompt.SetText();
    }

    public static void DisableInteractPrompt()
    {
        InteractionPrompt prompt = FindObjectOfType<InteractionPrompt>();

        //for (int i = 0; i < prompt.transform.childCount - 1; i++)
        //{
        //    Debug.Log("Disabling " + prompt.transform.GetChild(i));
        //    prompt.transform.GetChild(i).gameObject.SetActive(false);
        //}

        //foreach (Transform child in prompt.transform)
        //{
        //    Debug.Log("Disabling " + child);
        //    child.gameObject.SetActive(false);
        //}
        prompt.SetChildrenEnabled(false);
        //prompt.textContainer.enabled = false;
    }

    public void SetChildrenEnabled(bool enabled)
    {
        foreach (Transform child in transform)
        {
            //Debug.Log("Setting enabled '"+enabled+"' for " + child);
            child.gameObject.SetActive(enabled);
        }
    }

    public static void SetPromptKey(string key)
    {
        //Debug.Log("Setting key: " + key);
        InteractionPrompt prompt = FindObjectOfType<InteractionPrompt>();
        prompt.defaultKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), key);
    }

    public static void SetPromptText(string text)
    {
        InteractionPrompt prompt = FindObjectOfType<InteractionPrompt>();
        prompt.promptText = text;
    }

    public void SetText()
    {
        string newText = promptText;
        textContainer.text = newText.Replace("{key}", defaultKey.ToString());
    }
}
