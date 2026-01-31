using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Tracks all objects in a scene whose state should be saved.
/// </summary>
public class ProgressionManager : MonoBehaviour
{
    #region Member Variables

    public Dictionary<uint, string> savedProgression = new Dictionary<uint, string>();

    #endregion

    private void Awake()
    {
        GameManager.Get().ProgressionManager = this;
    }

    #region Save and Load

    /// <summary>
    /// Wipes the dictionary. Used for when loading checkpoints.
    /// </summary>
    public void WipeProgressionManager()
    {
        savedProgression.Clear();
    }

    /// <summary>
    /// Saves to SaveManager.
    /// </summary>
    /// <param name="data">Reference to ProgressionManagerData.</param>
    public void Save(ref ProgressionManagerData data)
    {
        data._savedID = new uint[savedProgression.Count];
        data._savedString = new string[savedProgression.Count];
        for (int i = 0; i < savedProgression.Count; i++)
        {
            data._savedID[i] = savedProgression.ElementAt(i).Key;
            data._savedString[i] = savedProgression.ElementAt(i).Value;
        }
    }

    /// <summary>
    /// Function for adding progression objects to be saved at next checkpoint.
    /// </summary>
    /// <param name="objToSave">Progression object to add to dictionary.</param>
    public void SaveMe(IProgressionObject objToSave)
    {
        /// Remove old data, replace with anything new.
        if (savedProgression.ContainsKey(objToSave.UniqueID))
        {
            savedProgression.Remove(objToSave.UniqueID);
        }

        savedProgression[objToSave.UniqueID] = objToSave.SaveAction();
    }

    /// <summary>
    /// Removes a specific IProgressionObject from the dictionary.
    /// </summary>
    /// <param name="objToUnsave">Object to remove from saving.</param>
    public void UnsaveMe(IProgressionObject objToUnsave)
    {
        if (savedProgression.ContainsKey(objToUnsave.UniqueID))
        {
            savedProgression.Remove(objToUnsave.UniqueID);
        }
    }

    /// <summary>
    /// Loads and sets the states of all saved progression objects in a scene.
    /// </summary>
    /// <param name="data">Saved ProgressionManagerData.</param>
    public void Load(ProgressionManagerData data)
    {
        for (int i = 0; i < data._savedID.Length; i++)
        {
            savedProgression[data._savedID[i]] = data._savedString[i];
        }

        foreach (IProgressionObject obj in FindObjectsOfType<MonoBehaviour>(true).OfType<IProgressionObject>())
        {
            if (savedProgression.TryGetValue(obj.UniqueID, out string objData))
            {
                obj.LoadAction(objData);
            }
        }
    }

    #endregion
}

/// <summary>
/// Data to be saved by Progression Manager.
/// </summary>
[System.Serializable]
public struct ProgressionManagerData
{
    public uint[] _savedID;
    public string[] _savedString;
}