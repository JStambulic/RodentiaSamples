using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Script for resetting all overrides on keybindings.</summary>
public class ResetDeviceBindings : MonoBehaviour
{
    [SerializeField] InputActionAsset _inputActions;
    [SerializeField] string _targetControlScheme;

    /// <summary>Resets all keybinds across every ActionMap.</summary>
    public void ResetAllBindings()
    {
        foreach (InputActionMap map in _inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    /// <summary>Resets all keybinds on a given ActionMap.</summary>
    public void ResetControlSchemeBinding()
    {
        foreach (InputActionMap map in _inputActions.actionMaps)
        {
            foreach (InputAction action in map.actions)
            {
                action.RemoveBindingOverride(InputBinding.MaskByGroup(_targetControlScheme));
            }
        }
    }
}
