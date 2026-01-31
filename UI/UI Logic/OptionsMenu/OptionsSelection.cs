using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>Logic for accessing/showing the submenus within the Options menu.</summary>
public class OptionsSelection : MonoBehaviour, ISelectHandler
{
    [SerializeField] OptionsMenu optionsMenuRef;
    public OptionsMenu OptionsMenuRef => optionsMenuRef;

    [SerializeField] GameObject menuToDisplay;
    [SerializeField] EnterSubmenu submenuCover;

    [SerializeField] GameObject startSelection;

    /// <summary>Sets the current selected GameObject to be the start of the new submenu.</summary>
    public void SelectOption()
    {
        submenuCover.optionsSelectionRef = null;
        submenuCover.gameObject.SetActive(false);

        optionsMenuRef.EnterSubmenu();
        EventSystem.current.SetSelectedGameObject(startSelection);
    }

    /// <summary>When selected/hovered, this menu becomes active.</summary>
    public void OnSelect(BaseEventData eventData)
    {
        //Debug.Log("OnSelect");
        if (menuToDisplay)
        {
            submenuCover.optionsSelectionRef = this;

            if (optionsMenuRef.IsInSubmenu)
            {
                optionsMenuRef.ExitSubmenuByHover();
            }

            submenuCover.gameObject.SetActive(true);
            optionsMenuRef.SwitchDisplayedSubmenu(menuToDisplay);
        }
    }
}
