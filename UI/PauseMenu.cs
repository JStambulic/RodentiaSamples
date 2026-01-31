using UnityEngine;

/// <summary>Script for handling pause menu UI logic.</summary>
public class PauseMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.Pause;

    // Starting Selection
    [SerializeField] GameObject pauseStart;
    public GameObject startSelection => pauseStart;

    #endregion

    #region Pause & Resume

    /// <summary>Purely used for OnButtonPressed in the UI.</summary>
    public void ResumeGameButton()
    {
        GameManager.Get().GetPlayer().GetComponent<MenuHandler>().EndPause();
    }

    #endregion

    #region Options

    /// <summary>Creates an option menu and destroys the current menu.</summary>
    /// <returns>Void.</returns>
    public void OpenOptions()
    {
        GameManager.Get().GetUIManager().CreateOptionsMenu(type);

        GameManager.Get().GetUIManager().HidePauseUI();
    }

    #endregion

    #region Tutorial Codex

    /// <summary>
    /// Opens the settings tutorial codex.
    /// </summary>
    public void OpenTutorialCodex()
    {
        GameManager.Get().GetUIManager().CreateTutorialCodexMenu(type);

        GameManager.Get().GetUIManager().HidePauseUI();
    }

    #endregion

    #region Main Menu & Quit

    /// <summary>Opens a confirmation menu to confirm you would like to return to main menu.</summary>
    /// <returns>Void.</returns>
    public void LoadMenu()
    {
        GameManager.Get().GetUIManager().CreateConfirmationMenu(type, MenuReturnType.menuConfirm);
    }

    /// <summary>Opens a confirmation menu to confirm you would like to quit.</summary>
    /// <returns>Void.</returns>
    public void QuitGame()
    {
        GameManager.Get().GetUIManager().CreateConfirmationMenu(type, MenuReturnType.quitConfirm);
    }

    #endregion
}
