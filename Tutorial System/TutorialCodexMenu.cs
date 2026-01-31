using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialCodexMenu : MonoBehaviour, IBaseUI
{
    [SerializeField] GameObject codexSelection;
    public GameObject startSelection => codexSelection;

    public UIType previousMenu;

    public UIType type => UIType.TutorialCodex;

    // Menus
    GameObject activeTutorial;
    public GameObject ActiveTutorial => activeTutorial;

    bool isInSubmenu = false;
    public bool IsInSubmenu => isInSubmenu;
    GameObject submenuOption;

    [Header("Tutorial Screen")]
    [SerializeField] GameObject tutorialFullscreenPrefab;

    #region Submenus

    /// <summary>
    /// Enters into a given tutorial submenu.
    /// </summary>
    public void EnterSubmenu(TutorialInfoObject tutorial)
    {
        gameObject.SetActive(false);

        submenuOption = EventSystem.current.currentSelectedGameObject;

        activeTutorial = Instantiate(tutorialFullscreenPrefab);
        activeTutorial.GetComponent<TutorialMenu>().SetTutorial(tutorial, false);

        isInSubmenu = true;
    }

    /// <summary>
    /// Exits out of a tutorial submenu.
    /// </summary>
    public void ExitSubmenu()
    {
        gameObject.SetActive(true);

        Destroy(activeTutorial);
        activeTutorial = null;

        EventSystem.current.SetSelectedGameObject(submenuOption);

        isInSubmenu = false;
    }

    #endregion

    #region Back Button

    /// <summary>
    /// Returns to the Main Menu or Pause Menu and closes.
    /// </summary>
    public void Back()
    {
        // Reopen Main/Pause Menu
        switch (previousMenu)
        {
            case UIType.Main:
                GameManager.Get().GetUIManager().ShowMainMenu();
                break;

            case UIType.Pause:
                GameManager.Get().GetUIManager().ShowPauseUI();
                break;

            case UIType.Player:
                GameManager.Get().GetUIManager().ShowPlayerUI();
                GameManager.Get().ResumeGame();
                break;

            default:
                break;
        }

        // Close Tutorial Codex Menu
        GameManager.Get().GetUIManager().CloseTutorialCodexMenu();
    }

    #endregion
}
