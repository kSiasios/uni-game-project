using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCharacter : MonoBehaviour
{
    [SerializeField] private string[] dialogLines;
    [SerializeField] private int index = -1;
    [SerializeField] [Range(0f, 0.5f)] private float printDelay;

    [Tooltip("Is the character colliding with the player?")]
    [SerializeField] private bool collidingWithPlayer = false;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.transform.parent = this.gameObject.transform;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collidingWithPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.transform.parent = null;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collidingWithPlayer = false;
        }
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
