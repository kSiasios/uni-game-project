using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    // Level Specific Variables
    public int levelID;
    public string levelChunck;

    // Player Specific Variables
    public SerializablePlayer player;

    // Collectables Specific Variables
    public SerializableCollectable[] collectables;

    // Enemies Specific Variables
    public SerializableEnemy[] enemies;

    // Inventory Specific Variables
    public InventoryData[] inventory;

    public SaveData() { }

    public SaveData(int lvlID, SerializablePlayer pl, InventoryData[] inv)
    {
        levelID = lvlID;
        //levelChunck = lvlChnk;
        player = pl;
        inventory = inv;
    }

    public SaveData(int lvlID, SerializablePlayer pl, InventoryData[] inv, SerializableEnemy[] en, SerializableCollectable[] col)
    {
        levelID = lvlID;
        //levelChunck = lvlChnk;
        player = pl;
        inventory = inv;
        enemies = en;
        collectables = col;
    }

    //public override string ToString()
    //{
    //    return $"Player Position: x = {player.position[0]}, y = {player.position[1]}, z = {player.position[2]}";
    //}
}

//public class GenericSaveData
//{
//    // This class is created because many of
//    // the objects we want to save, share
//    // the same data types of valuable data

//    // Player       ->  playerHealth (int)
//    //                  playerAmmo (int)
//    //                  playerPosition (float[])
//    //================================================
//    // Enemies      ->  enemyHealth (int)
//    //                  enemyState (int)
//    //                  enemyPosition (float[])
//    //================================================
//    // Collectable  ->  collectableAmount (int)
//    //                  collectableState (int)
//    //                  collectablePosition (float[])
//    public float firstVal;
//    public int secondVal;
//    public Vector3 position;

//    public GenericSaveData(float val1, int val2, Vector3 pos)
//    {
//        firstVal = val1;
//        secondVal = val2;
//        position = pos;
//    }
//}

[System.Serializable]
public class InventoryData
{
    public string name;
    public string serial;
    public int amount;
    public bool isKey;
    public string iconPath;

    public InventoryData(string n, string s, int a, bool k, string i)
    {
        name = n;
        serial = s;
        amount = a;
        isKey = k;
        iconPath = i;
    }

    public override string ToString()
    {
        return $"Name: {name},\t Amount: {amount},\t Sprite: {iconPath}, Serial: {serial}";
    }
}
