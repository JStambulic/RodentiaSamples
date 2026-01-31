using Tools;
using UnityEngine;

/// <summary>
/// Confirmation Menu type specifically for data management in Options.
/// </summary>
public class DataConfirmationMenu : ConfirmationMenu
{
    string settingsWipeKey = "WARNING_SETTINGS";
    string tutorialsWipeKey = "WARNING_TUTORIALS";
    string collectiblesWipeKey = "WARNING_COLLECTIBLES";
    string gameDataWipeKey = "WARNING_GAMEDATA";

    // Start is called before the first frame update
    void Start()
    {
        switch (buttonType)
        {
            case MenuReturnType.optionsSettings:
                textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", settingsWipeKey);
                break;

            case MenuReturnType.optionsTutorials:
                textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", tutorialsWipeKey);
                break;

            case MenuReturnType.optionsCollectibles:
                textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", collectiblesWipeKey);
                break;

            case MenuReturnType.optionsGameData:
                textLabel.text = LocalizationHelper.GetLocalizedString("ConfirmationMenuTable", gameDataWipeKey);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Does an action upon yes being selected.
    /// </summary>
    public override void YesSelected()
    {
        switch (buttonType)
        {
            case MenuReturnType.optionsSettings:
                PlayerPrefs.DeleteAll();
                SettingsManager.ApplyGameSettings();
                GameManager.Get().GetUIManager().OptionsUI_Instance.Back();
                break;

            case MenuReturnType.optionsTutorials:
                SaveManager.WipeTutorialsProgress();
                GameManager.Get().GetUIManager().OptionsUI_Instance.GameDataOptions.tutorials.interactable = false;
                break;

            case MenuReturnType.optionsCollectibles:
                SaveManager.WipeCollectiblesProgress();
                GameManager.Get().GetUIManager().OptionsUI_Instance.GameDataOptions.collectibles.interactable = false;
                break;

            case MenuReturnType.optionsGameData:
                PlayerPrefs.DeleteAll();
                SettingsManager.ApplyGameSettings();
                SaveManager.WipeGameData();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
             Application.Quit();
#endif
                break;

            default:
                break;
        }

        GameManager.Get().GetUIManager().CloseConfirmationMenu();
    }

    /// <summary>
    /// Calls base NoSelected and closes this menu.
    /// </summary>
    public override void NoSelected()
    {
        base.NoSelected();
    }
}
