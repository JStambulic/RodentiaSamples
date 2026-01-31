using System.Collections;
using UnityEngine;

/// <summary>
/// Allows for Unity Events to call for a tutorial in scene.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    TutorialInfoObject queuedTutorial = null;

    private void Awake()
    {
        GameManager.Get().TutorialManager = this;
    }

    /// <summary>
    /// Begins a tutorial.
    /// </summary>
    /// <param name="tutorialToPlay">Tutorial information to show the Player.</param>
    public void StartTutorial(TutorialInfoObject tutorialToPlay)
    {
        if (tutorialToPlay == null) { Debug.LogWarning("Tutorial was null."); return; }

        // If In-Game tutorials are off, only display a new entry has been added.
        if (SettingsManager.TutorialsEnabled == false)
        {
            // Do not display if the entry already exists.
            if (!SaveManager.IsTutorialCodexUnlocked(tutorialToPlay.titleLocale))
            {
                GameManager.Get().ShowCodexUI();
            }
            // Add the entry to the codex.
            AddTutorialToCodex(tutorialToPlay);

            return;
        }

        if (queuedTutorial == null) { queuedTutorial = tutorialToPlay; }

        var gm = GameManager.Get();
        if (gm != null)
        {
            if (gm.IsInCutscene || gm.IsInDialogue || gm.IsPaused)
            {
                StartCoroutine(AwaitCutsceneEnd(1.0f));
                return;
            }
            else
            {
                //gm.EnterCutscene();
                //gm.PauseGame();
                if (gm.GetUIManager().GetTutorialMenuComp() == null)
                {
                    gm.GetUIManager().CreateTutorialMenu();
                    gm.GetUIManager().GetTutorialMenuComp().SetTutorial(tutorialToPlay);
                }
                else
                {
                    // If it already exists, reset the current tutorial object and initialize again.
                    gm.GetUIManager().GetTutorialMenuComp().SetTutorial(tutorialToPlay);
                    gm.GetUIManager().GetTutorialMenuComp().InitiateTutorial();
                }

                queuedTutorial = null;
            }
        }
    }

    /// <summary>
    /// Adds this tutorial to the tutorial codex.
    /// </summary>
    /// <param name="tutorial">Tutorial to add.</param>
    public void AddTutorialToCodex(TutorialInfoObject tutorial)
    {
        if (!SaveManager.IsTutorialCodexUnlocked(tutorial.titleLocale))
        {
            SaveManager.UnlockedTutorialEntries.Add(tutorial.titleLocale);
            SaveManager.SaveTutorialCodexEntries();
        }
    }

    /// <summary>
    /// Called if a tutorial is attempted to be started while in a cutscene or dialogue.
    /// Awaits and continuously tried again until successful.
    /// </summary>
    /// <param name="time">Time to await.</param>
    /// <returns></returns>
    IEnumerator AwaitCutsceneEnd(float time)
    {
        yield return new WaitForSeconds(time);

        StartTutorial(queuedTutorial);
    }
}
