using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Script for handling options menu UI logic.
/// </summary>
public class OptionsMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.Options;

    public UIType previousMenu;

    // Starting Selection
    [SerializeField] GameObject optionsStart;
    public GameObject startSelection => optionsStart;

    // Menus
    GameObject activeSubmenu;
    public GameObject ActiveSubmenu => activeSubmenu;
    GameObject submenuOption;

    bool isInSubmenu = false;
    public bool IsInSubmenu => isInSubmenu;

    // Dropdown Menus
    [SerializeField] GameObject activeDropdownOption;

    [SerializeField] bool isInDropdown = false;
    public bool IsInDropdown => isInDropdown;

    // Gameplay
    [Header("Submenu Scripts")]
    [SerializeField] OptionsGameplay gameplayOptions;

    // Video
    [SerializeField] OptionsVideo videoOptions;

    // Audio
    [SerializeField] OptionsAudio audioOptions;

    // Controls
    [SerializeField] OptionsCameraControls keyboardControlsOptions;
    [SerializeField] OptionsCameraControls gamepadControlsOptions;

    // Data Management
    [SerializeField] OptionsData gameDataOptions; 
    [SerializeField] Button gameDataButton;

    public OptionsData GameDataOptions => gameDataOptions;

    #endregion

    #region Start

    /// <summary>
    /// Sets the current settings to be what the Unity currently has them set to.
    /// Finds and sets available screen resolutions based on user hardware.
    /// </summary>
    void Start()
    {
        InitializeOptions();
    }

    #endregion

    #region Initialize All Options, Set Defaults

    /// <summary>
    /// Runs through and initializes the submenus.
    /// </summary>
    void InitializeOptions()
    {
        gameplayOptions.Initialize();
        videoOptions.Initialize();
        audioOptions.Initialize();
        keyboardControlsOptions.Initialize();
        gamepadControlsOptions.Initialize();

        if (GameManager.Get().SceneData.sceneData.uniqueSceneName == "mainMenu")
        {
            gameDataButton.gameObject.SetActive(true);
        }
    }

    #endregion

    #region Submenus

    /// <summary>
    /// Switches which submenu is active and displayed in the Options menu.
    /// </summary>
    public void SwitchDisplayedSubmenu(GameObject menu)
    {
        if (activeSubmenu == menu) return;

        // IF old active menu, make it no longer active before switching to new menu.
        if (activeSubmenu != null)
        {
            activeSubmenu.SetActive(false);
        }

        // Switch Displayed Menu to current menu
        activeSubmenu = menu;
        activeSubmenu.SetActive(true);
    }

    /// <summary>
    /// Enters into a given submenu.
    /// </summary>
    public void EnterSubmenu()
    {
        submenuOption = EventSystem.current.currentSelectedGameObject;

        isInSubmenu = true;
    }

    /// <summary>
    /// Exits out of a submenu and returns navigator to core options.
    /// </summary>
    public void ExitSubmenu()
    {
        isInSubmenu = false;

        EventSystem.current.SetSelectedGameObject(submenuOption);
        submenuOption = null;
    }

    /// <summary>
    /// Exits out of a submenu if a different core menu option is hovered.
    /// </summary>
    public void ExitSubmenuByHover()
    {
        isInSubmenu = false;
        submenuOption = null;
    }

    #endregion

    #region Dropdown Menus

    /// <summary>
    /// Enters into a dropdown menu within an options submenu.
    /// </summary>
    public void EnterDropdownMenu(GameObject thisDropdown)
    {
        activeDropdownOption = thisDropdown;

        isInDropdown = true;
    }

    /// <summary>
    /// Exits from a dropdown menu.
    /// </summary>
    public void ExitDropdownMenu()
    {
        isInDropdown = false;

        EventSystem.current.SetSelectedGameObject(activeDropdownOption);
        activeDropdownOption = null;
    }

    /// <summary>
    /// Exits from a dropdown menu when another option is hovered.
    /// </summary>
    public void ExitDropdownByHover()
    {
        isInDropdown = false;
        activeDropdownOption = null;
    }

    #endregion

    #region Back Button

    /// <summary>
    /// Returns to the Main Menu or Pause Menu and closes.
    /// </summary>
    public void Back()
    {
        // Reopen Main/Pause Menu
        if (previousMenu == UIType.Main)
        {
            GameManager.Get().GetUIManager().ShowMainMenu();
        }
        else if (previousMenu == UIType.Pause)
        {
            GameManager.Get().GetUIManager().ShowPauseUI();
        }

        // Close Options Menu
        GameManager.Get().GetUIManager().CloseOptionsMenu();
    }

    #endregion
}