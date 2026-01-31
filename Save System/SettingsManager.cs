using Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.Localization.Settings;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

/// <summary>
/// Manages all settings from the Options menu that must persist across game sessions.
/// </summary>
public class SettingsManager
{
    #region Member Variables

    private static HDRenderPipelineAsset globalRenderSettings;

    private static VolumeProfile globalVolumeProfile;
    private static ColorAdjustments colorAdjustments;
    private static ChannelMixer channelMixer;

    private static AudioMixer gameMixer;

    private static InputActionAsset inputActions;

    public static UnityEvent onCameraUpdated = new UnityEvent();

    public static LanguageMode CurrentLanguageMode = LanguageMode.English;

    public static bool TutorialsEnabled = true;
    public static bool TailAssistMode = false;
    public static bool ShowDamageIndicator = true;

    public static float RenderValue = 100;

    #endregion

    #region Resource Loaders

    /// <summary>
    /// Loads the main VolumeProfile from Resources.
    /// </summary>
    /// <returns>The loaded volume profile.</returns>
    public static VolumeProfile GetGameVolumeProfile()
    {
        if (globalVolumeProfile == null)
        {
            globalVolumeProfile = Resources.Load<VolumeProfile>("GameProfile");
            globalVolumeProfile.TryGet(out colorAdjustments);
            globalVolumeProfile.TryGet(out channelMixer);
        }

        return globalVolumeProfile;
    }

