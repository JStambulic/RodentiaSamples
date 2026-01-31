using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Processors;
using TMPro;
using System;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering.HighDefinition;

public class OptionsGameplay : MonoBehaviour
{
    #region Member Variables

    [SerializeField] TMP_Dropdown languageDropdown;

    [SerializeField] TMP_Dropdown colorblindnessDropdown;
    ChannelMixer channelMixer;

    [SerializeField] Slider leftDeadzoneSlider;
    [SerializeField] Slider rightDeadzoneSlider;

    [SerializeField] Toggle tutorialsEnabledToggle;
    [SerializeField] Toggle tailAssistToggle;
    [SerializeField] Toggle damageValuesToggle;
    [SerializeField] TMP_Dropdown hudTypeDropdown;

    private InputAction moveInput;
    private InputAction lookInput;

    #endregion

    #region Start, OnEnable, Initialize

    void Start()
    {
        SetOptionsValues();
    }

    /// <summary>
    /// Updates the options for when submenu is created/enabled.
    /// </summary>
    void SetOptionsValues()
    {
        languageDropdown.value = PlayerPrefs.GetInt(Options.languageName, 0);

        colorblindnessDropdown.value = PlayerPrefs.GetInt(Options.colorBlindnessName, 0);
        colorblindnessDropdown.RefreshShownValue();

        leftDeadzoneSlider.value = PlayerPrefs.GetFloat(Options.leftDeadzoneName, 0.12f);
        rightDeadzoneSlider.value = PlayerPrefs.GetFloat(Options.rightDeadzoneName, 0.12f);

        tutorialsEnabledToggle.isOn = PlayerPrefs.GetInt(Options.tutorialsName, 1) > 0 ? true : false;
        tailAssistToggle.isOn = PlayerPrefs.GetInt(Options.tailAssistName, 0) > 0 ? true : false;
        damageValuesToggle.isOn = PlayerPrefs.GetInt(Options.damageValuesName, 1) > 0 ? true : false;
    }

    /// <summary>
    /// Initializes the menu with the look InputAction.
    /// </summary>
    public void Initialize()
    {
        if (lookInput == null)
        {
            InputActionAsset inputActions = SettingsManager.GetInputActions();

            moveInput = inputActions.FindActionMap("Player").FindAction("Move");
            lookInput = inputActions.FindActionMap("Player").FindAction("Look");
        }

        var globalVolumeProfile = SettingsManager.GetGameVolumeProfile();
        if (globalVolumeProfile != null)
        {
            globalVolumeProfile.TryGet(out channelMixer);
        }
    }

    #endregion

    #region Language

    public void ChangeCurrentLanguage(Int32 setting)
    {
        SettingsManager.CurrentLanguageMode = (LanguageMode)setting;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[setting];

        PlayerPrefs.SetInt(Options.languageName, setting);
        PlayerPrefs.Save();
    }

    #endregion

    #region Color Blindness

