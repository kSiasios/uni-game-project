using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LootItem : MonoBehaviour
{
    [SerializeField] GameObject item;
    [SerializeField] float chanceToDrop;
    [SerializeField] int quantity;

    public LootItem(GameObject item, int quantity, float chanceToDrop)
    {
        Item = item;
        Quantity = quantity;
        ChanceToDrop = chanceToDrop;
    }

    public int Quantity { get => quantity; set => quantity = value; }
    public float ChanceToDrop { get => chanceToDrop; set => chanceToDrop = value; }
    public GameObject Item { get => item; set => item = value; }

    private void Awake()
    {
        if (item == null)
        {
            item = transform.gameObject;
        }
    }

    public void Drop(Transform transform)
    {
        GameObject drop = Instantiate(Item, transform);
        drop.transform.localPosition = 5 * Vector3.up;
        drop.transform.parent = null;
    }
}