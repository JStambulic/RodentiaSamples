using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the current scene to Save/Load.
/// </summary>
public class SceneData : MonoBehaviour
{
    public SceneDataObject sceneData;

    private void Awake()
    {
        GameManager.Get().SceneData = this;
    }

    #region Save & Load

    /// <summary>
    /// Saves the unique Scene name into file.
    /// </summary>
    public void Save(ref SceneSaveData data)
    {
        data.savedSceneID = sceneData.uniqueSceneName;
    }

    /// <summary>
    /// Loads into a scene from passed in data.
    /// </summary>
    public void Load(SceneSaveData data)
    {
        GameManager.Get().SceneLoadManager.LoadSceneByIndex(data.savedSceneID);
    }

    /// <summary>
    /// Loads into a scene from passed in data asyncronously.
    /// </summary>
    public async Task LoadAsync(SceneSaveData data)
    {
        await GameManager.Get().SceneLoadManager.LoadSceneByIndexAsync(data.savedSceneID);
    }

    /// <summary>
    /// Waits for the scene to be fully loaded before continuing any operations requiring it.
    /// </summary>
    public Task WaitForSceneLoad()
    {
        TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

        UnityEngine.Events.UnityAction<Scene, LoadSceneMode> sceneLoadHandler = null;

        sceneLoadHandler = (scene, mode) =>
        {
            taskCompletion.SetResult(true);
            SceneManager.sceneLoaded -= sceneLoadHandler;
        };

        SceneManager.sceneLoaded += sceneLoadHandler;

        return taskCompletion.Task;
    }   

    #endregion
}

/// <summary>
/// Contains the sceneID to load from.
/// </summary>
[System.Serializable]
public struct SceneSaveData
{
    public string savedSceneID;
}
