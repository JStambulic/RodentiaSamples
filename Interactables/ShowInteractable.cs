using UnityEngine;
using UnityEngine.UI;

/// <summary>Displays a dialogue interact icon over a GameObject. Tracks position even when camera is moving.</summary>
public class ShowInteractable : MonoBehaviour
{
    #region Member Variables

    Image dialogueIcon = null;
    [SerializeField] Transform iconSpawnPosition;

    [SerializeField] GameObject keypressIndicator;

    public bool playerInRange = false;
    string playerTag = "Player";

    #endregion

    #region Start, Update

    void Start()
    {
        //dialogueIcon = GameManager.Get().GetUIManager().GetPlayerUIComp().InteractIcon;
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogueIcon && playerInRange)
        {
            UpdateUIPosition();
        }
    }

    #endregion

    /// <summary>Ensures Interact icon will never remain onscreen even if object is disabled/destroyed.</summary>
    #region OnDisable and OnDestroy

    void OnDisable()
    {
        if (dialogueIcon)
        {
            dialogueIcon.gameObject.SetActive(false);
            playerInRange = false;
        }

        if (keypressIndicator)
        {
            keypressIndicator.gameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (dialogueIcon)
        {
            dialogueIcon.gameObject.SetActive(false);
            playerInRange = false;
        }

        if (keypressIndicator)
        {
            keypressIndicator.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Trigger Enter & Exit

    /// <summary>Checks a collided GameObject. IF player, show dialogue interact icon.</summary>
    /// <param Collider name="other">Collided GameObject.</param>
    /// <returns>Void.</returns>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;

            if (dialogueIcon)
            {
                dialogueIcon.transform.position = Camera.main.WorldToScreenPoint(iconSpawnPosition.position);
                dialogueIcon.gameObject.SetActive(true);
            }

            if (keypressIndicator)
            {
                keypressIndicator.gameObject.SetActive(true);
            }
        }
    }

    /// <summary>Checks a departing GameObject. IF player, hide dialogue interact icon.</summary>
    /// <param Collider name="other">Collided GameObject.</param>
    /// <returns>Void.</returns>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag) && playerInRange)
        {
            playerInRange = false;

            if (dialogueIcon)
            {
                dialogueIcon.gameObject.SetActive(false);
            }

            if (keypressIndicator)
            {
                keypressIndicator.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region UI Positioning

    /// <summary>Updates the Icon UI element to always be over the interactible object while isWithinRange is true</summary>
    /// <returns>Void</returns>
    void UpdateUIPosition()
    {
        if (GameManager.Get().IsInCutscene) { return; }

        if (dialogueIcon)
        {
            dialogueIcon.transform.position = Vector3.Lerp(dialogueIcon.transform.position, Camera.main.WorldToScreenPoint(iconSpawnPosition.position), 50.0f * Time.deltaTime);
        }
    }

    #endregion
}
