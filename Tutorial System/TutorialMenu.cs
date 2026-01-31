using System.Collections;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Displays the action, icon, and description to teach the player a mechanic.
/// </summary>
public class TutorialMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    [Header("Tutorial Text Info")]
    [SerializeField] TutorialInfoObject tutorialInfo;
    const float tutorialTime = 15.0f;

    [Header("Text Boxes")]
    [SerializeField] TMP_Text titleTextbox;
    [SerializeField] TMP_Text tutorialTextbox;

    [Header("Tutorial Pictures")]
    [SerializeField] Image[] tutorialPictures;
    const int maxImages = 3;

    public UIType type => UIType.Tutorial;

    public GameObject startSelection => null;

    PlayerInput playerInput;
    InputAction closeInput;
    bool shouldCloseAfterTime = true;

    #endregion

    private void Start()
    {
        if (tutorialInfo == null) { ExitTutorial(); return; }

        playerInput = GameManager.Get().PlayerInputCachedRef;

        if (playerInput)
        {
            // Subscribe to controlsChangedEvent.
            playerInput.controlsChangedEvent.AddListener(OnControlsChanged);

            if (playerInput.currentActionMap.name == "Player")
            {
                closeInput = playerInput.actions.FindAction("Tutorials");
                if (closeInput != null)
                {
                    closeInput.performed += CloseOnKeypress;
                }
            }
        }
        else
        {
            Debug.LogWarning("Tutorial could not find PlayerInput!");
        }

        InitiateTutorial();
    }

    void OnEnable()
    {
        if (shouldCloseAfterTime)
        {
            StopAllCoroutines();
            StartCoroutine(CloseAfterTime(tutorialTime));
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            // Unsubscribe to controlsChangedEvent.
            playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
        }
    }

    /// <summary>
    /// Fills in all relevant tutorial information.
    /// </summary>
    public void InitiateTutorial()
    {
        titleTextbox.text = LocalizationHelper.GetLocalizedString(tutorialInfo.tableName, tutorialInfo.titleLocale);

        // Set tutorial text via dialogue object.
        SetTutorialTextbox();

        for (int i = 0; i < maxImages; i++)
        {
            if (tutorialInfo.TutorialImages.Length < i + 1)
            {
                tutorialPictures[i].gameObject.SetActive(false);
            }
            else if (tutorialInfo.TutorialImages[i] != null)
            {
                tutorialPictures[i].sprite = tutorialInfo.TutorialImages[i];
            }
        }

        if (shouldCloseAfterTime)
        {
            StopAllCoroutines();
            StartCoroutine(CloseAfterTime(tutorialTime));
        }
    }

    /// <summary>
    /// Updates the tutorial with correct inputs when controls switch.
    /// </summary>
    /// <param name="input">Current Player input.</param>
    void OnControlsChanged(PlayerInput input)
    {
        SetTutorialTextbox();
    }

    /// <summary>
    /// Sets the tutorial object.
    /// </summary>
    /// <param name="tutorialToPlay">Current tutorial to run.</param>
    public void SetTutorial(TutorialInfoObject tutorialToPlay, bool closeAfterTime = true)
    {
        // If it already exists, save it to the codex before swapping to new tutorial.
        if (tutorialInfo != null)
        {
            if (!SaveManager.IsTutorialCodexUnlocked(tutorialInfo.titleLocale))
            {
                GameManager.Get().TutorialManager.AddTutorialToCodex(tutorialInfo);
            }
        }

        tutorialInfo = tutorialToPlay;
        shouldCloseAfterTime = closeAfterTime;
    }

    /// <summary>
    /// Sets the tutorial textbox with localized text and any sprites.
    /// </summary>
    private void SetTutorialTextbox()
    {
        string tutorialText = string.Empty;

        // Localize the string.
        tutorialText = LocalizationHelper.GetLocalizedString(tutorialInfo.tableName, tutorialInfo.tutorialTextLocale);
        if (tutorialText == string.Empty)
        {
            tutorialText = tutorialInfo.TutorialDialogue;
        }

        tutorialText = SpriteHelper.StringFormat(tutorialText, GameManager.Get().ActionsDictionary);

        tutorialTextbox.text = tutorialText;
    }

    /// <summary>
    /// Button press for closing a tutorial screen.
    /// </summary>
    /// <param name="context">Input context.</param>
    void CloseOnKeypress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ExitTutorial();
        }
    }

    /// <summary>
    /// Closes the tutorial after time.
    /// </summary>
    /// <param name="time">Time to wait.</param>
    /// <returns>IEnumerator</returns>
    IEnumerator CloseAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        ExitTutorial();
    }

    /// <summary>
    /// Closes the tutorial and returns to normal gameplay.
    /// </summary>
    public void ExitTutorial()
    {
        var gm = GameManager.Get();
        if (gm != null)
        {
            if (!SaveManager.IsTutorialCodexUnlocked(tutorialInfo.titleLocale))
            {
                GameManager.Get().TutorialManager.AddTutorialToCodex(tutorialInfo);
                gm.ShowCodexUI();
            }
            gm.GetUIManager().CloseTutorialMenu();
        }
    }
}
