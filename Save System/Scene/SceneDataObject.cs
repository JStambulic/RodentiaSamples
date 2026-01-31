using UnityEngine;

/// <summary>
/// Contains a unique name and holds onto a scene build index.
/// </summary>
[CreateAssetMenu(menuName = "Scene Data", fileName = "New Scene Data")]
public class SceneDataObject : ScriptableObject
{
    public int sceneIndex;
    public string uniqueSceneName;
}
