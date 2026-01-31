using System.Collections;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>Core code for the dialogue system. Manages all UI and begins the printing.</summary>
public class DialogueMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.Dialogue;

    [SerializeField] GameObject dialogueStart;
    public GameObject startSelection => dialogueStart;

    // References
    [Header("Dialogue UI Reference")]
    [SerializeField] TMP_Text textLabel;
    [SerializeField] GameObject dialogueUI;
    [SerializeField] GameObject progressIcon;

    // Portraits
    [Header("Portraits")]
    [SerializeField] Sprite[] portraitSprites;
    [SerializeField] GameObject primaryPortrait;
    Vector3 primaryDefaultSize;
    [SerializeField] GameObject secondaryPortrait;
    Vector3 secondaryDefaultSize;

    public bool isInDialogue { get; private set; }

    ResponseHandler responseHandler;
    TypewriterEffect typewriterEffect;

    DialogueCharacters previousSpeaker = DialogueCharacters.empty;
    float portraitScaleDown = 1.25f;

    PlayerInput playerInput;
    InputAction progressInput;

    #endregion

    #region Start

    /// <summary>Finds and set necessary member variables.</summary>
    /// <returns>Void.</returns>
    void Start()
    {
        if (GameManager.Get())
        {
            playerInput = GameManager.Get().PlayerInputCachedRef;
        }

        if (playerInput)
        {
            progressInput = playerInput.actions.FindAction("ProgressDialogue");
        }

        typewriterEffect = GetComponent<TypewriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();

        primaryDefaultSize = primaryPortrait.transform.localScale;
        secondaryDefaultSize = secondaryPortrait.transform.localScale;

        EndDialogue();
    }

    #endregion

    #region Begin & End Dialogue

    /// <summary>Begins and opens the dialogue menu and UI. Sends the dialogue to the StepThrough coroutine.</summary>
    /// <param DialogueObject name="dialogueObject">Holds the block of dialogue to be printed.</param>
    /// <returns>Void.</returns>
    public void BeginDialogue(DialogueObject dialogueObject)
    {
        playerInput.SwitchCurrentActionMap("UI");

        var gm = GameManager.Get();

        if (gm.GetUIManager().IsActive(UIType.Tutorial))
        {
            gm.GetUIManager().HideTutorialUI();
        }

        gm.GetUIManager().HidePlayerUI();
        gm.GetUIManager().ShowDialogueUI();
        gm.SetMouseCursor(true);

        gm.PauseGame();

        // Disable secondary portrait.
        secondaryPortrait.SetActive(false);

        isInDialogue = true;
        gm.SetIsInDialogue(isInDialogue);

        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    /// <summary>Ends the dialogue menu, closing the UI and giving player back control.</summary>
    /// <returns>Void.</returns>
    public void EndDialogue()
    {
        UIManager uiRef = GameManager.Get().GetUIManager();
        var gm = GameManager.Get();

        uiRef.HideDialogueUI();

        // Ensures that Merchant UI will not show until any additional dialogue is completed. Else, return to game.
        MerchantMenu merchant = uiRef.GetMerchantUIComp();
        if (merchant)
        {
            if (merchant.IsInShop)
            {
                uiRef.ShowMerchantUI();
            }
        }
        else
        {
            gm.SetMouseCursor(false);
            gm.ResumeGame();

            if (gm.GetUIManager().GetTutorialMenuComp())
            {
                gm.GetUIManager().ShowTutorialUI();
            }

            if (playerInput)
            {
                playerInput.SwitchCurrentActionMap("Player");
            }
            uiRef.ShowPlayerUI();
        }

        textLabel.text = string.Empty;

        // Disable secondary portrait.
        secondaryPortrait.SetActive(false);
        previousSpeaker = DialogueCharacters.empty;
        // Reset Dink's portrait.
        primaryPortrait.transform.localScale = primaryDefaultSize;
        primaryPortrait.GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);

        isInDialogue = false;
        gm.SetIsInDialogue(isInDialogue);
    }

    #endregion

    #region Dialogue Handler

    /// <summary>Goes through the dialogue within a DialogueObject. Press DialogueProgress input to continue to next block, or end the dialogue.</summary>
    /// <summary>Will stop and display options if a DialogueObject has any DialogueResponse.</summary>
    /// <param DialogueObject name="dialogueObject">Holds the next block of dialogue to be printed.</param>
    /// <returns>IEnumerator.</returns>
    IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        if (dialogueObject != null)
        {
            // If there is multiple speakers, enable the second portrait.
            if (dialogueObject.HasMultipleSpeakers)
            {
                secondaryPortrait.SetActive(true);
                ChangeSecondaryPortraitAtStart(dialogueObject);
            }

            for (int i = 0; i < dialogueObject.DialogueInfo.Length; i++)
            {
                progressIcon?.SetActive(false);

                // Localize the string.
                string dialogue = dialogueObject.DialogueInfo[i].Dialogue;
                if (dialogueObject.TranslationTable != "None")
                {
                    dialogue = LocalizationHelper.GetLocalizedString(dialogueObject.TranslationTable, dialogueObject.DialogueInfo[i].LocaleName);
                    if (dialogue == string.Empty)
                    {
                        dialogue = dialogueObject.DialogueInfo[i].Dialogue;
                    }
                }

                dialogue = SpriteHelper.StringFormat(dialogue, GameManager.Get().ActionsDictionary);

                // Changes the focus onto whoever is speaking currently.
                if (dialogueObject.HasMultipleSpeakers)
                {
                    ChangeSpeaker(dialogueObject, i);
                }

                yield return RunTypingEffect(dialogue, dialogueObject.DialogueInfo[i].PrintSpeed, dialogueObject.DialogueInfo[i].SpeakingCharacter);

                textLabel.text = dialogue;

                if (i == dialogueObject.DialogueInfo.Length - 1 && dialogueObject.HasResponses)
                {
                    break;
                }

                // Show dialogue progression icon after text print if no responses.
                progressIcon?.SetActive(true);
                //progressIcon.transform.position = textLabel.textInfo.characterInfo[textLabel.textInfo.characterCount - 1].bottomLeft;

                yield return null;
                yield return new WaitUntil(() => progressInput.WasPressedThisFrame());
            }

            if (dialogueObject.HasResponses)
            {
                responseHandler.ShowResponses(dialogueObject);
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            EndDialogue();
        }
    }

    /// <summary>Changes focus on who becomes the highlighted speaker when there is more than one in a dialogue.</summary>
    /// <returns>Void.</returns>
    void ChangeSpeaker(DialogueObject dialogueObject, int index)
    {
        DialogueCharacters currentSpeaker = dialogueObject.DialogueInfo[index].SpeakingCharacter;
        ChangeSecondaryPortrait(currentSpeaker);

        if (currentSpeaker != previousSpeaker)
        {
            if (currentSpeaker == DialogueCharacters.bink)
            {
                // Primary speaker becomes highlighted.
                primaryPortrait.transform.localScale = primaryDefaultSize;
                primaryPortrait.GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);

                secondaryPortrait.transform.localScale = secondaryDefaultSize / portraitScaleDown;
                secondaryPortrait.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
            else
            {
                // Secondary Speaker becomes highlighted.
                primaryPortrait.transform.localScale = primaryDefaultSize / portraitScaleDown;
                primaryPortrait.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

                secondaryPortrait.transform.localScale = secondaryDefaultSize;
                secondaryPortrait.GetComponent<Image>().color = new Color(1, 1, 1, 1.0f);
            }
        }

        previousSpeaker = dialogueObject.DialogueInfo[index].SpeakingCharacter;
    }

    /// <summary>
    /// Changes secondary portrait dynamically. Allows for multiple secondary speakers in one dialogue.
    /// </summary>
    /// <param name="currentSpeaker"></param>
    void ChangeSecondaryPortrait(DialogueCharacters currentSpeaker)
    {
        switch (currentSpeaker)
        {
            case DialogueCharacters.unknown:
                secondaryPortrait.GetComponent<Image>().sprite = portraitSprites[(int)currentSpeaker];
                break;
            case DialogueCharacters.merchant:
                secondaryPortrait.GetComponent<Image>().sprite = portraitSprites[(int)currentSpeaker];
                break;
            case DialogueCharacters.merchantHivemind:
                secondaryPortrait.GetComponent<Image>().sprite = portraitSprites[(int)currentSpeaker];
                break;
            case DialogueCharacters.bink:
            default:
                break;
        }
    }

    /// <summary>
    /// Changes secondary portrait to first found secondary character on dialogue begin.
    /// </summary>
    /// <param name="dialogueObject">Current dialogue.</param>
    void ChangeSecondaryPortraitAtStart(DialogueObject dialogueObject)
    {
        foreach (var character in dialogueObject.DialogueInfo)
        {
            if (character.SpeakingCharacter == DialogueCharacters.bink) { continue; }
            else { ChangeSecondaryPortrait(character.SpeakingCharacter); break; }
        }
    }

    /// <summary>Runs the TypewriterEffect for the printing of the dialogue. Can click ProgressDialogue input to skip effect.</summary>
    /// <param string name="dialogue">Holds the block of dialogue to be printed.</param>
    /// <returns>IEnumerator.</returns>
    IEnumerator RunTypingEffect(string dialogue, float printSpeed, DialogueCharacters character = DialogueCharacters.bink)
    {
        typewriterEffect.Run(dialogue, textLabel, printSpeed, character);

        while (typewriterEffect.isRunning)
        {
            yield return null;

            if (progressInput.WasPressedThisFrame())
            {
                typewriterEffect.Stop();
            }
        }
    }

    #endregion

    #region Response Events

    /// <summary>Sends any ResponseEvents to the responseHandler.</summary>
    /// <param ResponseEvent[] name="events">The events for a dialogue response.</param>
    /// <returns>Void.</returns>
    public void AddResponseEvents(ResponseEvent[] events)
    {
        responseHandler.AddResponseEvents(events);
    }

    #endregion
}
