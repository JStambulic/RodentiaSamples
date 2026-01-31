using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

/// <summary>
/// Holds all Video related settings for the Options.
/// </summary>
public class OptionsVideo : MonoBehaviour
{
    #region Member Variables

    private HDRenderPipelineAsset globalRenderSettings;
    private VolumeProfile globalVolumeProfile;
    private ColorAdjustments colorAdjustments;

    // Resolution
    [Header("Video")]
    [SerializeField] TMP_Dropdown resolutionDropdown;
    List<Resolution> resolutions;

    // Fullscreen Mode
    [SerializeField] TMP_Dropdown fullscreenDropdown;

    // Quality
    [Header("Quality")]
    [SerializeField] TMP_Dropdown qualityDropdown;
    [SerializeField] Slider renderScaleSlider;

    // Render Settings
    [Header("Rendering")]
    [SerializeField] TMP_Dropdown msaaDropdown;
    [SerializeField] TMP_Dropdown antiAliasingDropdown;

    // FPS & Vsync
    [Header("FPS")]
    [SerializeField] Slider fpsSlider;
    [SerializeField] TMP_Text[] fpsText;
    [SerializeField] Toggle vSyncToggle;

    // Screen Display Settings
    [Header("Visuals")]
    [SerializeField] Slider brightnessSlider;
    [SerializeField] Slider saturationSlider;
    [SerializeField] Slider contrastSlider;

    #endregion

    #region Start

