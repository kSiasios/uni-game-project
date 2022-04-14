using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCharacter : InteractableEntity
{
    [SerializeField] private string[] dialogLines;
    [SerializeField] private int index = -1;
    [SerializeField] [Range(0f, 0.5f)] private float printDelay;

    Coroutine customPrintCoroutine;

    private void Awake()
    {
        Physics.IgnoreLayerCollision(0, 2);
        Physics.IgnoreLayerCollision(4, 9);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.E) && collidingWithPlayer)
        {
            // Activate dialog
            NextDialog();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        if (customPrintCoroutine != null)
        {
            StopCoroutine(customPrintCoroutine);
        }
        index = -1;
    }

    private void NextDialog()
    {
        if (dialogLines.Length - index > 1)
        {
            index++;
        }
        else
        {
            index = 0;
        }

        if (customPrintCoroutine != null)
        {
            StopCoroutine(customPrintCoroutine);
        }

        //Debug.Log(dialogLines[index]);
        customPrintCoroutine = StartCoroutine(CustomPrint(dialogLines[index]));
    }

    private IEnumerator CustomPrint(string printMsg)
    {
        for (int i = 0; i < printMsg.Length; i++)
        {
            //for (int j = 0; j <= i; j++)
            //{
            //    Debug.Log(printMsg[j]);
            //}
            Debug.Log(printMsg.Substring(0, i + 1));
            yield return new WaitForSeconds(printDelay);
        }
    }
}
