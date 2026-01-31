using TMPro;
using Tools;
using UnityEngine;

/// <summary>
/// Menu types that can lead to a ConfirmationUI.
/// </summary>
public enum MenuReturnType
{
    menuConfirm,
    quitConfirm,
    optionsSettings,
    optionsTutorials,
    optionsCollectibles,
    optionsGameData
}

/// <summary>
/// Confirmation UI to ensure accidental clicks on Main Menu or Quit do not immediately do the given action.
/// </summary>
public class ConfirmationMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.Confirmation;

    // Starting Selection
    [SerializeField] protected GameObject confirmationStart;
    public GameObject startSelection => confirmationStart;

    public UIType previousMenu;

    public MenuReturnType buttonType;

    public SceneDataObject mainMenuScene;

    [SerializeField] protected TMP_Text textLabel;
    string mainMenuQuitKey = "sureToQuitMain";
    string quitGamePauseKey = "sureToQuitPause";
    string backToMenuPauseKey = "backToMenuPause";

    #endregion

    #region Start
    /// <summary>
    /// Sets the text of the main box.
    /// </summary>
    void Start()
    {
        if (buttonType == MenuReturnType.menuConfirm)
        {
            textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", backToMenuPauseKey);
        }

        if (buttonType == MenuReturnType.quitConfirm && previousMenu == UIType.Pause)
        {
            textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", quitGamePauseKey);
        }
        else if (buttonType == MenuReturnType.quitConfirm)
        {
            textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", mainMenuQuitKey);
        }
    }
    #endregion

    #region Yes Button

    /// <summary>
    /// If Yes is selected, check menu return type. If menu, return to main menu. If quit, close the application.
    /// </summary>
    public virtual void YesSelected()
    {
        // Return to Main Menu.
        if (buttonType == MenuReturnType.menuConfirm)
        {
            //Debug.Log("Back to Menu...");
            GameManager.Get().GetUIManager().HidePauseUI();

            SceneLoadLogic.LoadScene(mainMenuScene);
        }
        // Quit game.
        else if (buttonType == MenuReturnType.quitConfirm)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit();
#endif
        }

        // Close Confirmation Menu
        GameManager.Get().GetUIManager().CloseConfirmationMenu();
    }

    #endregion

    #region No Button

    /// <summary>
    /// Close the confirmation menu when No is selected.
    /// </summary>

    public virtual void NoSelected()
    {
        // Close Confirmation Menu
        GameManager.Get().GetUIManager().CloseConfirmationMenu();
    }

    #endregion
}
