using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script for focusing dropdown menus, giving the ability to back out.
/// </summary>
[RequireComponent(typeof(TMP_Dropdown))]
public class InputDropdown : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    OptionsMenu optionsMenuRef;

    // Start is called before the first frame update
    void Start()
    {
        optionsMenuRef = GameManager.Get().GetUIManager().OptionsUI_Instance;
    }

    /// <summary>
    /// On a mouse click, lock into the dropdown.
    /// </summary>
    /// <param name="eventData">Data from mouse click.</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        LockIntoDropdown();
    }

    /// <summary>
    /// On a submit (gamepad/keyboard), lock into the dropdown.
    /// </summary>
    /// <param name="eventData">Data from submit input.</param>
    public void OnSubmit(BaseEventData eventData)
    {
        LockIntoDropdown();
    }

    /// <summary>
    /// Tells Options Menu that you are navigated into a dropdown menu.
    /// </summary>
    void LockIntoDropdown()
    {
        if (!optionsMenuRef.IsInDropdown)
        {
            optionsMenuRef.EnterDropdownMenu(gameObject);
        }
    }
}
