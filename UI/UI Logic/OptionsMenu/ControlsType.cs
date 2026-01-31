using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Changes the submenu Controls shows based on current control scheme.
/// </summary>
public class ControlsType : MonoBehaviour, ISelectHandler
{
    #region Member Variables

    [Header("Options Menu")]
    [SerializeField] OptionsMenu optionsMenuRef;
    [SerializeField] EnterSubmenu submenuCover;

    [Header("Control Menus")]
    [SerializeField] GameObject controlsKeyboard;
    [SerializeField] GameObject startSelectionKB;

    [SerializeField] GameObject controlsGamepad;
    [SerializeField] GameObject startSelectionGP;

    GameObject menuToDisplay;
    private PlayerInput playerInput;


    #endregion

    #region ControlsChanged Event

    void OnEnable()
    {
        // Get current Player Input.
        GameManager gm = GameManager.Get();
        playerInput = gm.GetUIManager().IsActive(UIType.Main) ? GameObject.Find("InputManager").GetComponent<PlayerInput>() : gm.GetPlayer().GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            // Subscribe to controlsChangedEvent.
            playerInput.controlsChangedEvent.AddListener(OnControlsChanged);

            // Set the initial submenu to display based on currentControlScheme.
            menuToDisplay = (playerInput.currentControlScheme == "Keyboard") ? controlsKeyboard : controlsGamepad;
        }
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            // Unsubscribe from controlsChangedEvent.
            playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            // Unsubscribe from controlsChangedEvent.
            playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }

    /// <summary>
    /// Listens for Controls Changed Event from Player Input to change the version of Controls submenu.
    /// </summary>
    private void OnControlsChanged(PlayerInput input)
    {
        if (optionsMenuRef.ActiveSubmenu == controlsKeyboard || optionsMenuRef.ActiveSubmenu == controlsGamepad)
        {
            if (input.currentControlScheme == "Keyboard")
            {
                if (optionsMenuRef.ActiveSubmenu == controlsKeyboard)
                {
                    return;
                }

                //Debug.Log("Keyboard Scheme Switch.");
                menuToDisplay = controlsKeyboard;

                // Reset Scroll to top.
                if (optionsMenuRef.ActiveSubmenu == controlsGamepad)
                {
                    menuToDisplay.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
                }

                optionsMenuRef.SwitchDisplayedSubmenu(menuToDisplay);

                if (optionsMenuRef.IsInSubmenu)
                {
                    if (optionsMenuRef.ActiveSubmenu == controlsKeyboard)
                    {
                        EventSystem.current.SetSelectedGameObject(startSelectionKB);
                    }
                }
            }
            else
            {
                if (optionsMenuRef.ActiveSubmenu == controlsGamepad)
                {
                    return;
                }

                //Debug.Log("Gamepad Scheme Switch.");
                menuToDisplay = controlsGamepad;

                // Reset Scroll to top.
                if (optionsMenuRef.ActiveSubmenu == controlsKeyboard)
                {
                    menuToDisplay.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
                }

                optionsMenuRef.SwitchDisplayedSubmenu(menuToDisplay);

                if (optionsMenuRef.IsInSubmenu)
                {
                    if (optionsMenuRef.ActiveSubmenu == controlsGamepad)
                    {
                        EventSystem.current.SetSelectedGameObject(startSelectionGP);
                    }
                }
            }
        }
    }

    #endregion

    #region Selection Logic

    /// <summary>
    /// Sets the current selected GameObject to be the start of the new submenu.
    /// </summary>
    public void SelectOption()
    {
        submenuCover?.gameObject.SetActive(false);
        submenuCover.controlsTypeRef = null;

        optionsMenuRef.EnterSubmenu();

        if (menuToDisplay == controlsKeyboard)
        {
            EventSystem.current.SetSelectedGameObject(startSelectionKB);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(startSelectionGP);
        }
    }

    /// <summary>
    /// When selected/hovered, this menu becomes active.
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("OnSelect");
        if (menuToDisplay)
        {
            if (optionsMenuRef.IsInSubmenu)
            {
                optionsMenuRef.ExitSubmenuByHover();
            }

            submenuCover?.gameObject.SetActive(true);
            submenuCover.controlsTypeRef = this;

            optionsMenuRef.SwitchDisplayedSubmenu(menuToDisplay);

            // Reset Scroll to top.
            menuToDisplay.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
        }
    }

    #endregion
}
