using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Public bools")]
    [Tooltip("Is the game paused or not?")]
    public static bool gameIsPaused = false;
    [Tooltip("Should we take input from the player or not?")]
    public static bool canGetGameplayInput = true;

    [Header("References needed for Saving")]
    [Tooltip("Reference to PlayerController script")]
    [SerializeField] PlayerController player;
    [Tooltip("Reference to InventoryManager script")]
    [SerializeField] InventoryManager inventory;
    [Tooltip("Reference to all the Collectables in the Scene")]
    [SerializeField] List<Collectable> collectables;
    [Tooltip("Reference to all the Enemies in the Scene")]
    [SerializeField] List<EnemyBehaviour> enemies;
    [Tooltip("Reference to current Scene")]
    [SerializeField] Scene currentScene;

    [SerializeField] Button saveBtn;
    [SerializeField] Button loadBtn;
    [SerializeField] bool justLoaded = false;

    SaveData loadData;

    private static GameManager s_Instance = null;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitReferences(true);
    }

    private void Update()
    {
        if (justLoaded)
        {
            InitReferences(false);
            justLoaded = false;
        }
        if (gameIsPaused && canGetGameplayInput)
        {
            canGetGameplayInput = false;
        }
        else if (!gameIsPaused && !canGetGameplayInput)
        {
            canGetGameplayInput = true;
        }
    }

    private void InitReferences(bool check)
    {
        if (check)
        {
            if (player == null)
            {
                player = FindObjectOfType<PlayerController>();
            }

            if (inventory == null)
            {
                inventory = player.GetComponentInChildren<InventoryManager>();
            }

            if (currentScene == null)
            {
                currentScene = SceneManager.GetActiveScene();
            }

            if (collectables.Count == 0)
            {
                Collectable[] array = FindObjectsOfType<Collectable>();
                collectables.AddRange(array);
            }

            if (enemies.Count == 0)
            {
                EnemyBehaviour[] array = FindObjectsOfType<EnemyBehaviour>();
                enemies.AddRange(array);
            }

            if (saveBtn == null)
            {
                saveBtn = FindObjectOfType<Canvas>().transform.Find("PauseMenu").transform.Find("SaveButton").GetComponent<Button>();
                saveBtn.onClick.AddListener(SaveGame);
            }

            if (loadBtn == null)
            {
                loadBtn = FindObjectOfType<Canvas>().transform.Find("PauseMenu").transform.Find("LoadButton").GetComponent<Button>();
                loadBtn.onClick.AddListener(LoadGame);
            }
        }
        else
        {
            player = FindObjectOfType<PlayerController>();

            inventory = player.GetComponentInChildren<InventoryManager>();

            currentScene = SceneManager.GetActiveScene();

            collectables.Clear();
            Collectable[] array = FindObjectsOfType<Collectable>();
            collectables.AddRange(array);

            enemies.Clear();
            EnemyBehaviour[] array1 = FindObjectsOfType<EnemyBehaviour>();
            enemies.AddRange(array1);

            saveBtn = FindObjectOfType<Canvas>().transform.Find("PauseMenu").transform.Find("SaveButton").GetComponent<Button>();
            saveBtn.onClick.AddListener(SaveGame);

            loadBtn = FindObjectOfType<Canvas>().transform.Find("PauseMenu").transform.Find("LoadButton").GetComponent<Button>();
            loadBtn.onClick.AddListener(LoadGame);
        }
    }

    public void SaveGame()
    {
        Debug.Log("Saving...");
        // Save player
        SerializablePlayer playerData = new SerializablePlayer(player);
        //GenericSaveData playerData = player.Save();
        Debug.Log("Created player data!!!");
        // Save the inventory
        //List<InventoryData> inventoryData = inventory.Save();
        InventoryData[] inventoryData = inventory.Save();

        // Save all the collectables in the scene
        //List<GenericSaveData> collectablesData = new List<GenericSaveData>();
        SerializableCollectable[] collectablesData = new SerializableCollectable[collectables.Count];
        int j = 0;
        foreach (var collectable in collectables)
        {
            SerializableCollectable localData = new SerializableCollectable(collectable);
            collectablesData[j] = localData;
            j++;
            //collectablesData.Add(collectable.Save());
        }
        
        // Save all the enemies in the scene
        //List<GenericSaveData> enemiesData = new List<GenericSaveData>();
        SerializableEnemy[] enemyData = new SerializableEnemy[enemies.Count];
        int i = 0;
        foreach (var enemy in enemies)
        {
            SerializableEnemy localData = new SerializableEnemy(enemy);
            enemyData[i] = localData;
            i++;
            //enemiesData.Add(enemy.Save());
        }

        // Save Scene
        int level = SceneManager.GetActiveScene().buildIndex;

        SaveData data = new SaveData(level, playerData, inventoryData, enemyData, collectablesData);

        // Save data using SaveSystem
        SaveSystem.SaveGame(data);
    }

    public void LoadGame()
    {
        //loadData = SaveSystem.LoadGame();

        //SceneManager.LoadScene(loadData.levelID);
        //Time.timeScale = 1f;
        //justLoaded = true;
        if (!justLoaded)
        {
            SaveData data = SaveSystem.LoadGame();
            if (data.levelID != SceneManager.GetActiveScene().buildIndex)
            {
                bool loadedScene = SaveSystem.LoadScene(data, !justLoaded);
                if(loadedScene)
                {
                    SceneManager.sceneLoaded += OnLevelLoaded;
                    InitReferences(false);
                }
            }
            player.Load(data.player);

            inventory.Load(data.inventory);

            int i = 0;
            foreach (var enemy in enemies)
            {
                enemy.Load(data.enemies[i]);
                enemy.gameObject.SetActive(true);
                i++;
            }

            i = 0;
            foreach (var collectable in collectables)
            {
                collectable.Load(data.collectables[i]);
                collectable.gameObject.SetActive(true);
                i++;
            }
            justLoaded = true;
        }
    }

    //private void LoadData(SaveData loadData)
    //{
    //    GenericSaveData playerData = new GenericSaveData(
    //                loadData.playerHealth,
    //                loadData.playerAmmo,
    //                new Vector3(loadData.playerPosition[0], loadData.playerPosition[1], loadData.playerPosition[2]));

    //    player.Load(playerData);

    //    List<InventoryData> inventoryData = new List<InventoryData>();
    //    for (int j = 0; j < loadData.inventoryItemAmount.Length; j++)
    //    {
    //        InventoryData item = new InventoryData(
    //            loadData.inventoryItemName[j],
    //            loadData.inventoryItemSerial[j],
    //            loadData.inventoryItemAmount[j]);
    //        inventoryData.Add(item);
    //    }
    //    inventory.Load(inventoryData);

    //    int i = 0;
    //    foreach (var collectable in collectables)
    //    {
    //        GenericSaveData collectableData = new GenericSaveData(
    //            loadData.collectablesAmount[i],
    //            loadData.collectablesState[i],
    //            new Vector3(loadData.collectablesPosition[i][0], loadData.collectablesPosition[i][1], loadData.collectablesPosition[i][2]));
    //        collectable.Load(collectableData);
    //        i++;
    //    }

    //    i = 0;
    //    foreach (var enemy in enemies)
    //    {
    //        GenericSaveData enemyData = new GenericSaveData(
    //            loadData.enemiesHealth[i],
    //            loadData.enemiesState[i],
    //            new Vector3(loadData.enemiesPosition[i][0], loadData.enemiesPosition[i][1], loadData.enemiesPosition[i][2]));

    //        enemy.Load(enemyData);
    //        i++;
    //    }
    //}

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level loaded");
        Debug.Log(scene.name + " " + scene.buildIndex);
        SceneManager.sceneLoaded -= OnLevelLoaded;

        InitReferences(false);
        LoadGame();
    }
}
