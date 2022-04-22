using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Transform[] customLoadUnloadList;
    private void Awake()
    {
        foreach (Transform child in transform)
        {
            //Debug.Log(child.name);
            child.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            foreach (Transform child in transform)
            {
                //Debug.Log(child.name);
                child.gameObject.SetActive(true);
            }
            foreach (Transform customTransform in customLoadUnloadList)
            {
                //Debug.Log(child.name);
                customTransform.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>())
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            foreach (Transform customTransform in customLoadUnloadList)
            {
                //Debug.Log(child.name);
                customTransform.gameObject.SetActive(true);
            }
        }
    }
}
