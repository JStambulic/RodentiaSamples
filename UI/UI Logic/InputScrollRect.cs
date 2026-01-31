using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Allows for ScrollRect to be navigated via Keyboard or Gamepad input.
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class InputScrollRect : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] RectTransform viewport;
    [SerializeField] RectTransform content;

    RectTransform selectedTransform;

    void Update()
    {
        // Check for scroll while the script is active.
        UpdateScrollToSelected();
    }

    /// <summary>
    /// Shifts the vertical position of the scroll rect based on which object is selected.
    /// </summary>
    void UpdateScrollToSelected()
    {
        // Get the currently selected option from the eventsystem.
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null)
        {
            return;
        }

        // If not a child of content, return.
        if (!selected.transform.IsChildOf(content.transform))
        {
            return;
        }

        selectedTransform = selected.GetComponent<RectTransform>();
        Rect viewRect = viewport.rect;

        // Transform selected rect from its local space into Content space.
        Rect selectedRectWorld = selectedTransform.rect.Transform(selectedTransform);
        Rect selectedRectViewport = selectedRectWorld.InverseTransform(viewport);

        // Calculate if we're outside the viewport either on top or on bottom.
        float outsideOnTop = selectedRectViewport.yMax - viewRect.yMax;
        float outsideOnBottom = viewRect.yMin - selectedRectViewport.yMin;

        // Positive is outside the viewport.
        // Negative is inside the viewport; zero out for simpler math.
        if (outsideOnTop < 0.0f)
        {
            outsideOnTop = 0.0f;
        }
        if (outsideOnBottom < 0.0f)
        {
            outsideOnBottom = 0.0f;
        }

        // Direction to scroll in, up or down.
        float delta = outsideOnTop > 0.0f ? outsideOnTop : -outsideOnBottom;
        // Return if no scroll direction is detected.
        if (delta == 0.0f)
        { 
            return;
        }

        // Transform Content into the viewport space.
        Rect contentRectWorld = content.rect.Transform(content);
        Rect contentRectViewport = contentRectWorld.InverseTransform(viewport);

        // Calculate how much of the content extends past the viewport.
        float overflow = contentRectViewport.height - viewRect.height;

        // How many units the normalized scroll will move.
        float unitsToNormalized = 1 / overflow;

        scrollRect.verticalNormalizedPosition += delta * unitsToNormalized;
    }
}

/// <summary>
/// Helper class for converting rects from local space into world space and vice versa.
/// </summary>
internal static class RectExtensions
{
    /// <summary>
    /// Transforms a rect from the transform local space to world space.
    /// </summary>
    public static Rect Transform(this Rect r, Transform transform)
    {
        return new Rect
        {
            min = transform.TransformPoint(r.min),
            max = transform.TransformPoint(r.max),
        };
    }

    /// <summary>
    /// Transforms a rect from world space to the transform local space.
    /// </summary>
    public static Rect InverseTransform(this Rect r, Transform transform)
    {
        return new Rect
        {
            min = transform.InverseTransformPoint(r.min),
            max = transform.InverseTransformPoint(r.max),
        };
    }
}