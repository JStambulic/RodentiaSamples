using TMPro;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants.TrackedObjects;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;
using UnityEngine.Localization.PropertyVariants;
using UnityEngine.Localization.Settings;

// Code sourced from Unity.
// https://docs.unity3d.com/Packages/com.unity.localization@1.0/api/UnityEngine.Localization.PropertyVariants.GameObjectLocalizer.html

/// <summary>
/// Allows all elements within a TMP dropdown to be localizable.
/// </summary>
[RequireComponent(typeof(TMP_Dropdown))]
public class TMPDropdownLocalization : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    public string localizationTable;
    public string locale;

    void Start()
    {
        if (dropdown == null)
        {
            GetComponent<TMP_Dropdown>();
        }

        if (dropdown == null) { return; }

        var localizer = gameObject.AddComponent<GameObjectLocalizer>();

        // Gets the Tracked text or creates a new tracker
        var trackedDropdown = localizer.GetTrackedObject<TrackedTmpDropdown>(dropdown);

        // Setup each option
        for (int i = 0; i < dropdown.options.Count; ++i)
        {
            var optionText = trackedDropdown.GetTrackedProperty<LocalizedStringProperty>($"m_Options.m_Options.Array.data[{i}].m_Text");
            optionText.LocalizedString.SetReference(localizationTable, locale + i);
        }

        // Force an Update
        localizer.ApplyLocaleVariant(LocalizationSettings.SelectedLocale);
    }
}
