using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Contains and loads Scenes based on their unique name and index.
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] SceneDataObject[] sceneDataObjects;
    Dictionary<string, int> sceneIDToIndex = new Dictionary<string, int>();

    private void Awake()
    {
        GameManager.Get().SceneLoadManager = this;

        PopulateSceneMappings();
    }

    /// <summary>
    /// Populates the Dictionary with the data from each Scene Data Object held within the array.
    /// </summary>
    void PopulateSceneMappings()
    {
        foreach (var sceneDataObj in sceneDataObjects)
        {
            sceneIDToIndex[sceneDataObj.uniqueSceneName] = sceneDataObj.sceneIndex;
        }
    }

    /// <summary>
    /// Contains and loads Scenes based on their unique name and index.
    /// </summary>
    /// <param name="sceneID"> The string of the scene to load passed in from Load. Tries to get build index from that name. </param>
    public void LoadSceneByIndex(string sceneID)
    {
        if (sceneIDToIndex.TryGetValue(sceneID, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"No saved Scene Data found for ID: {sceneIndex}");
        }
    }

    /// <summary>
    /// Loads Scenes based on their unique name and index asyncronously.
    /// </summary>
    /// <param name="sceneID"> The string of the scene to load passed in from Load. Tries to get build index from that name. </param>
    public async Task LoadSceneByIndexAsync(string sceneID)
    {
        if (Application.isPlaying == false)
        {
            return; // Stop async method execution.
        }

        if (sceneIDToIndex.TryGetValue(sceneID, out int sceneIndex))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                    break;
                }

                await Task.Yield();
                if (Application.isPlaying == false)
                {
                    return; // Stop async method execution.
                }
            }
        }
        else
        {
            Debug.LogError($"No saved Scene Data found for ID: {sceneIndex}");
        }
    }
}
