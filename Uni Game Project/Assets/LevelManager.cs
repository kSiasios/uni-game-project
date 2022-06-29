using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Collider2D colliderOnEnter;
    [SerializeField] Collider2D colliderOnExit;

    [SerializeField] Transform[] customLoadUnloadList;

    private bool load = true;

    private void Awake()
    {
        if (colliderOnEnter == null)
        {
            colliderOnEnter = FindObjectOfType<PlayerController>(true).GetComponent<CapsuleCollider2D>();
        }

        if (colliderOnExit == null)
        {
            colliderOnExit = FindObjectOfType<PlayerController>(true).GetComponent<CapsuleCollider2D>();
        }

        foreach (Transform child in transform)
        {
            //Debug.Log(child.name);
            child.gameObject.SetActive(false);
        }
        //UnloadLoop();
        //load = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LoadObjects(collision);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        UnloadObjects(collision);
    }

    private void LoadObjects(Collider2D collision)
    {
        if (colliderOnExit != null)
        {
            if (colliderOnExit == collision)
            {
                LoadLoop();
            }
        }
    }

    public void LoadLoop()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        //foreach (Transform customTransform in customLoadUnloadList)
        //{
        //    customTransform.gameObject.SetActive(false);
        //}
        load = true;
    }

    private void OnTransformChildrenChanged()
    {
        if (load)
        {
            LoadLoop();
        }
        else
        {
            UnloadLoop();
        }
    }

    private void UnloadObjects(Collider2D collision)
    {
        if (colliderOnExit != null)
        {
            if (colliderOnExit == collision)
            {
                UnloadLoop();
            }
        }
    }

    private void UnloadLoop()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        load = false;
    }
}
