using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SlowWrite : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    [SerializeField] float printDelay;
    [SerializeField] TextMeshProUGUI textContainer;
    [SerializeField] string[] messages;

    //[SerializeField]GameManager gm;

    [SerializeField] int index = 0;

    Coroutine customPrintCoroutine;

    private void Awake()
    {
        //gm = FindObjectOfType<GameManager>();
        Write();
    }


    public void Write()
    {
        if (index == messages.Length)
        {
            Debug.Log($"Last line. Loading Scene: {SceneManager.GetActiveScene().buildIndex + 1}");
            // load next scene
            FindObjectOfType<GameManager>().LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        if (customPrintCoroutine != null)
        {
            //Debug.Log("Custom Coroutine NOT null");
            StopCoroutine(customPrintCoroutine);
        }

        //if (dialogLines.Length - index <= 1)
        //{
        //    DisableSystem();
        //    return;
        //}

        //Debug.Log(dialogLines[index]);
        customPrintCoroutine = StartCoroutine(SlowWriter(messages[index]));
        index++;
        //Debug.Log($"Called Next Dialog! Index: {index}");
    }

    private IEnumerator SlowWriter(string printMsg)
    {
        for (int i = 0; i < printMsg.Length; i++)
        {
            //for (int j = 0; j <= i; j++)
            //{
            //    Debug.Log(printMsg[j]);
            //}

            // Create new dialog item object
            //DialogItem dialogItem = new DialogItem(printMsg.Substring(0, i + 1), currentSpeaker.Image);

            //Debug.Log(dialogItem);

            // Populate dialog system
            PrintMessage(printMsg.Substring(0, i + 1));

            // Play audio clip
            audioSource.PlayOneShot(audioClip);

            //Debug.Log(printMsg.Substring(0, i + 1));
            yield return new WaitForSeconds(printDelay);
        }
    }

    private void PrintMessage(string str)
    {
        textContainer.text = str;
    }

    //public void NextMessage()
    //{
    //}
}