    void Start()
    {
        // Set available Display Resolutions in dropdown menu.
        SetDisplayResolutions();

        fpsSlider.value = PlayerPrefs.GetInt(Options.framerateName, 60);
        vSyncToggle.isOn = PlayerPrefs.GetInt(Options.vsyncName, QualitySettings.vSyncCount) > 0 ? true : false;
        if (vSyncToggle.isOn == true)
        {
            fpsSlider.interactable = false;
            foreach (var text in fpsText)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.2f);
            }
        }

        fullscreenDropdown.value = PlayerPrefs.GetInt(Options.fullscreenModeName, (int)Screen.fullScreenMode);
        fullscreenDropdown.RefreshShownValue();

        qualityDropdown.value = PlayerPrefs.GetInt(Options.qualityName, QualitySettings.GetQualityLevel());
        qualityDropdown.RefreshShownValue();

        renderScaleSlider.value = PlayerPrefs.GetFloat(Options.renderScaleName, 100);

        msaaDropdown.value = PlayerPrefs.GetInt(Options.msaaName, 0);
        msaaDropdown.RefreshShownValue();
        antiAliasingDropdown.value = PlayerPrefs.GetInt(Options.antiAliasingName, 0);
        antiAliasingDropdown.RefreshShownValue();

        brightnessSlider.value = (PlayerPrefs.GetFloat(Options.brightnessName, 0) * 25.0f) + 50;
        saturationSlider.value = PlayerPrefs.GetInt(Options.saturationName, 0);
        contrastSlider.value = PlayerPrefs.GetInt(Options.contrastName, 0);
    }

    /// <summary>
    /// Initializes the menu with globalVolumeProfile.
    /// </summary>
    public void Initialize()
    {
        if (globalVolumeProfile == null)
        {
            globalVolumeProfile = SettingsManager.GetGameVolumeProfile();

            if (globalVolumeProfile != null)
            {
                globalVolumeProfile.TryGet(out colorAdjustments);
            }
        }

        if (globalRenderSettings == null)
        {
            globalRenderSettings = (HDRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        }
    }

    #endregion

    #region Screen Settings

    /// <summary>
    /// Sets which resolutions are available to choose from in the Resolution dropdown.
    /// </summary>
    void SetDisplayResolutions()
    {
        //var resNoHertz = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height}).Distinct();
        //resolutions = resNoHertz.ToArray();

        resolutions = Screen.resolutions.ToList().Distinct().ToList();

        resolutionDropdown.ClearOptions();

        for(int i = resolutions.Count - 1; i >= 0; i--)
        {
            if ((float)resolutions[i].refreshRateRatio.value < 60.0f)
            {
                resolutions.RemoveAt(i);
                continue;
            }
        }

        List<string> options = new List<string>();

        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " " + Mathf.CeilToInt((float)resolutions[i].refreshRateRatio.value) + "Hz";

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            {
                currentResIndex = i;
            }

            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// Changes the current game resolution to be the selected resolution.
    /// </summary>
    public void SetResolution(Int32 resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);

        PlayerPrefs.SetInt(Options.resolutionName, resolutionIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Changes fullscreen mode.
    /// </summary>
    public void SetFullscreen(Int32 setting)
    {
        switch (setting)
        {
            case 0:
                {
                    // Fullscreen
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                }

            case 1:
                {
                    // Borderless Fullscreen
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                }

            case 2:
                {
                    // Windowed
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
                }

            default:
                {
                    // Fullscreen
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                }
        }

        PlayerPrefs.SetInt(Options.fullscreenModeName, setting);
        PlayerPrefs.Save();
    }

    #endregion

    #region HDRP Rendering

    /// <summary>
    /// Sets the current game render scale.
    /// </summary>
    public void SetRenderScale(float scale)
    {
        SettingsManager.RenderValue = scale;

        PlayerPrefs.SetFloat(Options.renderScaleName, scale);
        PlayerPrefs.Save();
    }



    /// <summary>
    /// Enables or disables HDR.
    /// </summary>
    //public void SetHDR(bool enabled)
    //{
    //    if (globalRenderSettings != null)
    //    {
    //        //globalRenderSettings.supportsHDR = enabled;
    //    }

    //    PlayerPrefs.SetInt(Options.hdrName, enabled ? 1 : 0);
    //    PlayerPrefs.Save();
    //}

    public void SetMSAA(Int32 msaaIndex)
    {
        if (globalRenderSettings != null)
        {
            switch (msaaIndex)
            {
                //case 1:
                //    globalRenderSettings.msaaSampleCount = 2;
                //    break;

                //case 2:
                //    globalRenderSettings.msaaSampleCount = 4;
                //    break;

                //case 3:
                //    globalRenderSettings.msaaSampleCount = 8;
                //    break;

                //case 0:
                //default:
                //    globalRenderSettings.msaaSampleCount = 1;
                //    break;
            }
        }

        PlayerPrefs.SetInt(Options.msaaName, msaaIndex);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Changes the quality of the anti aliasing.
    /// </summary>
    public void SetAntiAliasing(Int32 aliasingIndex)
    {
        PlayerPrefs.SetInt(Options.antiAliasingName, aliasingIndex);
        PlayerPrefs.Save();

        SettingsManager.onCameraUpdated.Invoke();
    }


    #endregion

    #region FPS & VSync

    /// <summary>
    /// Sets the application framerate cap.
    /// </summary>
    public void SetFPSCap(float framerate)
    {
        Application.targetFrameRate = (int)framerate;

        PlayerPrefs.SetInt(Options.framerateName, (int)framerate);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Enables or disables VSync.
    /// </summary>
    public void SetVSync(bool enabled)
    {
        QualitySettings.vSyncCount = enabled ? 1 : 0;

        ToggleFPSInteractable(!enabled);

        PlayerPrefs.SetInt(Options.vsyncName, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ToggleFPSInteractable(bool enabled)
    {
        fpsSlider.interactable = enabled;

        if (enabled)
        {
            foreach (var text in fpsText)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1.0f);
            }
        }
        else
        {
            foreach (var text in fpsText)
            {
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0.2f);
            }
        }
    }

    #endregion

    #region Quality

    /// <summary>
    /// Changes the quality of the game.
    /// </summary>
    public void SetQuality(Int32 qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        PlayerPrefs.SetInt(Options.qualityName, qualityIndex);
        PlayerPrefs.Save();
    }

    #endregion

    #region Brightness, Saturation, Contrast 

    /// <summary>
    /// Sets the current game brightness.
    /// </summary>
    public void SetBrightness(float brightness)
    {
        brightness = (brightness - 50) / 25.0f;

        // Change Color Adjustment
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = brightness;
        }

        PlayerPrefs.SetFloat(Options.brightnessName, brightness);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the current game saturation.
    /// </summary>
    public void SetSaturation(float saturation)
    {
        // Change Color Adjustment
        if (colorAdjustments != null)
        {
            colorAdjustments.saturation.value = saturation;
        }

        PlayerPrefs.SetInt(Options.saturationName, (int)saturation);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the current game contrast.
    /// </summary>
    public void SetContrast(float contrast)
    {
        // Change Color Adjustment
        if (colorAdjustments != null)
        {
            colorAdjustments.contrast.value = contrast;
        }

        PlayerPrefs.SetInt(Options.contrastName, (int)contrast);
        PlayerPrefs.Save();
    }

    #endregion
}