using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem;

/// <summary>Logic for EventSystem Gamepad/Keyboard navigation across all relevant menus.</summary>
public class SelectLogic
{
    static string currentControl = "Keyboard";
    public static string CurrentControl => currentControl;

    static GameObject currentSelection;
    static GameObject previousSelection;

    static PlayerInput playerInput;
    static InputAction navigateAction;

    /// <summary>
    /// Initializes Select Logic.
    /// </summary>
    public static void SelectLogicStart()
    {
        if (playerInput == null)
        {
            playerInput = GameManager.Get().PlayerInputCachedRef;

            playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
        }
        if (navigateAction == null)
        {
            if (playerInput != null) { navigateAction = playerInput.actions.FindAction("Navigate"); }
        }
    }
    
    /// <summary>Will regain focus onto current selection when there is no current selected GameObject and a Navigate input is pressed.</summary>
    public static void RegainFocus()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (navigateAction != null)
            {
                if (navigateAction.WasPerformedThisFrame())
                {
                    ResetSelection();
                }
            }
        }
    }

    /// <summary>
    /// Hides the cursor if Gamepad is the current used device.
    /// </summary>
    /// <param name="input">Player input to listen from.</param>
    static void OnControlsChanged(PlayerInput input)
    {
        if (input.currentControlScheme == "Keyboard" &&
            currentSelection != null)
        {
            currentControl = "Keyboard";
            GameManager.Get().SetMouseCursor(true);
        }
        else if (input.currentControlScheme == "Gamepad")
        {
            currentControl = "Gamepad";
            GameManager.Get().SetMouseCursor(false);
        }
    }

    /// <summary>Sets the first selected GameObject in EventSystem.</summary>
    public static void SetStartSelection(GameObject menu)
    {
        EventSystem.current.firstSelectedGameObject = menu.GetComponent<IBaseUI>().startSelection;
        currentSelection = menu.GetComponent<IBaseUI>().startSelection;
    }

    /// <summary>Sets the currently selected GameObject, storing previous selection before swapping.</summary>
    public static void SetSelection(GameObject menu)
    {
        // Sets Previous Selection for if this new menu comes from a previous menu.
        SetPreviousSelection();

        EventSystem.current.SetSelectedGameObject(menu.GetComponent<IBaseUI>().startSelection);
        currentSelection = menu.GetComponent<IBaseUI>().startSelection;
    }

    /// <summary>Sets the currently selected GameObject, storing previous selection before swapping.</summary>
    public static void ResetCurrentSelection(GameObject menu)
    {
        currentSelection = menu.GetComponent<IBaseUI>().startSelection;
    }

    /// <summary>Sets a GameObject button as selected, storing previous selection before swapping.</summary>
    public static void SetSelectionFromButton(GameObject button)
    {
        SetPreviousSelection();

        EventSystem.current.SetSelectedGameObject(button);
        currentSelection = button;
    }

    /// <summary>Resets the selection to be current selection.</summary>
    public static void ResetSelection()
    {
        EventSystem.current.SetSelectedGameObject(currentSelection);
    }

    /// <summary>Stores previous selection for ease of backing out of submenus.</summary>
    public static void SetPreviousSelection()
    {
        if (EventSystem.current.currentSelectedGameObject)
        {
            previousSelection = EventSystem.current.currentSelectedGameObject;
        }
    }

    /// <summary>Sets Selected GameObject to be the previous selection.</summary>
    public static void SelectPrevious()
    {
        if (previousSelection)
        {
            EventSystem.current.SetSelectedGameObject(previousSelection);       
        }
    }

    /// <summary>Sets Selected GameObject to be the previous selection.</summary>
    public static void SelectPreviousFromButton(GameObject menu)
    {
        if (previousSelection)
        {
            EventSystem.current.SetSelectedGameObject(previousSelection);
            currentSelection = menu.GetComponent<IBaseUI>().startSelection;
        }
    }

    /// <summary>Resets Selected GameObject to null.</summary>
    public static void ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
