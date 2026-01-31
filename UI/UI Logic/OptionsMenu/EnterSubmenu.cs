using UnityEngine;
using UnityEngine.EventSystems;

public class EnterSubmenu : MonoBehaviour, IPointerClickHandler
{
    public OptionsSelection optionsSelectionRef;
    public ControlsType controlsTypeRef;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (optionsSelectionRef != null)
        {
            optionsSelectionRef.SelectOption();
        }
        else if (controlsTypeRef != null)
        {
            controlsTypeRef.SelectOption();
        }
    }
}
