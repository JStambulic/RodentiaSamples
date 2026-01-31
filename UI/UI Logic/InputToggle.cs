using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script to provide more readability to a toggle.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class InputToggle : MonoBehaviour
{
    Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(delegate { UpdateText(); });

            UpdateText();
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveAllListeners();
    }

    /// <summary>
    /// Updates the associated enabled text to be opaque or transparent.
    /// </summary>
    void UpdateText()
    {
        if (toggle != null)
        {
            var text = GetComponent<HoverSelection>().AssociatedText;

            if (toggle.isOn)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
            }
            else
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.25f);
            }
        }
    }
}
