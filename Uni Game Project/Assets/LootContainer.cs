using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainer : MonoBehaviour
{
    [SerializeField][Range(0f, 100f)] private float chance;

    [SerializeField] LootItem[] possibleDropItems;

    private void Awake()
    {
        float possibility = 0;
        for (int i = 0; i < possibleDropItems.Length; i++)
        {
            possibility += possibleDropItems[i].ChanceToDrop;
        }
        if (possibility > 100)
        {
            Debug.LogError("DROP ITEMS POSSIBILITY IS INVALID! CURRENT VALUE: " + possibility + ". ACCEPTABLE VALUES ARE BETWEEN 0 AND 100.");
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    public void DropLoot()
    {
        if (possibleDropItems.Length == 0)
        {
            Debug.LogAssertion("SORRY :( NO ITEMS TO DROP");
            return;
        }
        float chanceToDropAnything = Random.Range(0f, 100f);

        if (chanceToDropAnything >= 100 - chance)
        {
            float chanceToDropItem = Random.Range(0f, 100f);
            //LootItem itemToDrop = new LootItem(null, 0, 0);
            foreach (var item in possibleDropItems)
            {
                if (item.ChanceToDrop >= 100 - chanceToDropItem)
                {
                    //itemToDrop = item;
                    //itemToDrop.Item = item.Item;
                    //itemToDrop.Quantity = item.Quantity;
                    //itemToDrop.ChanceToDrop = item.ChanceToDrop;
                    Debug.Log("DROPPING " + item.ToString());
                    item.Drop(transform);
                    break;
                }
            }

            //Debug.Log("DROPPING " + itemToDrop.ToString());
        }
    }
}
