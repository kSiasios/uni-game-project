using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    [Tooltip("Is the game paused or not?")]
    public static bool interacting = false;


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

    [Header("References needed for Loading")]
    [Tooltip("Reference to Player Prefab")]
    [SerializeField] GameObject playerGO;
    [Tooltip("Reference to Enemy Walker Prefab")]
    [SerializeField] GameObject enemyWalkerGO;
    [Tooltip("Reference to Enemy Flyer Prefab")]
    [SerializeField] GameObject enemyFlyerGO;
    [Tooltip("Reference to Collectables Pefabs")]
    [SerializeField] List<GameObject> collectablesGO;
    [Tooltip("Reference to Patrol Point Prefab")]
    [SerializeField] GameObject patrolPointGO;

    [SerializeField] Button loadBtn;
    [SerializeField] bool justLoaded = false;

    [Header("UI needed for the loading screen")]
    [Tooltip("Loading screen canvas")]
    [SerializeField] GameObject _loadingScreenCanvas;
    [Tooltip("Loading slider")]
    [SerializeField] Slider _loadingSlider;
    float _target;

    [Header("Audio needed for the loading screen")]
    [Tooltip("Loading screen audio clip")]
    [SerializeField] AudioClip _loadingScreenAudioClip;
    [Tooltip("Loading screen audio fade time")]
    [SerializeField] float _loadingFadeTime;
    AudioSourceController audioSourceController;

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
            //InitReferences(false);
            InitReferences(true);
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

        if (interacting && canGetGameplayInput)
        {
            canGetGameplayInput = false;
        }
        else if (!interacting && !canGetGameplayInput)
        {
            canGetGameplayInput = true;
        }

        _loadingSlider.value = Mathf.MoveTowards(_loadingSlider.value, _target, Time.deltaTime);
    }

    private void InitReferences(bool check)
    {
        audioSourceController = GetComponent<AudioSourceController>();
        //if (check)
        //{
        //    if (player == null)
        //    {
        //        player = FindObjectOfType<PlayerController>();
        //    }

        //    if (player == null)
        //    {
        //        return;
        //    }

        //    if (inventory == null)
        //    {
        //        inventory = player.GetComponentInChildren<InventoryManager>();
        //    }

        //    if (currentScene == null)
        //    {
        //        currentScene = SceneManager.GetActiveScene();
        //    }

        //    if (collectables.Count == 0)
        //    {
        //        Collectable[] array = FindObjectsOfType<Collectable>();
        //        collectables.AddRange(array);
        //    }

        //    if (enemies.Count == 0)
        //    {
        //        EnemyBehaviour[] array = FindObjectsOfType<EnemyBehaviour>();
        //        enemies.AddRange(array);
        //    }

        //    if (saveBtn == null)
        //    {
        //        saveBtn = GameObject.Find("PauseMenu").transform.Find("SaveButton").GetComponent<Button>();
        //        saveBtn.onClick.AddListener(SaveGame);
        //    }

        //    if (loadBtn == null)
        //    {
        //        loadBtn = GameObject.Find("PauseMenu").transform.Find("LoadButton").GetComponent<Button>();
        //        loadBtn.onClick.AddListener(LoadGame);
        //    }
        //}
        //else
        //{
        player = FindObjectOfType<PlayerController>();

        inventory = FindObjectOfType<InventoryManager>();

        currentScene = SceneManager.GetActiveScene();

        collectables.Clear();
        Collectable[] array = FindObjectsOfType<Collectable>(true);
        collectables.AddRange(array);

        enemies.Clear();
        EnemyBehaviour[] array1 = FindObjectsOfType<EnemyBehaviour>(true);
        enemies.AddRange(array1);

        GameObject pauseMenu = GameObject.Find("PauseMenu");
        if (pauseMenu != null)
        {
            saveBtn = pauseMenu.transform.Find("SaveButton").GetComponent<Button>();
            saveBtn.onClick.AddListener(SaveGame);

            loadBtn = pauseMenu.transform.Find("LoadButton").GetComponent<Button>();
            loadBtn.onClick.AddListener(LoadGame);

        }

        //}
    }

    public void SaveGame()
    {
        Debug.Log("Saving...");
        //InitReferences(true);
        PlayerController player = FindObjectOfType<PlayerController>();
        //Debug.Log($"Player Object: {player}");
        //Debug.Log($"Player Position: {player.GetPosition()}");

        // Save player
        //SerializablePlayer playerData = new SerializablePlayer(player);

        string tempParentName = "";

        if (player.transform.parent != null)
        {
            tempParentName = player.transform.parent.name;
        }

        SerializablePlayer playerData = new SerializablePlayer(
            player.GetSpeed(),
            player.GetHealth(),
            player.GetMaxHealth(),
            player.GetPosition(),
            player.GetAmmo(),
            tempParentName);
        //GenericSaveData playerData = player.Save();
        //Debug.Log("Created player data!!!");
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
            Debug.Log($"Saving Enemy ({enemy})");
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

    public async void LoadGame()
    {
        if (!justLoaded)
        {
            SaveData data = SaveSystem.LoadGame();
            if (data.levelID != SceneManager.GetActiveScene().buildIndex)
            {
                bool loadedScene = SaveSystem.LoadScene(data, !justLoaded);
                if (loadedScene)
                {
                    SceneManager.sceneLoaded += OnLevelLoaded;
                }
            }
            InitReferences(false);

            PlayerController player = FindObjectOfType<PlayerController>();

            //Debug.Log($"LOADING --> Player: {player}, Player Data: ({data.player.positionX}, {data.player.positionY}, {data.player.positionZ})");

            player.Load(data.player);

            inventory.Load(data.inventory);

            // delete all enemies from the level
            EnemyBehaviour[] enemiesArray = FindObjectsOfType(typeof(EnemyBehaviour), true) as EnemyBehaviour[];
            foreach (var item in enemiesArray)
            {
                Destroy(item.gameObject);
            }

            // delete all patrolPoints from the level
            PatrolPoint[] patrolPointsArray = FindObjectsOfType(typeof(PatrolPoint), true) as PatrolPoint[];
            foreach (var item in patrolPointsArray)
            {
                Destroy(item.gameObject);
            }
            Debug.Log("Spawning Enemies");
            enemies.Clear();
            foreach (var enemyData in data.enemies)
            {
                Debug.Log($"EnemyType = {enemyData.type}");
                if (enemyData.type == 0)
                {

                    // walker
                    EnemyBehaviour prefabValues = enemyWalkerGO.GetComponent<EnemyBehaviour>();
                    if (prefabValues == null)
                    {
                        continue;
                    }
                    // spawn object
                    GameObject spawnedWalker = Instantiate(
                        enemyWalkerGO,
                        new Vector3(enemyData.positionX, enemyData.positionY, enemyData.positionZ),
                        Quaternion.identity,
                        GameManager.FindInActiveObjectByName(enemyData.parentName).transform);
                    EnemyBehaviour spawnedEnemyValues = spawnedWalker.GetComponent<EnemyBehaviour>();
                    spawnedEnemyValues.SetState((EnemyBehaviour.EnemyState)enemyData.state);
                    Debug.Log($"Spawning Walker ({spawnedWalker})");

                    // spawn patrol points
                    GameObject point1 = Instantiate(patrolPointGO, new Vector3(enemyData.ppPos1X, enemyData.ppPos1Y, enemyData.ppPos1Z), Quaternion.identity, spawnedWalker.transform.parent);
                    GameObject point2 = Instantiate(patrolPointGO, new Vector3(enemyData.ppPos2X, enemyData.ppPos2Y, enemyData.ppPos2Z), Quaternion.identity, spawnedWalker.transform.parent);
                    // set them up with the walker
                    GameObject[] patrolPoints = new GameObject[2];
                    patrolPoints[0] = point1;
                    patrolPoints[1] = point2;
                    spawnedEnemyValues.SetCustomPatrolPoints(patrolPoints);
                }

                if (enemyData.type == 1)
                {
                    // flyer
                    EnemyBehaviour prefabValues = enemyFlyerGO.GetComponent<EnemyBehaviour>();
                    if (prefabValues == null)
                    {
                        continue;
                    }
                    // spawn object
                    GameObject spawnedFlyer = Instantiate(
                        enemyFlyerGO,
                        new Vector3(enemyData.positionX, enemyData.positionY, enemyData.positionZ),
                        Quaternion.identity,
                        GameManager.FindInActiveObjectByName(enemyData.parentName).transform);
                    EnemyBehaviour spawnedEnemyValues = spawnedFlyer.GetComponent<EnemyBehaviour>();
                    spawnedEnemyValues.SetState((EnemyBehaviour.EnemyState)enemyData.state);
                    Debug.Log($"Spawning Flyer ({spawnedFlyer})");
                }
            }

            //i = 0;
            //foreach (var collectable in collectables)
            //{
            //    collectable.Load(data.collectables[i]);
            //    //collectable.gameObject.SetActive(true);
            //    i++;
            //}
            //justLoaded = true;

            // delete all collectables from the level
            //Collectable[] collectablesArray = FindObjectsOfType<Collectable>();
            //Collectable[] collectablesArray = FindObjectsOfType(typeof(Collectable)) as Collectable[];
            Collectable[] collectablesArray = FindObjectsOfType(typeof(Collectable), true) as Collectable[];
            foreach (var item in collectablesArray)
            {
                //Debug.Log($"++++++ Destroying {item}");
                Destroy(item.gameObject);
            }

            Debug.Log("Spawning Collectable");
            collectables.Clear();
            foreach (var collectableData in data.collectables)
            {
                // spawn the collectable prefab that has the same amount as the saved collectable
                foreach (var collectablePrefab in collectablesGO)
                {
                    Collectable prefabValues = collectablePrefab.GetComponent<Collectable>();
                    if (prefabValues == null)
                    {
                        continue;
                    }
                    if (collectableData.amount == prefabValues.GetAmount())
                    {
                        // spawn object
                        GameObject spawnedCollectable = Instantiate(
                            collectablePrefab,
                            new Vector3(collectableData.positionX, collectableData.positionY, collectableData.positionZ),
                            Quaternion.identity,
                            GameManager.FindInActiveObjectByName(collectableData.parentName).transform);
                        Collectable spawnedCollectableValues = spawnedCollectable.GetComponent<Collectable>();
                        spawnedCollectableValues.SetState((Collectable.CollectableState)collectableData.state);
                    }
                }
            }
        }
    }

    public async void NewGame()
    {
        LoadingAudioIn();
        _target = 0f;
        _loadingSlider.value = 0f;


        var scene = SceneManager.LoadSceneAsync(1);
        scene.allowSceneActivation = false;

        _loadingScreenCanvas.SetActive(true);

        do
        {
            // Normalize progress to 100% instead of the default 90%
            _target = scene.progress * 1 / 0.9f;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;

        //SaveGame();
        await Task.Delay(5000);
        //justLoaded = true;
        LoadingAudioOut();
        _loadingScreenCanvas.SetActive(false);
        //InitReferences(false);

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

    private void LoadingAudioIn()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audioSource in audioSources)
        {
            if (audioSource.gameObject != gameObject)
            {
                audioSource.Stop();
            }
        }
        // fade sound for loading screen in
        audioSourceController.FadeIn(_loadingScreenAudioClip, _loadingFadeTime);
    }

    private void LoadingAudioOut()
    {
        // fade sound for loading screen out
        audioSourceController.FadeOut(_loadingFadeTime);
    }

    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("Level loaded");
        //Debug.Log($"{scene.name} {scene.buildIndex}");
        SceneManager.sceneLoaded -= OnLevelLoaded;

        InitReferences(false);


        LoadGame();
    }

    public static GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }
}
