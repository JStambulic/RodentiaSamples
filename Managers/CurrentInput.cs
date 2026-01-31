using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Processors;
using UnityEngine.InputSystem.XInput;

/// <summary>
/// Sets GameManager input reference and manages which control scheme is currently in use.
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class CurrentInput : MonoBehaviour
{
    PlayerInput playerInput;

    InputAction lookAction;

    static TMP_SpriteAsset keyboardSprites;
    static TMP_SpriteAsset playstationSprites;
    static TMP_SpriteAsset xboxSprites;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        GameManager.SetInput(playerInput);

        lookAction = playerInput.actions.FindAction("Look");

        keyboardSprites = Resources.Load<TMP_SpriteAsset>("Sprite Assets/KeyboardMouse");
        playstationSprites = Resources.Load<TMP_SpriteAsset>("Sprite Assets/PlayStation");
        xboxSprites = Resources.Load<TMP_SpriteAsset>("Sprite Assets/XBox");

        OnControlsChanged(playerInput);
    }

    void OnEnable()
    {
        if (playerInput != null)
        {
            // Subscribe to controlsChangedEvent.
            playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
        }
    }

    void OnDisable()
    {
        if (playerInput != null)
        {
            // Subscribe to controlsChangedEvent.
            playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            // Subscribe to controlsChangedEvent.
            playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }

    /// <summary>
    /// Listens for Controls Changed Event from Player Input to change the version of Controls submenu.
    /// </summary>
    void OnControlsChanged(PlayerInput input)
    {
        UpdateSpriteAsset(input);

        GameManager.Get().UpdateActions();

        UpdateCurrentSensitivity(input);
    }

    /// <summary>
    /// Updates current sprite asset.
    /// </summary>
    /// <param name="input">Current player input.</param>
    public static void UpdateSpriteAsset(PlayerInput input)
    {
        if (input.currentControlScheme == "Keyboard")
        {
            SpriteHelper.ChangeDefaultSpriteAsset(ref keyboardSprites);
        }
        else if (input.currentControlScheme == "Gamepad")
        {
            if (Gamepad.current is XInputController)
            {
                SpriteHelper.ChangeDefaultSpriteAsset(ref xboxSprites);
            }
            else if (Gamepad.current is DualShockGamepad)
            {
                SpriteHelper.ChangeDefaultSpriteAsset(ref playstationSprites);
            }
        }
    }

    /// <summary>
    /// Updates control sensitivity parameter depending on current input device.
    /// </summary>
    /// <param name="input">Current player input.</param>
    void UpdateCurrentSensitivity(PlayerInput input)
    {
        if (lookAction != null)
        {
            if (input.currentControlScheme == "Keyboard")
            {
                lookAction.ApplyParameterOverride((ScaleVector2Processor p) => p.x, PlayerPrefs.GetFloat(Options.sensitivityKeyboardName, 1));
                lookAction.ApplyParameterOverride((ScaleVector2Processor p) => p.y, PlayerPrefs.GetFloat(Options.sensitivityKeyboardName, 1));
            }
            else
            {
                lookAction.ApplyParameterOverride((ScaleVector2Processor p) => p.x, PlayerPrefs.GetFloat(Options.sensitivityGamepadName, 1) * 10.0f);
                lookAction.ApplyParameterOverride((ScaleVector2Processor p) => p.y, PlayerPrefs.GetFloat(Options.sensitivityGamepadName, 1) * 10.0f);
            }
        }
    }
}
