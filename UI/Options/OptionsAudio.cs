using Tools;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/// <summary>
/// Holds all Audio related settings for the Options.
/// </summary>
public class OptionsAudio : MonoBehaviour
{
    #region Member Variables

    AudioMixer masterMixer;
    public AudioMixer MasterMixer => masterMixer;

    // Audio
    [Header("Audio")]
    // Master
    public Slider volumeSliderMASTER;

    // Sound Effects
    public Slider volumeSliderSFX;

    // Dialogue
    public Slider volumeSliderDIA;

    // Music
    public Slider volumeSliderMUS;

    #endregion

    #region Start, Initialize, Set Defaults

    void Start()
    {
        float volumeVal = PlayerPrefs.GetFloat(Options.masName, 1.0f);

        volumeSliderMASTER.value = volumeVal;

        volumeVal = PlayerPrefs.GetFloat(Options.sfxName, 1.0f);
        volumeSliderSFX.value = volumeVal;

        volumeVal = PlayerPrefs.GetFloat(Options.diaName, 1.0f);
        volumeSliderDIA.value = volumeVal;

        volumeVal = PlayerPrefs.GetFloat(Options.musName, 1.0f);
        volumeSliderMUS.value = volumeVal;
    }

    /// <summary>
    /// Initializes the menu with the master audio mixer.
    /// </summary>
    public void Initialize()
    {
        // Find the Master Audio Mixer.
        if (masterMixer == null)
        {
            masterMixer = SettingsManager.GetGameMixer();
        }
    }

    #endregion

    #region Volume

    /// <summary>
    /// Sets the master volume based on the float value of the slider bar.
    /// </summary>
    public void SetVolumeMaster(float volume)
    {
        masterMixer.SetFloat("volumeMaster", VolumeHelper.LogarithmicVolume(volume));

        PlayerPrefs.SetFloat(Options.masName, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the sound effects volume based on the float value of the slider bar.
    /// </summary>
    public void SetVolumeSFX(float volume)
    {
        masterMixer.SetFloat("volumeSFX", VolumeHelper.LogarithmicVolume(volume));

        PlayerPrefs.SetFloat(Options.sfxName, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the dialogue volume based on the float value of the slider bar.
    /// </summary>
    public void SetVolumeDIA(float volume)
    {
        masterMixer.SetFloat("volumeDIA", VolumeHelper.LogarithmicVolume(volume));

        PlayerPrefs.SetFloat(Options.diaName, volume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the music volume based on the float value of the slider bar.
    /// </summary>
    public void SetVolumeMUS(float volume)
    {
        masterMixer.SetFloat("volumeMUS", VolumeHelper.LogarithmicVolume(volume));

        PlayerPrefs.SetFloat(Options.musName, volume);
        PlayerPrefs.Save();
    }

    #endregion
}