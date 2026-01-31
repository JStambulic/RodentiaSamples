using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCodex : MonoBehaviour
{
    #region Member Variables

    [Header("Codex Entry")]
    [SerializeField] TutorialInfoObject tutorialInfo;
    [SerializeField] TMP_Text tutorialDescription;
    [SerializeField] Button button;
    bool isUnlocked = false;

    [Header("Tutorial Codex Screen")]
    [SerializeField] TutorialCodexMenu codexMenu;

    #endregion

    #region Setup

    void Awake()
    {
        if (SaveManager.UnlockedTutorialEntries.Count > 0)
        {
            if (SaveManager.UnlockedTutorialEntries.Contains(tutorialInfo.titleLocale))
            {
                isUnlocked = true;

                tutorialDescription.text = LocalizationHelper.GetLocalizedString(tutorialInfo.tableName, tutorialInfo.titleLocale);

                button.onClick.AddListener(OpenTutorialEntry);

                return;
            }
        }

        isUnlocked = false;
        button.interactable = false;
    }

    private void OnDestroy()
    {
        if (isUnlocked)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    #endregion

    #region Open Entry

    void OpenTutorialEntry()
    {
        if (isUnlocked)
        {
            codexMenu.EnterSubmenu(tutorialInfo);
        }
    }

    #endregion
}
