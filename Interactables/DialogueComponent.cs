using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>Manages the dialogue system on the Player.</summary>
public class DialogueComponent : MonoBehaviour
{
    DialogueMenu dialogueUIRef;

    public DialogueMenu DialogueUI => dialogueUIRef;

    public IInteractable Interactable { get; set; }

    void Start()
    {
        dialogueUIRef = GameManager.Get().GetUIManager().GetDialogueUIComp();
    }

    /// <summary>Initiates dialogue interact if Interactable is not null.</summary>
    public void InitiateDialogue(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Interactable?.Interact(this);
        }
    }
}