    /// <summary>
    /// Sets colorblindness mode.
    /// </summary>
    /// <param name="state">Dropdown state.</param>
    public void SetColorblindnessMode(int state)
    {
        switch (state)
        {
            // Protanopia
            case 1:
                channelMixer.redOutRedIn.Override(56.667f);
                channelMixer.redOutGreenIn.Override(43.33f);
                channelMixer.redOutBlueIn.Override(0.0f);

                channelMixer.greenOutRedIn.Override(55.833f);
                channelMixer.greenOutGreenIn.Override(44.167f);
                channelMixer.greenOutBlueIn.Override(0.0f);

                channelMixer.blueOutRedIn.Override(0.0f);
                channelMixer.blueOutGreenIn.Override(24.167f);
                channelMixer.blueOutBlueIn.Override(75.833f);
                break;

            // Deuteranopia
            case 2:
                channelMixer.redOutRedIn.Override(62.5f);
                channelMixer.redOutGreenIn.Override(37.5f);
                channelMixer.redOutBlueIn.Override(0.0f);

                channelMixer.greenOutRedIn.Override(70.0f);
                channelMixer.greenOutGreenIn.Override(30.0f);
                channelMixer.greenOutBlueIn.Override(0.0f);

                channelMixer.blueOutRedIn.Override(0.0f);
                channelMixer.blueOutGreenIn.Override(30.0f);
                channelMixer.blueOutBlueIn.Override(70.0f);
                break;

            // Tritanopia
            case 3:
                channelMixer.redOutRedIn.Override(95.0f);
                channelMixer.redOutGreenIn.Override(5.0f);
                channelMixer.redOutBlueIn.Override(0.0f);

                channelMixer.greenOutRedIn.Override(0.0f);
                channelMixer.greenOutGreenIn.Override(43.33f);
                channelMixer.greenOutBlueIn.Override(56.667f);

                channelMixer.blueOutRedIn.Override(0.0f);
                channelMixer.blueOutGreenIn.Override(47.5f);
                channelMixer.blueOutBlueIn.Override(52.5f);
                break;

            // Achromatopsia
            case 4:
                channelMixer.redOutRedIn.Override(29.9f);
                channelMixer.redOutGreenIn.Override(58.7f);
                channelMixer.redOutBlueIn.Override(11.4f);

                channelMixer.greenOutRedIn.Override(29.9f);
                channelMixer.greenOutGreenIn.Override(58.7f);
                channelMixer.greenOutBlueIn.Override(11.4f);

                channelMixer.blueOutRedIn.Override(29.9f);
                channelMixer.blueOutGreenIn.Override(58.7f);
                channelMixer.blueOutBlueIn.Override(11.4f);
                break;

            // Off
            case 0:
            default:
                channelMixer.redOutRedIn.Override(100.0f);
                channelMixer.redOutGreenIn.Override(0.0f);
                channelMixer.redOutBlueIn.Override(0.0f);

                channelMixer.greenOutRedIn.Override(0.0f);
                channelMixer.greenOutGreenIn.Override(100.0f);
                channelMixer.greenOutBlueIn.Override(0.0f);

                channelMixer.blueOutRedIn.Override(0.0f);
                channelMixer.blueOutGreenIn.Override(0.0f);
                channelMixer.blueOutBlueIn.Override(100.0f);
                break;
        }

        PlayerPrefs.SetInt(Options.colorBlindnessName, state);
        PlayerPrefs.Save();
    }

    #endregion

    #region Stick Deadzones

    /// <summary>
    /// Increases the minimum deadzone on the left analog stick.
    /// </summary>
    public void ChangeLeftDeadzone(float deadzone)
    {
        if (moveInput != null)
        {
            moveInput.ApplyParameterOverride((StickDeadzoneProcessor d) => d.min, deadzone);

            PlayerPrefs.SetFloat(Options.leftDeadzoneName, deadzone);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Increases the minimum deadzone on the right analog stick.
    /// </summary>
    public void ChangeRightDeadzone(float deadzone)
    {
        if (lookInput != null)
        {
            lookInput.ApplyParameterOverride((StickDeadzoneProcessor d) => d.min, deadzone);

            PlayerPrefs.SetFloat(Options.rightDeadzoneName, deadzone);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Ease of Use Settings

    /// <summary>
    /// Sets whether tutorials should be shown to the player or not.
    /// </summary>
    /// <param name="state"> Desired state. </param>
    public void SetTutorialsEnabled(bool state)
    {
        SettingsManager.TutorialsEnabled = state;

        PlayerPrefs.SetInt(Options.tutorialsName, state ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the state of whether to use the tail assist mode or not. On by default.
    /// </summary>
    /// <param name="state"> Desired State </param>
    public void SetTailAssist(bool state)
    {
        SettingsManager.TailAssistMode = state;

        if (GameManager.Player() != null)
        {
            GameManager.Player().GetComponent<TailComponent>().SetTailAssistMode(state);
        }

        PlayerPrefs.SetInt(Options.tailAssistName, state ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the state of whether to show the damage number indicator or not. On by default
    /// </summary>
    /// <param name="state"> Desired state. </param>
    public void SetShowDamageIndicators(bool state)
    {
        SettingsManager.ShowDamageIndicator = state;

        PlayerPrefs.SetInt(Options.damageValuesName, state ? 1 : 0);
        PlayerPrefs.Save();
    }
    #endregion
}
