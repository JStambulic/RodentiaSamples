using UnityEngine;

public class SceneLoadLogic : MonoBehaviour
{
    public static void LoadScene(SceneDataObject sceneData)
    {
        GameManager.Get().LoadScene(sceneData.uniqueSceneName);
    }
}
