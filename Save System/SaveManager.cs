using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Logic to be called by GameManager to save/load the game.
/// </summary>
public class SaveManager
{
    #region Member Variables

    // Save Data
    public static SaveData[] _saveData = { new SaveData(), new SaveData(), new SaveData() };

    // Collectibles
    static CollectiblesData collectiblesData = new CollectiblesData();
    public static int[] collectibles = { 0, 0, 0, 0, 0, 0 };
    private static bool isCollectiblesSaving;
    public static bool IsCollectiblesSaving => isCollectiblesSaving;

    // Tutorial Codex Entries
    static TutorialCodexData tutorialCodexData = new TutorialCodexData();
    public static List<string> UnlockedTutorialEntries = new List<string>();
    private static bool isCodexSaving;
    public static bool IsCodexSaving => isCodexSaving;

    // Save Slot
    public static int activeSaveSlot = 0;

    // String Builder
    static StringBuilder builder = new StringBuilder();

    [System.Serializable]
    public struct SaveData
    {
        // Player
        public PlayerStatusData playerStatus;
        public PlayerRespawnData playerRespawn;
        public PlayerCurrencyData playerCurrency;
        public PlayerInventoryData playerInventory;

        //public WeaponManagerData weaponManager;
        public AbilityWheelManagerData abilityWheelManager;
        public UnlockManagerData unlockManager;

        // Scene
        public SceneSaveData sceneSaveData;

        // Progression
        public ProgressionManagerData progressionManagerData;
    }

    [System.Serializable]
    public struct CollectiblesData
    {
        public int[] _collectibles;
    }

    [System.Serializable]
    public struct TutorialCodexData
    {
        public List<string> _tutorialCodexEntries;
    }

    #endregion

    #region File paths

    /// <summary>
    /// Returns save file name from active save file.
    /// </summary>
    /// <returns>Path of active save file.</returns>
    public static string SaveFileName()
    {
        builder.Clear();
        builder.Append(Application.persistentDataPath).Append("/savefile").Append(activeSaveSlot.ToString()).Append(".sav");
        return builder.ToString();
    }

    /// <summary>
    /// Returns save file path from a given file index.
    /// </summary>
    /// <param name="fileIndex">Index to return.</param>
    /// <returns>Path of given save file.</returns>
    public static string SaveFileNameSpecific(int fileIndex)
    {
        builder.Clear();
        builder.Append(Application.persistentDataPath).Append("/savefile").Append(fileIndex.ToString()).Append(".sav");
        return builder.ToString();
    }

    /// <summary>
    /// Save path for collectibles.
    /// </summary>
    /// <returns>File path of collectibles save file.</returns>
    public static string CollectiblesSavePath()
    {
        builder.Clear();
        builder.Append(Application.persistentDataPath).Append("/collectibles").Append(".sav");
        return builder.ToString();
    }

    /// <summary>
    /// Tutorials Codex save path.
    /// </summary>
    /// <returns>File path of tutorial codex entries save file.</returns>
    public static string TutorialEntriesSavePath()
    {
        builder.Clear();
        builder.Append(Application.persistentDataPath).Append("/tutorials").Append(".sav");
        return builder.ToString();
    }

    /// <summary>
    /// Checks for default/active save slot.
    /// </summary>
    /// <returns>True if file is found.</returns>
    public static bool CheckForSaveFile()
    {
        return File.Exists(SaveFileName());
    }

    /// <summary>
    /// Checks for a specific save file at an index.
    /// </summary>
    /// <param name="fileIndex">Save file index to check.</param>
    /// <returns>True if file is found.</returns>
    public static bool CheckForSaveFile(int fileIndex)
    {
        return File.Exists(SaveFileNameSpecific(fileIndex));
    }

