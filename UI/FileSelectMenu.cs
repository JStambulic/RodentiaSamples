using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FileSelectMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.FileSelect;

    // Starting Selection
    [SerializeField] GameObject fileSelectStart;
    public GameObject startSelection => fileSelectStart;

    [SerializeField] GameObject[] saveFileSlots;

    [SerializeField] SceneDataObject levelOneScene;

    [Header("Deletion")]
    [SerializeField] GameObject fileDeleteText;
    [SerializeField] bool isDeleting = false;
    Image saveFileSlotRef;
    int indexForDelete = -1;

    InputAction deleteAction;

    #endregion

    #region Start, Update, Show Empty File

    private void Start()
    {
        deleteAction = GameManager.Get().PlayerInputCachedRef.actions.FindAction("Delete");
        if (deleteAction != null)
        {
            deleteAction.performed += DeleteSelectedFile;
            deleteAction.canceled += CancelFileDelete;
        }

        for (int i = 0; i < saveFileSlots.Length; i++)
        {
            if (!SaveManager.CheckForSaveFile(i))
            {
                if (saveFileSlots[i])
                {
                    ShowEmptyFile(i);
                }
            }
            else
            {
                if (saveFileSlots[i])
                {
                    TMP_Text text = saveFileSlots[i].GetComponentInChildren<TMP_Text>();
                    if (text)
                    {
                        text.text = LocalizationHelper.GetLocalizedString("FileSelectTable", "fileDesignator" + (i + 1).ToString());
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (saveFileSlotRef != null && isDeleting)
        {
            saveFileSlotRef.fillAmount += Time.deltaTime * 0.5f;

            // Deletion bar is full. Delete.
            if (saveFileSlotRef.fillAmount == 1.0f)
            {
                if (SaveManager.DeleteSpecifiedSaveFile(indexForDelete))
                {
                    // Set file back to empty.
                    ShowEmptyFile(indexForDelete);

                    ClearDeletionCache();
                }
            }
        }
    }

    /// <summary>
    /// Displays a given file as empty.
    /// </summary>
    /// <param name="index">File index to change.</param>
    void ShowEmptyFile(int index)
    {
        TMP_Text text = saveFileSlots[index].GetComponentInChildren<TMP_Text>();
        if (text)
        {
            text.text = LocalizationHelper.GetLocalizedString("FileSelectTable", "emptyFile");
        }
        Image image = saveFileSlots[index].transform.Find("FileIcon").GetComponent<Image>();
        if (image)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0.25f);
        }
    }

    #endregion

    #region Deletion

    /// <summary>
    /// Begins the deletion process while the delete key is held.
    /// </summary>
    /// <param name="context">Action context data.</param>
    void DeleteSelectedFile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            for (int file = 0; file < saveFileSlots.Length; file++)
            {
                if (EventSystem.current.currentSelectedGameObject == saveFileSlots[file] &&
                    SaveManager.CheckForSaveFile(file))
                {
                    if (isDeleting == false)
                    {
                        saveFileSlotRef = saveFileSlots[file].transform.Find("DeleteProgress").GetComponent<Image>();
                        indexForDelete = file;

                        isDeleting = true;

                        // Button is now held. Begin deleting.
                        fileDeleteText.transform.position = new Vector2(saveFileSlots[indexForDelete].transform.position.x, fileDeleteText.transform.position.y);
                        fileDeleteText.SetActive(true);

                        break;
                    }
                }
            }

            if (saveFileSlotRef == null)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Cancels file deletion when button press is cancelled and isDeleting is true.
    /// </summary>
    /// <param name="context">Action context data.</param>
    void CancelFileDelete(InputAction.CallbackContext context)
    {
        // If button is no longer held while file is still deleting, cancel the deletion.
        if (context.canceled && isDeleting)
        {
            // Cancel the deletion.
            ClearDeletionCache();
        }
    }

    /// <summary>
    /// Resets all variables for file deletion.
    /// </summary>
    void ClearDeletionCache()
    {
        saveFileSlotRef.fillAmount = 0;
        saveFileSlotRef = null;
        indexForDelete = -1;
        fileDeleteText.SetActive(false);
        isDeleting = false;
    }

    #endregion

    #region Load File

    /// <summary>
    /// UI method for loading into a file. If a save file exists, it will load. Else, load into a new level 1.
    /// </summary>
    public void LoadIntoFile()
    {
        // Load into level 1 if a save file is not found.
        if (!SaveManager.CheckForSaveFile())
        {
            SceneLoadLogic.LoadScene(levelOneScene);
        }
        else
        {
            if (!GameManager.Get().IsLoading)
            {
                GameManager.Get().LoadGameAsync();
            }
        }
    }

    #endregion

    #region Back Button

    /// <summary>
    /// Returns to the Main Menu and closes.
    /// </summary>
    public void Back()
    {
        // Reopen Main Menu
        GameManager.Get().GetUIManager().ShowMainMenu();

        // Close File Select Menu
        GameManager.Get().GetUIManager().CloseFileSelectMenu();
    }

    #endregion
}