    /// <summary>
    /// Returns the global render pipeline asset.
    /// </summary>
    /// <returns>The loaded pipeline asset.</returns>
    public static HDRenderPipelineAsset GetRenderSettingsAsset()
    {
        if (globalRenderSettings == null)
        {
            globalRenderSettings = (HDRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        }

        return globalRenderSettings;
    }

    /// <summary>
    /// Loads the AudioMixer from Resources.
    /// </summary>
    /// <returns>The loaded master audio mixer.</returns>
    public static AudioMixer GetGameMixer()
    {
        if (gameMixer == null)
        {
            gameMixer = Resources.Load<AudioMixer>("Mixers/Master");
        }

        return gameMixer;
    }

    /// <summary>
    /// Loads the InputActionAsset from Resources.
    /// </summary>
    /// <returns>The loaded input action asset.</returns>
    public static InputActionAsset GetInputActions()
    {
        if (inputActions == null)
        {
            inputActions = Resources.Load<InputActionAsset>("Inputs/PlayerInputs");
        }

        return inputActions;
    }

    #endregion

    #region Load & Set Options

    /// <summary>
    /// Applies all game settings that need to be carried across scenes/game sessions.
    /// </summary>
    public static void ApplyGameSettings()
    {
        RenderValue = PlayerPrefs.GetFloat(Options.renderScaleName, 100);
        DynamicResolutionHandler.SetDynamicResScaler(ChangeResScale, DynamicResScalePolicyType.ReturnsMinMaxLerpFactor);

        SaveManager.LoadCollectibles();
        SaveManager.LoadTutorialCodex();
        SetGameplay();
        SetVideo();
        SetAudio();
        SetCameraControls();
        LoadRebinds();
    }

    /// <summary>
    /// Sets all Gameplay Options from PlayerPrefs data at runtime.
    /// </summary>
    static void SetGameplay()
    {
        if (inputActions == null)
        {
            GetInputActions();
        }

        CurrentLanguageMode = (LanguageMode)PlayerPrefs.GetInt(Options.languageName, 0);
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[(int)CurrentLanguageMode];
        //Debug.Log((LanguageMode)PlayerPrefs.GetInt(Options.languageName, 0));

        TutorialsEnabled = PlayerPrefs.GetInt(Options.tutorialsName, 1) > 0 ? true : false;

        TailAssistMode = PlayerPrefs.GetInt(Options.tailAssistName, 0) > 0 ? true : false;
        ShowDamageIndicator = PlayerPrefs.GetInt(Options.damageValuesName, 1) > 0 ? true : false;

        if (inputActions)
        {
            InputAction lookInput = inputActions.FindActionMap("Player").FindAction("Look");
            InputAction moveInput = inputActions.FindActionMap("Player").FindAction("Move");

            moveInput.ApplyParameterOverride((StickDeadzoneProcessor d) => d.min, PlayerPrefs.GetFloat(Options.leftDeadzoneName, 0.12f));
            lookInput.ApplyParameterOverride((StickDeadzoneProcessor d) => d.min, PlayerPrefs.GetFloat(Options.rightDeadzoneName, 0.12f));
        }
    }

    /// <summary>
    /// Sets all Video Options from PlayerPrefs data at runtime.
    /// Does not set resolution, fullscreen mode, vsync, or fps, as those are dealt with by Unity by default.
    /// </summary>
    static void SetVideo()
    {
        if (globalVolumeProfile == null)
        {
            GetGameVolumeProfile();
        }
        if (globalRenderSettings == null)
        {
            GetRenderSettingsAsset();
        }

        if (globalVolumeProfile != null)
        {
            if (colorAdjustments != null)
            {
                colorAdjustments.postExposure.value = PlayerPrefs.GetFloat(Options.brightnessName, 0.0f);
                colorAdjustments.saturation.value = PlayerPrefs.GetInt(Options.saturationName, 0);
                colorAdjustments.contrast.value = PlayerPrefs.GetInt(Options.contrastName, 0);
            }
            if (channelMixer != null)
            {
                int colorMode = PlayerPrefs.GetInt(Options.colorBlindnessName, 0);

                switch (colorMode)
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
            }
        }
    }

    static float ChangeResScale()
    {
        return (RenderValue / 100.0f);
    }

    /// <summary>
    /// Sets all Audio Options from PlayerPrefs data at runtime.
    /// </summary>
    static void SetAudio()
    {
        // Find the Master Audio Mixer.
        if (gameMixer == null)
        {
            GetGameMixer();
        }

        if (gameMixer != null)
        {
            float volumeVal = PlayerPrefs.GetFloat(Options.masName, 1.0f);

            gameMixer.SetFloat("volumeMaster", VolumeHelper.LogarithmicVolume(volumeVal));

            volumeVal = PlayerPrefs.GetFloat(Options.sfxName, 1.0f);
            gameMixer.SetFloat("volumeSFX", VolumeHelper.LogarithmicVolume(volumeVal));

            volumeVal = PlayerPrefs.GetFloat(Options.diaName, 1.0f);
            gameMixer.SetFloat("volumeDIA", VolumeHelper.LogarithmicVolume(volumeVal));

            volumeVal = PlayerPrefs.GetFloat(Options.musName, 1.0f);
            gameMixer.SetFloat("volumeMUS", VolumeHelper.LogarithmicVolume(volumeVal));
        }
    }

    /// <summary>
    /// Sets all Camera Control Options from PlayerPrefs data at runtime.
    /// </summary>
    static void SetCameraControls()
    {
        if (inputActions == null)
        {
            GetInputActions();
        }

        if (inputActions)
        {
            InputAction lookInput = inputActions.FindActionMap("Player").FindAction("Look");

            lookInput.ApplyParameterOverride((InvertVector2Processor p) => p.invertX, PlayerPrefs.GetInt(Options.invertXName, 0) > 0 ? true : false);
            lookInput.ApplyParameterOverride((InvertVector2Processor p) => p.invertY, PlayerPrefs.GetInt(Options.invertYName, 0) > 0 ? true : false);

            lookInput.ApplyParameterOverride((ScaleVector2Processor p) => p.x, PlayerPrefs.GetFloat(Options.sensitivityKeyboardName, 1.0f));
            lookInput.ApplyParameterOverride((ScaleVector2Processor p) => p.y, PlayerPrefs.GetFloat(Options.sensitivityKeyboardName, 1.0f));
        }
    }

    /// <summary>
    /// Loads the control rebind overrides from file.
    /// </summary>
    static void LoadRebinds()
    {
        if (inputActions == null)
        {
            GetInputActions();
        }

        if (inputActions)
        {
            var rebinds = PlayerPrefs.GetString("rebinds");
            if (!string.IsNullOrEmpty(rebinds))
                inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    #endregion
}

public enum LanguageMode
{
    English,
    French,
    Spanish,
}

/// <summary>
/// Contains all const strings for PlayerPrefs save data.
/// </summary>
public struct Options
{
    // Gameplay
    public const string languageName = "currentLanguage";
    public const string colorBlindnessName = "colorBlindnessMode";
    public const string leftDeadzoneName = "leftDeadzone";
    public const string rightDeadzoneName = "rightDeadzone";
    public const string tutorialsName = "tutorialsEnabledSetting";
    public const string tailAssistName = "tailAssistSetting";
    public const string damageValuesName = "damageValuesSetting";
    public const string hudModeName = "hudSetting";

    // Video
    public const string resolutionName = "resOption";
    public const string fullscreenModeName = "screenModeOption";
    public const string qualityName = "qualityOption";

    public const string renderScaleName = "renderScaleOption";
    //public const string hdrName = "HDROption";
    public const string msaaName = "msaaOption";
    public const string antiAliasingName = "antiAliasingOption";

    public const string framerateName = "framerateOption";
    public const string vsyncName = "vSyncOption";

    public const string brightnessName = "brightnessOption";
    public const string saturationName = "saturationOption";
    public const string contrastName = "contrastOption";

    // Audio
    public const string masName = "masterVolume";
    public const string sfxName = "sfxVolume";
    public const string diaName = "diaVolume";
    public const string musName = "musVolume";

    // Controls
    public const string invertXName = "xInvert";
    public const string invertYName = "yInvert";
    public const string sensitivityGamepadName = "gamepadSensitivity";
    public const string sensitivityKeyboardName = "keyboardSensitivity";
}