    /// <summary>
    /// Checks all possible save file slots.
    /// </summary>
    /// <returns>Returns true if one is found.</returns>
    public static bool CheckForSaveFiles()
    {
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.sav");

        foreach (string filePath in filePaths)
        {
            if (File.Exists(filePath))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Deletes the file at a given save file index. Will only occur if file actually exists.
    /// </summary>
    /// <param name="fileIndex">File to delete.</param>
    public static bool DeleteSpecifiedSaveFile(int fileIndex)
    {
        if (File.Exists(SaveFileNameSpecific(fileIndex)))
        {
            File.Delete(SaveFileNameSpecific(fileIndex));
            return true;
        }
        return false;
    }

    #endregion

    #region Saving

    /// <summary>
    /// DEPRECIATED - USE SaveAsyncronously INSTEAD.
    /// Saves game using the main thread.
    /// </summary>
    public static void Save()
    {
        HandleSaving();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(collectibles, true));
    }

    /// <summary>
    /// Saves the game asyncronously on another thread.
    /// </summary>
    /// <returns></returns>
    public static async Task SaveAsyncronously()
    {
        await SaveAsync();
    }

    /// <summary>
    /// Writes the save data to Json.
    /// </summary>
    /// <returns></returns>
    private static async Task SaveAsync()
    {
        HandleSaving();

        await File.WriteAllTextAsync(SaveFileName(), JsonUtility.ToJson(_saveData[activeSaveSlot], true));
    }

    /// <summary>
    /// Handles and calls the save function on each of the saved components.
    /// </summary>
    private static void HandleSaving()
    {
        // Player
        GameManager.Get().PlayerStatus.Save(ref _saveData[activeSaveSlot].playerStatus);
        GameManager.Get().RespawnComponent.Save(ref _saveData[activeSaveSlot].playerRespawn);
        GameManager.Get().CurrencyManager.Save(ref _saveData[activeSaveSlot].playerCurrency);
        GameManager.Get().Inventory.Save(ref _saveData[activeSaveSlot].playerInventory);

        GameManager.Get().AbilityManager.Save(ref _saveData[activeSaveSlot].abilityWheelManager);
        GameManager.Get().UnlockManager.Save(ref _saveData[activeSaveSlot].unlockManager);

        // Scenes
        GameManager.Get().SceneData.Save(ref _saveData[activeSaveSlot].sceneSaveData);

        // Progression
        GameManager.Get().ProgressionManager.Save(ref _saveData[activeSaveSlot].progressionManagerData);
    }

    #endregion

    #region Loading

    /// <summary>
    /// DEPRECIATED - USE LoadAsync INSTEAD.
    /// Loads the game on the main thread.
    /// </summary>
    public static void Load()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveData = File.ReadAllText(SaveFileName());

            _saveData[activeSaveSlot] = JsonUtility.FromJson<SaveData>(saveData);

            HandleLoading();
        }
    }

    /// <summary>
    /// Loads the data from a save file asyncronously on another thread.
    /// </summary>
    /// <returns></returns>
    public static async Task LoadAsync()
    {
        if (File.Exists(SaveFileName()))
        {
            string saveData = File.ReadAllText(SaveFileName());

            _saveData[activeSaveSlot] = JsonUtility.FromJson<SaveData>(saveData);

            await HandleLoadingAsync();
        }
    }

    /// <summary>
    /// Loads only Player stats asyncronously on another thread.
    /// </summary>
    /// <returns></returns>
    public static async Task LoadNewLevelAsync(string sceneToLoad)
    {
        if (File.Exists(SaveFileName()))
        {
            string saveData = File.ReadAllText(SaveFileName());

            _saveData[activeSaveSlot] = JsonUtility.FromJson<SaveData>(saveData);

            await HandleLoadingNewLevelAsync(sceneToLoad);
        }
    }

    /// <summary>
    /// Handles and calls the load function on each saved component async.
    /// </summary>
    /// <returns></returns>
    private static async Task HandleLoadingAsync()
    {
        GameManager.Get().LoadScene(_saveData[activeSaveSlot].sceneSaveData.savedSceneID);
        //await GameManager.Get().SceneData.LoadAsync(_saveData.sceneSaveData);

        await GameManager.Get().SceneData.WaitForSceneLoad();

        GameManager.Get().ProgressionManager.Load(_saveData[activeSaveSlot].progressionManagerData);

        // Player
        GameManager.Get().PlayerStatus.Load(_saveData[activeSaveSlot].playerStatus);
        GameManager.Get().RespawnComponent.Load(_saveData[activeSaveSlot].playerRespawn);
        GameManager.Get().CurrencyManager.Load(_saveData[activeSaveSlot].playerCurrency);
        GameManager.Get().Inventory.Load(_saveData[activeSaveSlot].playerInventory);

        //GameManager.Get().WeaponManager.Load(_saveData[activeSaveSlot].weaponManager);
        GameManager.Get().AbilityManager.Load(_saveData[activeSaveSlot].abilityWheelManager);
        GameManager.Get().UnlockManager.Load(_saveData[activeSaveSlot].unlockManager);
    }

    private static async Task HandleLoadingNewLevelAsync(string sceneToLoad)
    {
        GameManager.Get().LoadScene(sceneToLoad);

        await GameManager.Get().SceneData.WaitForSceneLoad();

        // No need to load ProgressionManager or RespawnComponent here - new scene.

        if (sceneToLoad != "mainMenu")
        {
            // Player
            GameManager.Get().PlayerStatus.Load(_saveData[activeSaveSlot].playerStatus);
            GameManager.Get().CurrencyManager.Load(_saveData[activeSaveSlot].playerCurrency);
            GameManager.Get().Inventory.Load(_saveData[activeSaveSlot].playerInventory);

            //GameManager.Get().WeaponManager.Load(_saveData[activeSaveSlot].weaponManager);
            GameManager.Get().AbilityManager.Load(_saveData[activeSaveSlot].abilityWheelManager);
            GameManager.Get().UnlockManager.Load(_saveData[activeSaveSlot].unlockManager);
        }
    }

    /// <summary>
    /// DEPRECIATED - USE HandleLoadingAsync INSTEAD
    /// Handles and calls the load function on each saved component using main thread.
    /// </summary>
    private static void HandleLoading()
    {
        // Player
        GameManager.Get().PlayerStatus.Load(_saveData[activeSaveSlot].playerStatus);
        GameManager.Get().RespawnComponent.Load(_saveData[activeSaveSlot].playerRespawn);
        GameManager.Get().CurrencyManager.Load(_saveData[activeSaveSlot].playerCurrency);
        GameManager.Get().Inventory.Load(_saveData[activeSaveSlot].playerInventory);

        //GameManager.Get().WeaponManager.Load(_saveData[activeSaveSlot].weaponManager);
        GameManager.Get().AbilityManager.Load(_saveData[activeSaveSlot].abilityWheelManager);
        GameManager.Get().UnlockManager.Load(_saveData[activeSaveSlot].unlockManager);

        // Scenes
        GameManager.Get().SceneData.Load(_saveData[activeSaveSlot].sceneSaveData);

        // Progression
        GameManager.Get().ProgressionManager.Load(_saveData[activeSaveSlot].progressionManagerData);
    }

    #endregion

    #region Collectibles 

    /// <summary>
    /// Sets the collectible as obtained and calls the save function.
    /// </summary>
    /// <param name="index">Collectible type.</param>
    public static async void CollectbleObtained(int index)
    {
        collectibles[index] = 1;

        isCollectiblesSaving = true;
        await SaveCollectiblesAsync();
        isCollectiblesSaving = false;
    }

    /// <summary>
    /// Check if a collectible at an index is unlocked.
    /// </summary>
    /// <param name="index">Collectible type.</param>
    /// <returns>True if collectible is obtained.</returns>
    public static bool IsCollectibleObtained(int index)
    {
        return collectibles[index] == 1;
    }

    /// <summary>
    /// Asyncoronously saves the collectibles to file.
    /// </summary>
    /// <returns></returns>
    private static async Task SaveCollectiblesAsync()
    {
        SaveCollectibles(ref collectiblesData);

        await File.WriteAllTextAsync(CollectiblesSavePath(), JsonUtility.ToJson(collectiblesData, true));
    }

    /// <summary>
    /// Loads the collectibles data from a file asyncronously on another thread.
    /// </summary>
    /// <returns></returns>
    public static void LoadCollectibles()
    {
        if (File.Exists(CollectiblesSavePath()))
        {
            string saveData = File.ReadAllText(CollectiblesSavePath());

            collectiblesData = JsonUtility.FromJson<CollectiblesData>(saveData);

            LoadCollectibles(collectiblesData);
        }
    }

    /// <summary>
    /// Handles and saves the collectibles to json.
    /// </summary>
    public static void SaveCollectibles(ref CollectiblesData data)
    {
        data._collectibles = collectibles;
    }

    /// <summary>
    /// Handles and loads the collectibles from json.
    /// </summary>
    /// <param name="data">Data to load.</param>
    public static void LoadCollectibles(CollectiblesData data)
    {
        collectibles = data._collectibles;
    }

    /// <summary>
    /// Deletes collectibles.sav file.
    /// </summary>
    public static void WipeCollectiblesProgress()
    {
        if (File.Exists(CollectiblesSavePath()))
        {
            for (int i = 0; i < collectibles.Length; i++)
            {
                collectibles[i] = 0;
            }

            File.Delete(CollectiblesSavePath());
        }
    }

    #endregion

    #region Tutorial Codex 

    /// <summary>
    /// Sets the tutorial codex as unlocked and calls the save function.
    /// </summary>
    /// <param name="index">Tutorial index.</param>
    public static async void SaveTutorialCodexEntries()
    {
        isCodexSaving = true;
        await SaveTutorialCodexAsync();
        isCodexSaving = false;
    }

    /// <summary>
    /// Check if a tutorial codex at an index is unlocked.
    /// </summary>
    /// <param name="index">Collectible type.</param>
    /// <returns>True if tutorial is already unlocked.</returns>
    public static bool IsTutorialCodexUnlocked(string tutorial)
    {
        return UnlockedTutorialEntries.Contains(tutorial);
    }

    /// <summary>
    /// Asyncoronously saves the tutorials to file.
    /// </summary>
    /// <returns></returns>
    private static async Task SaveTutorialCodexAsync()
    {
        SaveTutorials(ref tutorialCodexData);

        await File.WriteAllTextAsync(TutorialEntriesSavePath(), JsonUtility.ToJson(tutorialCodexData, true));
    }

    /// <summary>
    /// Loads the tutorials data from a file asyncronously on another thread.
    /// </summary>
    /// <returns></returns>
    public static void LoadTutorialCodex()
    {
        if (File.Exists(TutorialEntriesSavePath()))
        {
            string saveData = File.ReadAllText(TutorialEntriesSavePath());

            tutorialCodexData = JsonUtility.FromJson<TutorialCodexData>(saveData);

            LoadTutorials(tutorialCodexData);
        }
    }

    /// <summary>
    /// Handles and saves the tutorials to json.
    /// </summary>
    public static void SaveTutorials(ref TutorialCodexData data)
    {
        data._tutorialCodexEntries = UnlockedTutorialEntries;
    }

    /// <summary>
    /// Handles and loads the tutorials from json.
    /// </summary>
    /// <param name="data">Data to load.</param>
    public static void LoadTutorials(TutorialCodexData data)
    {
        UnlockedTutorialEntries = data._tutorialCodexEntries;
    }

    /// <summary>
    /// Deletes the tutorials.sav file.
    /// </summary>
    public static void WipeTutorialsProgress()
    {
        if (File.Exists(TutorialEntriesSavePath()))
        {
            UnlockedTutorialEntries.Clear();
            File.Delete(TutorialEntriesSavePath());
        }
    }

    #endregion

    #region Wipe Game Data

    /// <summary>
    /// Wipes every active save file.
    /// </summary>
    static void WipeSaveFiles()
    {
        for (int i = 0; i < _saveData.Length; i++)
        {
            if (File.Exists(SaveFileNameSpecific(i)))
            {
                DeleteSpecifiedSaveFile(i);
            }
        }
        activeSaveSlot = 0;
    }

    /// <summary>
    /// Wipes all SaveManager data.
    /// </summary>
    public static void WipeGameData()
    {
        WipeCollectiblesProgress();
        WipeTutorialsProgress();
        WipeSaveFiles();
    }

    #endregion
}