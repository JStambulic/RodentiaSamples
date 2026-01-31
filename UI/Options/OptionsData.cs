using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Options Submenu for managing game data.
/// </summary>
public class OptionsData : MonoBehaviour
{
    #region Member Variables

    [SerializeField] public Button tutorials;
    [SerializeField] public Button collectibles;

    #endregion

    private void Start()
    {
        if (!File.Exists(SaveManager.TutorialEntriesSavePath()))
        {
            tutorials.interactable = false;
        }

        if (!File.Exists(SaveManager.CollectiblesSavePath()))
        {
            collectibles.interactable = false;
        }
    }

    #region Reset Functions

    /// <summary>
    /// Reset Tutorials.
    /// </summary>
    public void ResetTutorials()
    {
        GameManager.Get().GetUIManager().CreateDataConfirmationMenu(UIType.Options, MenuReturnType.optionsTutorials);
    }

    /// <summary>
    /// Reset Collectibles.
    /// </summary>
    public void ResetCollectibles()
    {
        GameManager.Get().GetUIManager().CreateDataConfirmationMenu(UIType.Options, MenuReturnType.optionsCollectibles);
    }

    /// <summary>
    /// Reset All Settings. Closes Options menu.
    /// </summary>
    public void ResetGameSettings()
    {
        GameManager.Get().GetUIManager().CreateDataConfirmationMenu(UIType.Options, MenuReturnType.optionsSettings);
    }

    /// <summary>
    /// Reset ALL progress and settings. Reboots the game.
    /// </summary>
    public void WipeAllGameData()
    {
        GameManager.Get().GetUIManager().CreateDataConfirmationMenu(UIType.Options, MenuReturnType.optionsGameData);
    }

    #endregion
}
