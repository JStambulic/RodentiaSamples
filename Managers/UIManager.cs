using System;
using UnityEngine;

/// <summary>
/// Bitwise enum for UI types. Can handle up to 32 menu types.
/// </summary>
[Flags]
public enum UIType : int
{
    Main = 1 << 0,
    Options = 1 << 1,
    Pause = 1 << 2,
    Player = 1 << 3,
    Dialogue = 1 << 4,
    Merchant = 1 << 5,
    Inventory = 1 << 6,
    Confirmation = 1 << 7,
    FileSelect = 1 << 8,
    Cutscene = 1 << 9,
    Tutorial = 1 << 10,
    Collectibles = 1 << 11,
    TutorialCodex = 1 << 12,
}

/// <summary>
/// Creates/maintains/destroys all UI in the game and holds their active instances while they exist.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Member Variables

    private UIType activeUIs;

    #endregion

    private void Start()
    {
        SelectLogic.SelectLogicStart();
    }

    void Update()
    {
        SelectLogic.RegainFocus();
    }

    #region UI Functions

    public bool IsActive(UIType type)
    {
        return (activeUIs & type) > 0;
    }

    public UIType GetActiveMenu()
    {
        return activeUIs;
    }

    #endregion

    #region Main Menu

    [SerializeField] GameObject mainMenuUI;
    GameObject mainMenuUI_Instance;

    public MainMenu GetMainMenu()
    {
        if (!mainMenuUI_Instance)
        {
            CreateMainMenu();
        }

        return mainMenuUI_Instance.GetComponent<MainMenu>();
    }

    public void CreateMainMenu()
    {
        if (!mainMenuUI_Instance)
        {
            mainMenuUI_Instance = Instantiate(mainMenuUI, transform);

            activeUIs |= UIType.Main;

            SelectLogic.SetStartSelection(mainMenuUI_Instance);
        }
    }

    public void CloseMainMenu()
    {
        activeUIs &= ~(UIType.Main);
        SelectLogic.ClearSelection();

        Destroy(mainMenuUI_Instance);
    }

    public void ShowMainMenu()
    {
        if (mainMenuUI_Instance)
        {
            mainMenuUI_Instance.SetActive(true);

            SelectLogic.ResetCurrentSelection(mainMenuUI_Instance);

            activeUIs |= UIType.Main;
        }
    }

    public void HideMainMenu()
    {
        if (mainMenuUI_Instance)
        {
            mainMenuUI_Instance.SetActive(false);

            activeUIs &= ~(UIType.Main);
        }
    }

    #endregion

    #region Options Menu

    [SerializeField] GameObject optionsUI;
    GameObject optionsUI_Instance;

    public OptionsMenu OptionsUI_Instance => optionsUI_Instance.GetComponent<OptionsMenu>();

    public OptionsMenu CreateOptionsMenu(UIType type)
    {
        if (!optionsUI_Instance)
        {
            optionsUI_Instance = Instantiate(optionsUI, transform);
            optionsUI_Instance.GetComponent<OptionsMenu>().previousMenu = type;

            activeUIs |= UIType.Options;
            SelectLogic.SetSelection(optionsUI_Instance);

            return optionsUI_Instance.GetComponent<OptionsMenu>();
        }

        return optionsUI_Instance.GetComponent<OptionsMenu>();
    }

    public void CloseOptionsMenu()
    {
        activeUIs &= ~(UIType.Options);

        // Since this menu can only be accessed via another menu button, return to previous.
        SelectLogic.SelectPrevious();

        Destroy(optionsUI_Instance);
    }

    #endregion

    #region Pause Menu

    [SerializeField] GameObject pauseUI;
    GameObject pauseUI_Instance;

    public PauseMenu CreatePauseMenu()
    {
        if (!pauseUI_Instance)
        {
            pauseUI_Instance = Instantiate(pauseUI, transform);

            activeUIs |= UIType.Pause;

            SelectLogic.SetSelection(pauseUI_Instance);

            return pauseUI_Instance.GetComponent<PauseMenu>();
        }

        return pauseUI_Instance.GetComponent<PauseMenu>();
    }

    public void ClosePauseMenu()
    {
        activeUIs &= ~(UIType.Pause);

        SelectLogic.ClearSelection();

        Destroy(pauseUI_Instance);
    }

    public void ShowPauseUI()
    {
        if (pauseUI_Instance)
        {
            pauseUI_Instance.SetActive(true);

            SelectLogic.ResetCurrentSelection(pauseUI_Instance);

            activeUIs |= UIType.Pause;
        }
    }

    public void HidePauseUI()
    {
        if (pauseUI_Instance)
        {
            pauseUI_Instance.SetActive(false);

            activeUIs &= ~(UIType.Pause);
        }
    }

    #endregion

    #region Player UI

    [SerializeField] GameObject playerUI;
    GameObject playerUI_Instance;

    public void CreatePlayerUI()
    {
        if (!playerUI_Instance)
        {
            playerUI_Instance = Instantiate(playerUI, transform);

            activeUIs |= UIType.Player;
        }
    }

    public void ClosePlayerUI()
    {
        activeUIs &= ~(UIType.Player);

        Destroy(playerUI_Instance);
    }

    public GameObject GetPlayerUIObject()
    {
        if (playerUI_Instance)
        {
            return playerUI_Instance;
        }
        return null;
    }

    public PlayerUI GetPlayerUIComp()
    {
        if (playerUI_Instance)
        {
            return playerUI_Instance.GetComponent<PlayerUI>();
        }
        return null;
    }

    public AbilityUIManager GetAbilityUIComp()
    {
        if (playerUI_Instance)
        {
            return playerUI_Instance.GetComponent<AbilityUIManager>();
        }
        return null;
    }
    public EntityUI GetEntityUIComp()
    {
        if (playerUI_Instance)
        {
            return playerUI_Instance.GetComponent<EntityUI>();
        }
        return null;
    }

    public void ShowPlayerUI()
    {
        if (playerUI_Instance)
        {
            playerUI_Instance.SetActive(true);
            activeUIs |= UIType.Player;
        }
    }

    public void HidePlayerUI()
    {
        if (playerUI_Instance)
        {
            playerUI_Instance.SetActive(false);
            activeUIs &= ~(UIType.Player);
        }
    }

    #endregion

    #region Dialogue UI

    [SerializeField] GameObject dialogueUI;
    GameObject dialogueUI_Instance;

    public void CreateDialogueUI()
    {
        if (!dialogueUI_Instance)
        {
            dialogueUI_Instance = Instantiate(dialogueUI, transform);

            activeUIs |= UIType.Dialogue;
        }
    }

    public void CloseDialogueUI()
    {
        activeUIs &= ~(UIType.Dialogue);

        Destroy(dialogueUI_Instance);
    }

    public GameObject GetDialogueUIObject()
    {
        if (dialogueUI_Instance)
        {
            return dialogueUI_Instance;
        }
        return null;
    }

    public DialogueMenu GetDialogueUIComp()
    {
        if (dialogueUI_Instance)
        {
            return dialogueUI_Instance.GetComponent<DialogueMenu>();
        }
        return null;
    }

    public void ShowDialogueUI()
    {
        if (dialogueUI_Instance)
        {
            dialogueUI_Instance.SetActive(true);
            SelectLogic.SetSelection(dialogueUI_Instance);
            activeUIs |= UIType.Dialogue;
        }
    }

    public void HideDialogueUI()
    {
        if (dialogueUI_Instance)
        {
            dialogueUI_Instance.SetActive(false);
            SelectLogic.ClearSelection();
            activeUIs &= ~(UIType.Dialogue);
        }
    }

    #endregion

    #region Merchant UI

    [SerializeField] GameObject merchantUI;
    GameObject merchantUI_Instance;

    public void CreateMerchantUI(Merchant merchant)
    {
        if (!merchantUI_Instance)
        {
            merchantUI_Instance = Instantiate(merchantUI, transform);
            merchantUI_Instance.GetComponent<MerchantMenu>().merchantInventory = merchant;

            activeUIs |= UIType.Merchant;
        }
    }

    public MerchantMenu GetMerchantUIComp()
    {
        if (merchantUI_Instance)
        {
            return merchantUI_Instance.GetComponent<MerchantMenu>();
        }
        return null;
    }

    public void CloseMerchantUI()
    {
        activeUIs &= ~(UIType.Merchant);

        Destroy(merchantUI_Instance);
    }

    public void ShowMerchantUI()
    {
        if (merchantUI_Instance)
        {
            merchantUI_Instance.SetActive(true);
            SelectLogic.SetSelection(merchantUI_Instance);
            activeUIs |= UIType.Merchant;
        }
    }

    public void HideMerchantUI()
    {
        if (merchantUI_Instance)
        {
            merchantUI_Instance.SetActive(false);
            SelectLogic.ClearSelection();
            activeUIs &= ~(UIType.Merchant);
        }
    }

    #endregion

    #region Confirmation UI

    [SerializeField] GameObject confirmationUI;
    [SerializeField] GameObject dataConfirmationUI;
    GameObject confirmationUI_Instance;

    public ConfirmationMenu ConfirmationUI_Instance => confirmationUI_Instance.GetComponent<ConfirmationMenu>();

    public ConfirmationMenu CreateConfirmationMenu(UIType type, MenuReturnType confirmType)
    {
        if (!confirmationUI_Instance)
        {
            confirmationUI_Instance = Instantiate(confirmationUI, transform);
            confirmationUI_Instance.GetComponent<ConfirmationMenu>().previousMenu = type;
            confirmationUI_Instance.GetComponent<ConfirmationMenu>().buttonType = confirmType;

            activeUIs |= UIType.Confirmation;
            SelectLogic.SetSelection(confirmationUI_Instance);

            return confirmationUI_Instance.GetComponent<ConfirmationMenu>();
        }

        return confirmationUI_Instance.GetComponent<ConfirmationMenu>();
    }

    public ConfirmationMenu CreateDataConfirmationMenu(UIType type, MenuReturnType confirmType)
    {
        if (!confirmationUI_Instance)
        {
            confirmationUI_Instance = Instantiate(dataConfirmationUI, transform);
            confirmationUI_Instance.GetComponent<ConfirmationMenu>().previousMenu = type;
            confirmationUI_Instance.GetComponent<ConfirmationMenu>().buttonType = confirmType;

            activeUIs |= UIType.Confirmation;
            SelectLogic.SetSelection(confirmationUI_Instance);

            return confirmationUI_Instance.GetComponent<ConfirmationMenu>();
        }

        return confirmationUI_Instance.GetComponent<ConfirmationMenu>();
    }

    public void CloseConfirmationMenu()
    {
        activeUIs &= ~(UIType.Confirmation);

        // Since this menu can only be accessed via another menu button, return to previous.
        SelectLogic.SelectPrevious(); 

        if (IsActive(UIType.Main))
        {
            if (mainMenuUI_Instance)
            {
                SelectLogic.ResetCurrentSelection(mainMenuUI_Instance);
            }
        }
        else if (IsActive(UIType.Pause))
        {
            if (pauseUI_Instance)
            {
                SelectLogic.ResetCurrentSelection(pauseUI_Instance);
            }
        }
        else if (IsActive(UIType.Options))
        {
            if (optionsUI_Instance)
            {
                SelectLogic.ResetCurrentSelection(optionsUI_Instance);
            }
        }

        Destroy(confirmationUI_Instance);
    }

    #endregion

    #region File Select UI

    [SerializeField] GameObject fileSelectUI;
    GameObject fileSelectUI_Instance;

    public FileSelectMenu FileSelectUI_Instance => fileSelectUI_Instance.GetComponent<FileSelectMenu>();

    public void CreateFileSelectMenu()
    {
        if (!confirmationUI_Instance)
        {
            fileSelectUI_Instance = Instantiate(fileSelectUI, transform);

            activeUIs |= UIType.FileSelect;

            SelectLogic.SetSelection(fileSelectUI_Instance);
        }
    }

    public void CloseFileSelectMenu()
    {
        activeUIs &= ~(UIType.FileSelect);

        // Since this menu can only be accessed via another menu button, return to previous.
        SelectLogic.SelectPrevious();

        if (IsActive(UIType.Main))
        {
            if (mainMenuUI_Instance)
            {
                SelectLogic.ResetCurrentSelection(mainMenuUI_Instance);
            }
        }

        Destroy(fileSelectUI_Instance);
    }

    #endregion

    #region Cutscene Menu

    [SerializeField] GameObject cutsceneUI;
    GameObject cutsceneUI_Instance;

    public CutsceneDirector CreateCutsceneMenu()
    {
        if (!cutsceneUI_Instance)
        {
            cutsceneUI_Instance = Instantiate(cutsceneUI, transform);

            activeUIs |= UIType.Cutscene;

            return cutsceneUI_Instance.GetComponent<CutsceneDirector>();
        }

        return cutsceneUI_Instance.GetComponent<CutsceneDirector>();
    }

    public void CloseCutsceneMenu()
    {
        activeUIs &= ~(UIType.Cutscene);

        Destroy(cutsceneUI_Instance);
    }

    #endregion

    #region Tutorial Menu

    [SerializeField] GameObject tutorialUI;
    GameObject tutorialUI_Instance;

    public TutorialMenu GetTutorialMenuComp()
    {
        if (tutorialUI_Instance != null)
        {
            return tutorialUI_Instance.GetComponent<TutorialMenu>();
        }
        return null;
    }

    public void CreateTutorialMenu()
    {
        if (!tutorialUI_Instance)
        {
            tutorialUI_Instance = Instantiate(tutorialUI, transform);

            activeUIs |= UIType.Tutorial;
        }
    }

    public void CloseTutorialMenu()
    {
        activeUIs &= ~(UIType.Tutorial);

        Destroy(tutorialUI_Instance);
    }

    public void ShowTutorialUI()
    {
        if (tutorialUI_Instance)
        {
            tutorialUI_Instance.SetActive(true);
            activeUIs |= UIType.Tutorial;
        }
    }

    public void HideTutorialUI()
    {
        if (tutorialUI_Instance)
        {
            tutorialUI_Instance.SetActive(false);
            activeUIs &= ~(UIType.Tutorial);
        }
    }

    #endregion

    #region Collectibles UI

    [SerializeField] GameObject collectiblesUI;
    GameObject collectiblesUI_Instance;

    public CollectiblesUI CollectiblesUI_Instance => collectiblesUI_Instance.GetComponent<CollectiblesUI>();

    public void CreateCollectiblesMenu()
    {
        if (!collectiblesUI_Instance)
        {
            collectiblesUI_Instance = Instantiate(collectiblesUI, transform);

            activeUIs |= UIType.Collectibles;

            SelectLogic.SetSelection(collectiblesUI_Instance);
        }
    }

    public void CloseCollectiblesMenu()
    {
        activeUIs &= ~(UIType.Collectibles);

        // Since this menu can only be accessed via another menu button, return to previous.
        SelectLogic.SelectPrevious();

        if (IsActive(UIType.Main))
        {
            if (mainMenuUI_Instance)
            {
                SelectLogic.ResetCurrentSelection(mainMenuUI_Instance);
            }
        }

        Destroy(collectiblesUI_Instance);
    }

    #endregion

    #region Tutorial Codex Menu

    [SerializeField] GameObject tutorialCodexUI;
    GameObject tutorialCodexUI_Instance;

    public TutorialCodexMenu TutorialCodexUI_Instance => tutorialCodexUI_Instance.GetComponent<TutorialCodexMenu>();

    public OptionsMenu CreateTutorialCodexMenu(UIType type)
    {
        if (!tutorialCodexUI_Instance)
        {
            tutorialCodexUI_Instance = Instantiate(tutorialCodexUI, transform);
            tutorialCodexUI_Instance.GetComponent<TutorialCodexMenu>().previousMenu = type;

            activeUIs |= UIType.TutorialCodex;
            SelectLogic.SetSelection(tutorialCodexUI_Instance);

            return tutorialCodexUI_Instance.GetComponent<OptionsMenu>();
        }

        return tutorialCodexUI_Instance.GetComponent<OptionsMenu>();
    }

    public void CloseTutorialCodexMenu()
    {
        activeUIs &= ~(UIType.TutorialCodex);

        // Since this menu can only be accessed via another menu button, return to previous.
        SelectLogic.SelectPrevious();

        Destroy(tutorialCodexUI_Instance);
    }

    #endregion

    #region Remove UI

    public void CloseAllUI()
    {
        if (mainMenuUI_Instance) { CloseMainMenu(); }
        if (optionsUI_Instance) { CloseOptionsMenu(); }
        if (pauseUI_Instance) { ClosePauseMenu(); }
        if (playerUI_Instance) { ClosePlayerUI(); }
        if (dialogueUI_Instance) { CloseDialogueUI(); }
        if (merchantUI_Instance) { CloseMerchantUI(); }
        if (confirmationUI_Instance) { CloseConfirmationMenu(); }
        if (fileSelectUI_Instance) { CloseFileSelectMenu(); }
        if (cutsceneUI_Instance) { CloseCutsceneMenu(); }
        if (collectiblesUI_Instance) { CloseCollectiblesMenu(); }
        SelectLogic.ClearSelection();
    }

    #endregion
}
