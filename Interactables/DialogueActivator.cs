using UnityEngine;

/// <summary>Inherits from IInteractible. Allows for player input on dialogue interactible objects.</summary>
public class DialogueActivator : MonoBehaviour, IInteractable
{
    #region Member Variables

    [SerializeField] DialogueObject dialogueObject;

    public bool canInteract = true;

    string playerTag = "Player";

    #endregion

    private void Start()
    {
        if (dialogueObject == null)
        {
            canInteract = false;
        }
    }

    #region Update Dialogue Object

    /// <summary>Updates the Interactable object to have a new dialogue string.</summary>
    /// <param DialogueObject name="dialogueObject">Dialogue Object to replace previous string.</param>
    /// <returns>Void.</returns>
    public void UpdateDialogueObject(DialogueObject dialogueObject)
    {
        this.dialogueObject = dialogueObject;
    }

    #endregion

    #region Trigger Enter & Exit

    /// <summary>Checks a collided GameObject. IF player, Interactable is valid, and player input can begin dialogue.</summary>
    /// <param Collider name="other">Collided GameObject.</param>
    /// <returns>Void.</returns>
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeSelf && canInteract)
        {
            if (other.CompareTag(playerTag) && other.TryGetComponent(out DialogueComponent playerDialogue))
            {
                playerDialogue.Interactable = this;
            }
        }
    }

    /// <summary>Checks a departing GameObject. IF player, Interactable is no longer valid.</summary>
    /// <param Collider name="other">Collided GameObject.</param>
    /// <returns>Void.</returns>
    void OnTriggerExit(Collider other)
    {
        if (canInteract)
        {
            if (other.CompareTag(playerTag) && other.TryGetComponent(out DialogueComponent playerDialogue))
            {
                if (playerDialogue.Interactable is DialogueActivator dialogueActivator && dialogueActivator == this)
                {
                    playerDialogue.Interactable = null;
                }
            }
        }
    }

    #endregion

    #region Interact

    /// <summary>Runs the DialogueMenu for dialogue UI and functionality. Checks if a DialogueComponent has responses and adds them to the DialogueMenu.</summary>
    /// <param DialogueComponent name="playerDialogue">The Dialogue Component of the Player.</param>
    /// <returns>Void.</returns>
    public void Interact(DialogueComponent playerDialogue)
    {
        if (gameObject.activeSelf)
        {
            foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
            {
                if (responseEvents.DialogueObject == dialogueObject)
                {
                    playerDialogue.DialogueUI.AddResponseEvents(responseEvents.Events);
                    break;
                }
            }

            AudioManager.PlaySound(SFXType.Interact, true);

            //GameManager.Get().GetPlayer().GetComponent<PlayerMovement>().StopPlayerMovement();
            playerDialogue.DialogueUI.BeginDialogue(dialogueObject);
        }
    }

    /// <summary>
    /// Used for forcing dialogue onto the player via an event.
    /// </summary>
    public void StartForcedDialogue()
    {
        if (gameObject.activeSelf)
        {
            DialogueComponent playerDialogue = GameManager.Player().GetComponent<DialogueComponent>();

            if (playerDialogue)
            {
                foreach (DialogueResponseEvents responseEvents in GetComponents<DialogueResponseEvents>())
                {
                    if (responseEvents.DialogueObject == dialogueObject)
                    {
                        playerDialogue.DialogueUI.AddResponseEvents(responseEvents.Events);
                        break;
                    }
                }

                GameManager.Get().GetPlayer().GetComponent<PlayerMovement>().StopPlayerMovement();
                playerDialogue.DialogueUI.BeginDialogue(dialogueObject);
            }
        }
    }

    #endregion
}
