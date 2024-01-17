using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager Instance { get; private set; }

    [Header("Debuging")]
    [SerializeField] private bool initializeDataIfNull = false;

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private ItemDatabaseObject database;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public bool IsNewGame { get; private set; } = false;
    public bool DeleteSaveFile = false; // temp
    public bool InitializeDataIfNull => initializeDataIfNull;
    public FileDataHandler FileDataHandler => dataHandler; // temp

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Loader.Scene.GamePlay.ToString())
        {
            database.UpdateID();
            StartCoroutine(LoadGameAfterSceneLoaded());
        }
        else
        {
            // Debug.Log("OnSceneLoaded CALLED");
            dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
    }

    public void OnSceneUnloaded(Scene scene)
    {
        // Debug.Log($"OnSceneUnloaded - SAVED in: {scene.name}");
        if (scene.name == Loader.Scene.GamePlay.ToString()) return;
        SaveGame();
    }

    private IEnumerator LoadGameAfterSceneLoaded()
    {
        yield return new WaitForEndOfFrame();
        // Debug.Log("OnSceneLoaded CALLED");
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
        IsNewGame = false;
    }

    public void NewGame()
    {
        // Debug.Log("INITIALIZED NEWGAME");
        IsNewGame = true;
        DeleteSaveFile = false;
        gameData = new GameData();
    }
    public void LoadGame()
    {
        database.UpdateID();
        // load any saved data from a file using the data handler
        gameData = dataHandler.Load();

        // if no data can be loaded, create a new game
        if (gameData == null && initializeDataIfNull)
        {
            NewGame();
        }

        if (gameData == null || IsNewGame)
        {
            // Debug.LogWarning("No Data was found. a new game needs to be started before the game can be loaded");
            return;
        }


        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            if (dataPersistenceObject != null)
                dataPersistenceObject.LoadData(gameData);
            else
                Debug.LogWarning($"{dataPersistenceObject} is null or already destroyed while attempting to LOAD the game");
        }
        // Debug.Log("LOADED");
    }
    public void SaveGame()
    {
        // if we dont have any data to save, log a warning here
        if (gameData == null)
        {
            // Debug.LogWarning("No Data was found. a new game needs to be started before the game can be saved");
            return;
        }


        foreach (IDataPersistence dataPersistenceObject in dataPersistenceObjects)
        {
            if (dataPersistenceObject != null)
                dataPersistenceObject.SaveData(ref gameData);
            else
                Debug.LogWarning($"{dataPersistenceObject} is null or already destroyed while attempting to SAVE the game");
        }

        // not saving the game after deleting the save file
        if (DeleteSaveFile) return;

        // save that data to a file usin gthe data handler
        dataHandler.Save(gameData);
        // Debug.Log("Save");
    }

    private void OnApplicationQuit()
    {
        if (TimeController.Instance != null && !TimeController.Instance.IsNightTime())
            SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }
}
