using UnityEngine;
using UnityEngine.EventSystems;

public class DropdownOption : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Get().GetUIManager().OptionsUI_Instance.IsInDropdown)
        {
            GameManager.Get().GetUIManager().OptionsUI_Instance.ExitDropdownMenu();
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (GameManager.Get().GetUIManager().OptionsUI_Instance.IsInDropdown)
        {
            GameManager.Get().GetUIManager().OptionsUI_Instance.ExitDropdownMenu();
        }
    }
}
