using UnityEngine;

/// <summary>Script for handling main menu UI logic.</summary>
public class MainMenu : MonoBehaviour, IBaseUI
{
    // UI Type
    public UIType type => UIType.Main;

    // Starting Selection
    [SerializeField] GameObject mainMenuStart;
    public GameObject startSelection => mainMenuStart;

    [SerializeField] GameObject returnToMain;

    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject creditsUI;

    bool isInCredits = false;
    public bool IsInCredits => isInCredits;

    [SerializeField] SceneDataObject combatScene;
    [SerializeField] SceneDataObject abilitiesScene;

    [Header("To Be Deleted")]
    [SerializeField] GameObject testLevelsSelection;

    private void Start()
    {
        GameManager.Get().PlayerInputCachedRef.SwitchCurrentActionMap("UI");
    }

    #region File Selection

    public void OpenFileSelect()
    {
        GameManager.Get().GetUIManager().CreateFileSelectMenu();

        GameManager.Get().GetUIManager().HideMainMenu();
    }

    #endregion

    #region Quit

    /// <summary>Opens a confirmation menu to confirm you would like to quit.</summary>
    /// <returns>Void.</returns>
    public void QuitGame()
    {
        GameManager.Get().GetUIManager().CreateConfirmationMenu(type, MenuReturnType.quitConfirm);
    }

    #endregion

    #region Options

    /// <summary>Creates an option menu and destroys the current menu.</summary>
    /// <returns>Void.</returns>
    public void OpenOptions()
    {
        GameManager.Get().GetUIManager().CreateOptionsMenu(type);

        GameManager.Get().GetUIManager().HideMainMenu();
    }

    #endregion

    #region Collectibles

    public void OpenCollectibles()
    {
        GameManager.Get().GetUIManager().CreateCollectiblesMenu();

        GameManager.Get().GetUIManager().HideMainMenu();
    }

    #endregion

    #region Tutorial Codex

    public void OpenTutorialCodex()
    {
        GameManager.Get().GetUIManager().CreateTutorialCodexMenu(type);

        GameManager.Get().GetUIManager().HideMainMenu();
    }

    #endregion

    #region Credits

    public void EnterCredits()
    {
        mainMenuUI.SetActive(false);
        creditsUI.SetActive(true);
        isInCredits = true;
    }

    public void ExitCredits()
    {
        mainMenuUI.SetActive(true);
        creditsUI.SetActive(false);
        isInCredits = false;
    }

    #endregion 

    #region Menu Navigation

    /// <summary>
    /// Updates selection to sub menus based on a UI button being clicked.
    /// </summary>
    /// <param name="button"> The UI button being clicked </param>
    public void SetSubmenuSelected(GameObject button)
    {
        isInCredits = true;
        SelectLogic.SetSelectionFromButton(button);
    }

    /// <summary>
    /// Backs out to the last selection from current menu/sub menu
    /// </summary>
    public void Back()
    {
        isInCredits = false;
        SelectLogic.SelectPreviousFromButton(gameObject);
    }

    #endregion

    public void LoadCombat()
    {
        SceneLoadLogic.LoadScene(combatScene);
    }

    public void LoadAbilities()
    {
        SceneLoadLogic.LoadScene(abilitiesScene);
    }
}
