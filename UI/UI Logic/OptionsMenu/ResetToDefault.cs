using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Script for resetting a selectable to a default initial value.
/// </summary>
public class ResetToDefault : MonoBehaviour
{
    #region Member Variables

    public enum OptionType
    {
        Toggle,
        Slider,
        Dropdown,
    }
    OptionType typeOfOption;

    Toggle toggleComponent;
    Slider sliderComponent;
    TMP_Dropdown dropdownComponent;

    float defaultValue;

    GameObject resetButton;

    InputAction resetAction;

    #endregion

    #region Awake, Start, etc

    private void Awake()
    {
        resetButton = transform.Find("Reset").gameObject;
        if (resetButton == null)
        {
            Debug.LogWarning("Reset Visual not found in Selectable UI!");
        }

        toggleComponent = GetComponent<Toggle>();
        if (toggleComponent != null) { typeOfOption = OptionType.Toggle; }

        sliderComponent = GetComponent<Slider>();
        if (sliderComponent != null) { typeOfOption = OptionType.Slider; }

        dropdownComponent = GetComponent<TMP_Dropdown>();
        if (dropdownComponent != null) { typeOfOption = OptionType.Dropdown; }

        switch (typeOfOption)
        {
            case OptionType.Toggle:
                defaultValue = toggleComponent.isOn ? 1 : 0;
                toggleComponent.onValueChanged.AddListener(delegate { OnSelectableChanged(); });
                break;

            case OptionType.Slider:
                defaultValue = sliderComponent.value;
                sliderComponent.onValueChanged.AddListener(delegate { OnSelectableChanged(); });
                break;

            case OptionType.Dropdown:
                defaultValue = dropdownComponent.value;
                dropdownComponent.onValueChanged.AddListener(delegate { OnSelectableChanged(); });
                break;

            default: break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        resetAction = GameManager.Get().PlayerInputCachedRef.actions.FindAction("ResetToDefault");
        if (resetAction != null)
        {
            resetAction.started += ResetValueToDefault;
        }
    }

    private void OnDestroy()
    {
        if (resetAction != null)
        {
            resetAction.started -= ResetValueToDefault;
        }

        switch (typeOfOption)
        {
            case OptionType.Toggle:
                toggleComponent.onValueChanged.RemoveAllListeners();
                break;

            case OptionType.Slider:
                sliderComponent.onValueChanged.RemoveAllListeners();
                break;

            case OptionType.Dropdown:
                dropdownComponent.onValueChanged.RemoveAllListeners();
                break;

            default: break;
        }
    }

    #endregion

    #region Actions

    /// <summary>
    /// Allows outside scripts to update the default value.
    /// </summary>
    /// <param name="newValue">Value to set as new default.</param>
    public void SetDefaultValue(float newValue)
    {
        defaultValue = newValue;
    }

    /// <summary>
    /// When the value of this selectable changes, update to show the reset icon.
    /// </summary>
    public void OnSelectableChanged()
    {
        switch (typeOfOption)
        {
            case OptionType.Toggle:

                int compare = toggleComponent.isOn ? 1 : 0;
                if (compare != (int)defaultValue)
                {
                    resetButton.SetActive(true);
                }
                else if (compare == (int)defaultValue)
                {
                    resetButton.SetActive(false);
                }
                break;

            case OptionType.Slider:

                if (sliderComponent.value != defaultValue)
                {
                    resetButton.SetActive(true);
                }
                else if (sliderComponent.value == defaultValue)
                {
                    resetButton.SetActive(false);
                }
                break;

            case OptionType.Dropdown:

                if (dropdownComponent.value != (int)defaultValue)
                {
                    resetButton.SetActive(true);
                }
                else if (dropdownComponent.value == (int)defaultValue)
                {
                    resetButton.SetActive(false);
                }
                break;

            default: break;
        }
    }

    /// <summary>
    /// When reset is pressed and this is the currently selected object, reset value if changed.
    /// </summary>
    /// <param name="context">Input context.</param>
    void ResetValueToDefault(InputAction.CallbackContext context)
    {
        if (EventSystem.current.currentSelectedGameObject != gameObject)
        {
            return;
        }

        if (context.started)
        {
            switch (typeOfOption)
            {
                case OptionType.Toggle:

                    int compare = toggleComponent.isOn ? 1 : 0;
                    if (compare != (int)defaultValue)
                    {
                        toggleComponent.isOn = defaultValue > 0 ? true : false;

                        resetButton.SetActive(false);
                    }
                    break;

                case OptionType.Slider:

                    if (sliderComponent.value != defaultValue)
                    {
                        sliderComponent.value = defaultValue;

                        resetButton.SetActive(false);
                    }
                    break;

                case OptionType.Dropdown:

                    if (dropdownComponent.value != (int)defaultValue)
                    {
                        dropdownComponent.value = (int)defaultValue;

                        resetButton.SetActive(false);
                    }
                    break;

                default: break;
            }
        }
    }

    #endregion
}
