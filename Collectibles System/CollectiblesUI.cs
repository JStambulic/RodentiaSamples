using System.Collections;
using TMPro;
using UnityEngine;

public class CollectiblesUI : MonoBehaviour, IBaseUI
{
    #region Member Variables

    public UIType type => UIType.Collectibles;

    // Starting Selection
    [SerializeField] GameObject collectiblesStart;
    public GameObject startSelection => collectiblesStart;

    [SerializeField] TMP_Text descriptionText;

    [SerializeField] GameObject cursor;

    [SerializeField] TypewriterEffect typewriterEffect;

    #endregion

    private void Start()
    {
        typewriterEffect = GetComponent<TypewriterEffect>();
    }

    public void UpdateCursor(Vector3 newSelection)
    {
        cursor.transform.position = newSelection;
    }

    #region Back Button

    /// <summary>
    /// Returns to the Main Menu and closes.
    /// </summary>
    public void Back()
    {
        // Reopen Main Menu
        GameManager.Get().GetUIManager().ShowMainMenu();

        // Close Collectibles Menu
        GameManager.Get().GetUIManager().CloseCollectiblesMenu();
    }

    #endregion

    #region Collectibles Description Handler

    /// <summary>
    /// Prints the collectible tooltip dialogue.
    /// If switching to another item and text is currently still being printed, stop printing and clear the textbox.
    /// </summary>
    /// <param name="description">Holds the collectible tooltip dialogue to be printed.</param>
    void DisplayText(string description)
    {
        if (typewriterEffect.isRunning)
        {
            typewriterEffect.Stop();
            StopAllCoroutines();
            descriptionText.text = string.Empty;
        }

        StartCoroutine(StepThroughDialogue(description));
    }

    /// <summary>
    /// Passes in the collectible tooltip string to be printed.
    /// </summary>
    /// <param name="description">Holds the collectible tooltip dialogue to be printed.</param>
    public void ReadCollectibleDescription(string description)
    {
        DisplayText(description);
    }

    /// <summary>
    /// Goes through the dialogue within a string. 
    /// </summary>
    /// <param name="description">Holds the collectible tooltip dialogue to be printed.</param>
    IEnumerator StepThroughDialogue(string description)
    {
        if (description != null)
        {
            for (int i = 0; i < description.Length; i++)
            {
                yield return RunTypingEffect(description, 75.0f);

                descriptionText.text = description;

                if (i == description.Length - 1 || !typewriterEffect.isRunning)
                {
                    break;
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// Runs the TypewriterEffect for the printing of the item tooltip dialogue.
    /// </summary>
    /// <param string name="dialogue">Holds the item tooltip dialogue to be printed.</param>
    IEnumerator RunTypingEffect(string dialogue, float printSpeed = 50.0f)
    {
        typewriterEffect.Run(dialogue, descriptionText, printSpeed, DialogueCharacters.bink);

        while (typewriterEffect.isRunning)
        {
            yield return null;
        }
    }

    #endregion
}