using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Updates a UI help sprite to be the current control scheme sprites.
/// </summary>
public class MenuNavigationText : MonoBehaviour
{
    PlayerInput inputRef;

    public TMP_Text spriteTextBox;
    public string[] spriteNamesKeyboard;
    public string[] spriteNamesGamepad;

    #region OnEnable, Start, OnDisable, OnDestroy
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (inputRef == null)
        {
            inputRef = GameManager.Get().PlayerInputCachedRef;
        }
        if (inputRef != null)
        {
            // Subscribe to controlsChangedEvent.
            inputRef.controlsChangedEvent.AddListener(OnControlsChanged);

            OnControlsChanged(inputRef);
        }
    }

    void Start()
    {
        if (inputRef == null)
        {
            inputRef = GameManager.Get().PlayerInputCachedRef;

            if (inputRef != null ) { OnControlsChanged(inputRef); }
        }
    }

    private void OnDisable()
    {
        if (inputRef != null)
        {
            // Subscribe to controlsChangedEvent.
            inputRef.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }

    void OnDestroy()
    {
        if (inputRef != null)
        {
            // Subscribe to controlsChangedEvent.
            inputRef.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }
    #endregion

    #region Controls Changed
    /// <summary>
    /// Updates the sprites that show the controls when input source is changed.
    /// </summary>
    /// <param name="input"> The player's input. </param>
    StringBuilder sb = new StringBuilder();
    void OnControlsChanged(PlayerInput input)
    {
        sb.Clear();

        if (input.currentControlScheme == "Keyboard")
        {
            if (spriteTextBox != null)
            {
                for (int i = 0; i < spriteNamesKeyboard.Length; i++)
                {
                    sb.Append("<sprite name=").Append(spriteNamesKeyboard[i]).Append(">");
                }
            }
        }
        else
        {
            if (spriteTextBox != null)
            {
                for (int i = 0; i < spriteNamesGamepad.Length; i++)
                {
                    sb.Append("<sprite name=").Append(spriteNamesGamepad[i]).Append(">");
                }                
            }
        }

        spriteTextBox.text = sb.ToString();
    }
    #endregion
}
