using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] Collider2D colliderOnEnter;
    [SerializeField] Collider2D colliderOnExit;

    [SerializeField] Transform[] customLoadUnloadList;

    private bool load = false;

    public bool enabledChildren = false;
    private void Awake()
    {
        if (colliderOnEnter == null)
        {
            colliderOnEnter = FindObjectOfType<PlayerController>().GetComponent<CapsuleCollider2D>();
        }

        if (colliderOnExit == null)
        {
            colliderOnExit = FindObjectOfType<PlayerController>().GetComponent<CapsuleCollider2D>();
        }

        foreach (Transform child in transform)
        {
            //Debug.Log(child.name);
            child.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (gameObject.name == "LevelChuck - Fog")
        //{
        //    Debug.Log($"ENTER: Collision with {collision}. Checking with {colliderOnEnter}");
        //}
        if (colliderOnEnter != null)
        {
            if (colliderOnEnter == collision)
            {
                LoadObjects(collision);
                load = true;
            }
        }
        else
        {
            LoadObjects(collision);
                load = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (gameObject.name == "LevelChuck - Fog")
        //{
        //    Debug.Log($"EXIT: Collision with {collision}. Checking with {colliderOnExit}");
        //}
        if (colliderOnExit != null)
        {
            if (colliderOnExit == collision)
            {
                UnloadObjects(collision);
                load = false;
            }
        }
        else
        {
            UnloadObjects(collision);
                load = false;
        }
    }

    private void LoadObjects(Collider2D collision)
    {
        //Debug.Log($"Colliding with {collision.gameObject}");
        PlayerController collisionIsPlayer = collision.GetComponent<PlayerController>();

        if (collisionIsPlayer == null)
        {
            // check parent
            GameObject collisionParent = collision.transform.parent == null ? null : collision.transform.parent.gameObject;
            if (collisionParent != null)
            {
                collisionIsPlayer = collisionParent.GetComponent<PlayerController>();
            }
        }

        if (collisionIsPlayer != null)
        {
            LoadLoop();
        }
    }

    public void LoadLoop()
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

        enabledChildren = true;
    }

    private void OnTransformChildrenChanged()
    {
        if (load)
        {
            LoadLoop();
        } else
        {
            UnloadLoop();
        }
    }

    private void UnloadObjects(Collider2D collision)
    {
        //GameObject collisionParent = collision.transform.parent;
        //Debug.Log($"Colliding with {collision.gameObject}");
        PlayerController collisionIsPlayer = collision.GetComponent<PlayerController>();

        if (collisionIsPlayer == null)
        {
            // check parent
            GameObject collisionParent = collision.transform.parent == null ? null : collision.transform.parent.gameObject;
            if (collisionParent != null)
            {
                collisionIsPlayer = collisionParent.GetComponent<PlayerController>();
            }
        }

        if (collisionIsPlayer != null)
        {
            UnloadLoop();
        }
    }

    private void UnloadLoop()
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
        enabledChildren = false;
    }
}
