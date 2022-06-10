using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;

public class SaveSystem
{
    public static void SaveGame(SaveData data, int slot = 1)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //string path = Application.persistentDataPath + "/Saves/Save" + slot + "/";
        string path = $"{Application.persistentDataPath}/Saves/Save{slot}/";


        Directory.CreateDirectory(path);

        Debug.Log(path);

        // First, save the global data that will be used all over the game (player's health, ammo, inventory, last level)
        using(FileStream stream = new FileStream($"{path}global.fork", FileMode.Create))
        {
            formatter.Serialize(stream, data.levelID);
        }

        Directory.CreateDirectory($"{path}Level{data.levelID}/");
        using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Collectables.fork", FileMode.Create))
        {
            formatter.Serialize(stream, data.collectables);
        }

        using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Enemies.fork", FileMode.Create))
        {
            formatter.Serialize(stream, data.enemies);
        }
        
        //using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Unlockables.fork", FileMode.Create))
        //{
        //    formatter.Serialize(stream, data);
        //}

        using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Inventory.fork", FileMode.Create))
        {
            formatter.Serialize(stream, data.inventory);
        }

        using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Player.fork", FileMode.Create))
        {
            formatter.Serialize(stream, data.player);
        }
    }

    public static bool LoadScene(SaveData data, bool check)
    {
        if (check)
        {
            SceneManager.LoadSceneAsync(data.levelID);
            //SceneManager.sceneLoaded += GameManager.OnLevelLoaded;
            return true;
        }
        return false;
    }

    public static SaveData LoadGame(int slot = 1)
    {
        string path = $"{Application.persistentDataPath}/Saves/Save{slot}/";
        if (File.Exists($"{path}global.fork"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            SaveData data = new SaveData();

            using (FileStream stream = new FileStream($"{path}global.fork", FileMode.Open))
            {
                data.levelID = (int)formatter.Deserialize(stream);
            }

            using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Inventory.fork", FileMode.Open))
            {
                data.inventory = formatter.Deserialize(stream) as InventoryData[];
            }

            using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Player.fork", FileMode.Open))
            {
                data.player = formatter.Deserialize(stream) as SerializablePlayer;
            }

            using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Enemies.fork", FileMode.Open))
            {
                data.enemies = formatter.Deserialize(stream) as SerializableEnemy[];
            }

            using (FileStream stream = new FileStream($"{path}Level{data.levelID}/Collectables.fork", FileMode.Open))
            {
                data.collectables = formatter.Deserialize(stream) as SerializableCollectable[];
            }
            //FileStream stream = new FileStream(path, FileMode.Open);

            //SaveData data = formatter.Deserialize(stream) as SaveData;
            //stream.Close();

            return data;
        } else
        {
            Debug.LogError($"Save file not found in {path}");
            return null;
        }
    }
}

[System.Serializable]
public struct SerializableVector3
{
    /// <summary>
    /// x component
    /// </summary>
    public float x;

    /// <summary>
    /// y component
    /// </summary>
    public float y;

    /// <summary>
    /// z component
    /// </summary>
    public float z;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    /// <summary>
    /// Returns a string representation of the object
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return String.Format("[{0}, {1}, {2}]", x, y, z);
    }

    /// <summary>
    /// Automatic conversion from SerializableVector3 to Vector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator Vector3(SerializableVector3 rValue)
    {
        return new Vector3(rValue.x, rValue.y, rValue.z);
    }

    /// <summary>
    /// Automatic conversion from Vector3 to SerializableVector3
    /// </summary>
    /// <param name="rValue"></param>
    /// <returns></returns>
    public static implicit operator SerializableVector3(Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }
}