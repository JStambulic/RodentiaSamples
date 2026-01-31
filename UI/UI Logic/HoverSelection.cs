using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Makes a hovered UI become the current selected GameObject.
/// Will play audio when hovered/selected and when the button is pressed.
/// </summary>
public class HoverSelection : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] TMP_Text associatedText;
    public TMP_Text AssociatedText => associatedText;

    [Header("Audio")]
    [SerializeField] public SFXType hoverSound = SFXType.MenuHovered;
    [SerializeField] public SFXType selectSound = SFXType.MenuPressed;
    Button button;

    #region Button Selected

    void Start()
    {
        button = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button)
        {
            button.onClick.AddListener(ClickedAudio);
        }
    }

    void OnDisable()
    {
        if (button)
        {
            button.onClick.RemoveListener(ClickedAudio);
        }
    }

    void OnDestroy()
    {
        if (button)
        {
            button.onClick.RemoveListener(ClickedAudio);
        }
    }

    /// <summary>
    /// Plays audio when button is clicked.
    /// </summary>
    void ClickedAudio()
    {
        AudioManager.PlaySound(selectSound, true);
    }

    #endregion

    /// <summary>
    /// On Mouse Pointer Enter, set this GameObject to be currently selected.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this != EventSystem.current.currentSelectedGameObject)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    /// <summary>
    /// On a button select, play UI navigate audio.
    /// </summary>
    /// <param name="eventData">Data of button selection from EventSystem.</param>
    public void OnSelect(BaseEventData eventData)
    {
        if (eventData.selectedObject == gameObject)
        {
            AudioManager.PlaySound(hoverSound, true);
        }
    }
}
