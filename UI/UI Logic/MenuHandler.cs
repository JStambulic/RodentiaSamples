using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Handles how pausing input occurs on the player, removing the dependency from being purely the UI's job.</summary>
public class MenuHandler : MonoBehaviour
{
    #region Menu Action
    /// <summary>Runs when player presses the Menu button. Will open/close the Pause menu depending on its current state.</summary>
    /// <param InputAction.CallbackContext name="context">The data of the input.</param>
    /// <returns>Void.</returns>
    public void MenuAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameManager.Get() == null)
            {
                return;
            }
            if (GameManager.Get().IsLoading)
            {
                return;
            }

            var uiManager = GameManager.Get().GetUIManager();
            if (uiManager == null) { return; }

            switch (uiManager.GetActiveMenu())
            {
                case UIType.Main:
                    MainMenu main = uiManager.GetMainMenu();
                    if (main.IsInCredits)
                    {
                        main.ExitCredits();
                    }
                    break;

                case UIType.Options:
                    OptionsMenu options = uiManager.OptionsUI_Instance;
                    if (options.IsInDropdown)
                    {
                        options.ExitDropdownMenu();
                    }
                    else if (options.IsInSubmenu)
                    {
                        options.ExitSubmenu();
                    }
                    else
                    {
                        options.Back();
                    }
                    break;

                case UIType.Confirmation:
                    uiManager.ConfirmationUI_Instance.NoSelected();
                    break;

                case UIType.FileSelect:
                    uiManager.FileSelectUI_Instance.Back();
                    break;

                case UIType.Collectibles:
                    uiManager.CollectiblesUI_Instance.Back();
                    break;

                case UIType.Merchant:
                    uiManager.GetMerchantUIComp().CloseShop();
                    break;

                case UIType.TutorialCodex:
                    TutorialCodexMenu tutorialCodex = uiManager.TutorialCodexUI_Instance;
                    if (tutorialCodex.IsInSubmenu)
                    {
                        tutorialCodex.ExitSubmenu();
                    }
                    else
                    {
                        tutorialCodex.Back();
                    }
                    break;

                case UIType.Player:
                case UIType.Pause:
                    if (uiManager.IsActive(UIType.Player) || uiManager.IsActive(UIType.Pause))
                    {
                        if (!GameManager.Get().IsPaused)
                        {
                            BeginPause();
                        }
                        else
                        {
                            EndPause();
                        }
                    }
                    break;

                default:
                    break;

            }
        }
    }
    #endregion

    #region Pause Handler
    /// <summary>Gets UI Manager and creates a Pause UI. From which, access the PauseMenu script to run PauseGame.</summary>
    /// <returns>Void.</returns>
    public void BeginPause()
    {
        if (!GameManager.Get().CanPause)
        {
            return;
        }

        // Hide Player UI, create Pause UI.
        GameManager.Get().GetUIManager().HidePlayerUI();
        GameManager.Get().GetUIManager().CreatePauseMenu();
        GameManager.Get().SetMouseCursor(true);

        // Change Action Map to UI.
        GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");

        GameManager.Get().PauseGame();
    }

    /// <summary>Closes the Pause UI in UI Manager and sets reference to null.</summary>
    /// <returns>Void.</returns>
    public void EndPause()
    {
        // Close Pause UI, re-show Player UI.
        GameManager.Get().GetUIManager().ClosePauseMenu();
        GameManager.Get().GetUIManager().ShowPlayerUI();
        GameManager.Get().SetMouseCursor(false);

        // Reset current Action Map to Player.
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Player");

        GameManager.Get().ResumeGame();
    }
    #endregion
}
