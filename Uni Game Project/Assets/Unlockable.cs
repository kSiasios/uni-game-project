using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unlockable : MonoBehaviour
{
    [SerializeField] string serial = "0000";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Trigger");
        if(Input.GetKey(KeyCode.E))
        {
            if (collision.gameObject.name == "Player")
            {
                InventoryManager iManager = collision.gameObject.GetComponentInChildren<InventoryManager>();

                foreach (var item in iManager.inventory)
                {
                    if (item.Serial != null && item.Serial == serial)
                    {
                        Debug.Log("Opening Unlockable...");
                        transform.gameObject.SetActive(false);
                        break;
                    }
                }

                if (transform.gameObject.activeInHierarchy)
                {
                    Debug.Log("You don't have a key!");
                }
            }
            
        }
    }
}
