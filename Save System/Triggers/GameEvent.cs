using Tools;
using UnityEngine;

/// <summary>
/// Base class for saving triggers to Progression Manager.
/// </summary>
public class GameEvent : MonoBehaviour, IProgressionObject
{
    public uint UniqueID => HelperFunctions.GenerateUniqueID(objectType, eventName);

    public bool isSaveable => true;

    public virtual ProgressionObject objectType => ProgressionObject.gameEvent;

    public string eventName = "Default";
    public bool wasTriggered = false;

    [System.Serializable]
    class GameEventSaveData
    {
        public bool _wasTriggered;
    }

    /// <summary>
    /// Saves whether this event was triggered to Progression Manager.
    /// </summary>
    /// <returns>A Json string containing save information.</returns>
    public virtual string SaveAction()
    {
        GameEventSaveData data = new GameEventSaveData();
        data._wasTriggered = wasTriggered;

        return JsonUtility.ToJson(data);
    }

    /// <summary>
    /// Loads from Progression Manager whether this event was triggered.
    /// </summary>
    /// <param name="data">Json string containing data.</param>
    public virtual void LoadAction(string data)
    {
        GameEventSaveData load = JsonUtility.FromJson<GameEventSaveData>(data);

        wasTriggered = load._wasTriggered;
    }
}
