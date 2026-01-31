using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;
using UnityEngine.UI;

/// <summary>
/// Video data to be saved to config json.
/// </summary>
public class OptionsCameraControls : MonoBehaviour
{
    #region Member Variables

    // Controls
    [SerializeField] string controlType;

    [Header("Control Settings")]
    [SerializeField] Toggle invertXToggle;

    [SerializeField] Toggle invertYToggle;

    [SerializeField] Slider sensitivitySlider;

    private InputAction lookInput;

    #endregion

    #region Start, OnEnable, Initialize

    void Start()
    {
        SetOptionsValues();
    }

    void OnEnable()
    {
        SetOptionsValues();
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject.name == "ResetToDefaultButton")
                {
                    // When a Reset button is pressed, it becomes disabled, breaking navigation for Gamepad.
                    // This will set the current selected GameObject to become the Rebind button associated with that Reset, fixing the issue.
                    if (!EventSystem.current.currentSelectedGameObject.activeSelf)
                    {
                        var go = EventSystem.current.currentSelectedGameObject.gameObject.transform.parent.Find("TriggerRebindButton");
                        if (go)
                        {
                            EventSystem.current.SetSelectedGameObject(go.gameObject);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the options for when submenu is created/enabled.
    /// </summary>
    void SetOptionsValues()
    {
        invertXToggle.isOn = PlayerPrefs.GetInt(Options.invertXName, 0) > 0 ? true : false;
        invertYToggle.isOn = PlayerPrefs.GetInt(Options.invertYName, 0) > 0 ? true : false;

        if (controlType == "Keyboard")
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat(Options.sensitivityKeyboardName, 1.0f);
        }
        else
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat(Options.sensitivityGamepadName, 1.0f);
        }
    }

    /// <summary>
    /// Initializes the menu with the look InputAction.
    /// </summary>
    public void Initialize()
    {
        if (lookInput == null)
        {
            InputActionAsset inputActions = SettingsManager.GetInputActions();

            lookInput = inputActions.FindActionMap("Player").FindAction("Look");
        }
    }

    #endregion

    #region Camera Behaviours

    /// <summary>
    /// Applies an override to Look action's Scale processor to increase/decrease sensitivity.
    /// Gamepad sensitivity is given a 10x multiplier.
    /// </summary>
    public void ChangeCameraSensitivity(float sensitivity)
    {
        if (controlType == "Keyboard")
        {
            lookInput.ApplyParameterOverride((ScaleVector2Processor p) => p.x, sensitivity);
            lookInput.ApplyParameterOverride((ScaleVector2Processor p) => p.y, sensitivity);

            PlayerPrefs.SetFloat(Options.sensitivityKeyboardName, sensitivity);
        }
        else
        {
            lookInput.ApplyParameterOverride((ScaleVector2Processor p) => p.x, sensitivity * 10);
            lookInput.ApplyParameterOverride((ScaleVector2Processor p) => p.y, sensitivity * 10);

            PlayerPrefs.SetFloat(Options.sensitivityGamepadName, sensitivity);
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Applies an override to Look action's Invert processor to invert camera X.
    /// </summary>
    public void InvertCameraX(bool invert)
    {
        lookInput.ApplyParameterOverride((InvertVector2Processor p) => p.invertX, invert);

        PlayerPrefs.SetInt(Options.invertXName, invert ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Applies an override to Look action's Invert processor to invert camera Y.
    /// </summary>
    public void InvertCameraY(bool invert)
    {
        lookInput.ApplyParameterOverride((InvertVector2Processor p) => p.invertY, invert);

        PlayerPrefs.SetInt(Options.invertYName, invert ? 1 : 0);
        PlayerPrefs.Save();
    }

    #endregion
}