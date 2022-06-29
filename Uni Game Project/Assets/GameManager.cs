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

    [Header("UI needed for the death screen")]
    [Tooltip("Loading screen canvas")]
    [SerializeField] GameObject _deathScreenCanvas;
    //float _target;

    [Header("Audio needed for the loading screen")]
    [Tooltip("Loading screen audio clip")]
    [SerializeField] AudioClip _loadingScreenAudioClip;
    [Tooltip("Loading screen audio fade time")]
    [SerializeField] float _loadingFadeTime;
    AudioSourceController audioSourceController;

    SaveData loadData;

    private static GameManager s_Instance = null;

    //[SerializeField] 
    public static int gameState = 0;

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

        player = FindObjectOfType<PlayerController>(true);

        inventory = FindObjectOfType<InventoryManager>(true);

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
        InitReferences(true);
        PlayerController player = FindObjectOfType<PlayerController>(true);
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
        inventory = FindObjectOfType<InventoryManager>(true);
        InventoryData[] inventoryData = inventory.Save();
        //InventoryData[] inventoryData = FindObjectOfType<InventoryManager>(true).Save();

        Debug.Log("Saving Inventory");
        Debug.Log("======== INVENTORY LIST ========");
        inventory.PrintInventory();
        Debug.Log("==== END OF INVENTORY LIST =====");


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

        SerializableGlobalData globalData = new SerializableGlobalData(level, gameState);

        //SaveData data = new SaveData(level, $"{lvlChnck}", playerData, inventoryData, enemyData, collectablesData);
        SaveData data = new SaveData(globalData, playerData, inventoryData, enemyData, collectablesData);

        // Save data using SaveSystem
        SaveSystem.SaveGame(data);
    }

    public async void LoadGame()
    {
        if (!justLoaded)
        {
            SaveData data = SaveSystem.LoadGame();
            if (data != null)
            {
                if (data.globalData.levelID != SceneManager.GetActiveScene().buildIndex)
                {
                    LoadScene(data.globalData.levelID);
                    await Task.Delay(1000);
                }
                InitReferences(false);

                DataLoad(data);
            }
        }
    }

    private void DataLoad(SaveData data)
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        //Debug.Log($"LOADING --> Player: {player}, Player Data: ({data.player.positionX}, {data.player.positionY}, {data.player.positionZ})");

        player.Load(data.player);

        inventory.Load(data.inventory);
        Debug.Log("Loading Inventory");
        Debug.Log("======== INVENTORY LIST ========");
        inventory.PrintInventory();
        Debug.Log("==== END OF INVENTORY LIST =====");

        gameState = data.globalData.gameState;
        //Debug.Log($"Loaded GameState:: {gameState}");
        //Debug.Log($"Level  Loaded:: {data.globalData.levelID}");
        if (data.globalData.levelID == 2)
        {
            GameStateManager();
        }

        // disable death screen
        _deathScreenCanvas.SetActive(false);

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
        //Debug.Log("Spawning Enemies");
        enemies.Clear();
        foreach (var enemyData in data.enemies)
        {
            //Debug.Log($"EnemyType = {enemyData.type}");
            if (enemyData.type == 0)
            {

                // walker
                EnemyBehaviour prefabValues = enemyWalkerGO.GetComponent<EnemyBehaviour>();
                if (prefabValues == null)
                {
                    continue;
                }
                // spawn object
                //Debug.Log($"Spawning Walker with parent: {enemyData.parentName}");
                GameObject spawnedWalker = Instantiate(
                    enemyWalkerGO,
                    new Vector3(enemyData.positionX, enemyData.positionY, enemyData.positionZ),
                    Quaternion.identity,
                    enemyData.parentName == "" ? null :
                    GameManager.FindInActiveObjectByName(enemyData.parentName).transform);
                EnemyBehaviour spawnedEnemyValues = spawnedWalker.GetComponent<EnemyBehaviour>();
                spawnedEnemyValues.SetState((EnemyBehaviour.EnemyState)enemyData.state);
                //Debug.Log($"Spawning Walker ({spawnedWalker}) with parent: {enemyData.parentName}");

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
                    Quaternion.identity, enemyData.parentName == "" ? null :
                    GameManager.FindInActiveObjectByName(enemyData.parentName).transform);
                EnemyBehaviour spawnedEnemyValues = spawnedFlyer.GetComponent<EnemyBehaviour>();
                spawnedEnemyValues.SetState((EnemyBehaviour.EnemyState)enemyData.state);
                //Debug.Log($"Spawning Flyer ({spawnedFlyer})");
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

        //Debug.Log("Spawning Collectable");
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
                    Debug.Log($"Trying to spawn {prefabValues.name} at position ({collectableData.positionX}, {collectableData.positionY}, {collectableData.positionZ}) under parent => {collectableData.parentName}");
                    // spawn object
                    GameObject spawnedCollectable = Instantiate(
                        collectablePrefab,
                        new Vector3(collectableData.positionX, collectableData.positionY, collectableData.positionZ),
                        Quaternion.identity,
                        collectableData.parentName == "" ? null :
                        GameManager.FindInActiveObjectByName(collectableData.parentName).transform);
                    Collectable spawnedCollectableValues = spawnedCollectable.GetComponent<Collectable>();
                    spawnedCollectableValues.SetState((Collectable.CollectableState)collectableData.state);
                }
            }
        }

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

    public async void LoadScene(int sceneIndex)
    {
        LoadingAudioIn();
        _target = 0f;
        _loadingSlider.value = 0f;


        var scene = SceneManager.LoadSceneAsync(sceneIndex);
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
        SaveGame();

    }

    public void NewGame()
    {
        LoadScene(1);
    }

    private void GameStateManager()
    {
        Debug.Log($"Game State :: {gameState}");
        GameObject greenDoor = FindInActiveObjectByName("Door Green");
        GameObject redDoor = FindInActiveObjectByName("Door Red");
        GameObject blueDoor = FindInActiveObjectByName("Door Blue");
        GameObject generator = FindObjectOfType<Genarator>(true).gameObject;
        GameObject endGameTrigger = FindObjectOfType<EndGameTrigger>(true).gameObject;

        //Debug.Log("OBJECT FOUND");
        //Debug.Log(greenDoor);
        //Debug.Log(redDoor);
        //Debug.Log(blueDoor);
        //Debug.Log(generator);
        //Debug.Log(endGameTrigger);
        switch (gameState)
        {
            case 1:
                Debug.Log("CASE 1");
                //green door
                greenDoor.SetActive(true);
                greenDoor.GetComponent<Unlockable>().SetUnlocked(true);

                redDoor.SetActive(true);
                redDoor.GetComponent<Unlockable>().SetUnlocked(false);
                blueDoor.SetActive(true);
                blueDoor.GetComponent<Unlockable>().SetUnlocked(false);
                generator.GetComponent<Genarator>().SetState(Genarator.GeneratorState.broken);
                endGameTrigger.SetActive(false);
                break;
            case 2:
                Debug.Log("CASE 2");
                //red door
                greenDoor.SetActive(true);
                greenDoor.GetComponent<Unlockable>().SetUnlocked(true);
                redDoor.SetActive(true);
                redDoor.GetComponent<Unlockable>().SetUnlocked(true);

                blueDoor.SetActive(true);
                blueDoor.GetComponent<Unlockable>().SetUnlocked(false);
                generator.GetComponent<Genarator>().SetState(Genarator.GeneratorState.broken);
                endGameTrigger.SetActive(false);
                break;
            case 3:
                Debug.Log("CASE 3");
                //blue door
                greenDoor.SetActive(true);
                greenDoor.GetComponent<Unlockable>().SetUnlocked(true);
                redDoor.SetActive(true);
                redDoor.GetComponent<Unlockable>().SetUnlocked(true);
                blueDoor.SetActive(true);
                blueDoor.GetComponent<Unlockable>().SetUnlocked(true);

                generator.GetComponent<Genarator>().SetState(Genarator.GeneratorState.broken);
                endGameTrigger.SetActive(false);
                break;
            case 4:
                Debug.Log("CASE 4");
                //end game
                greenDoor.SetActive(true);
                greenDoor.GetComponent<Unlockable>().SetUnlocked(true);
                redDoor.SetActive(true);
                redDoor.GetComponent<Unlockable>().SetUnlocked(true);
                blueDoor.SetActive(true);
                blueDoor.GetComponent<Unlockable>().SetUnlocked(true);
                generator.GetComponent<Genarator>().SetState(Genarator.GeneratorState.enabled);
                endGameTrigger.SetActive(true);
                break;
            default:
                //new game
                Debug.Log("DEFAULT CASE");
                break;
        }
    }

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

    public void DeathSequence()
    {
        _deathScreenCanvas.SetActive(true);
    }

    public async void LoadGameNoCheck()
    {
        if (!justLoaded)
        {
            SaveData data = SaveSystem.LoadGame();
            LoadScene(data.globalData.levelID);
            await Task.Delay(1000);

            InitReferences(false);

            DataLoad(data);
        }
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}