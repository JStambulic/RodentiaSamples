using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// Script for running events when a UI element becomes selected.
/// </summary>
public class OnSelectEvent : MonoBehaviour, ISelectHandler
{
    public UnityEvent onOptionSelect = new UnityEvent();

    /// <summary>
    /// When selected/hovered, run event(s).
    /// </summary>
    public void OnSelect(BaseEventData eventData)
    {
        onOptionSelect.Invoke();
    }
}
