using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Tools;

/// <summary>Manages how responses to dialogue are handled. Runs given dialogue/event when a response is selected.</summary>
public class ResponseHandler : MonoBehaviour
{
    #region Member Variables

    [SerializeField] GameObject progressDialogueHelper;

    [SerializeField] RectTransform signpost;

    [SerializeField] RectTransform responseBox;
    [SerializeField] RectTransform responseButtonTemplate;
    [SerializeField] RectTransform responseContainer;

    DialogueMenu dialogueUI;
    ResponseEvent[] responseEvents;

    List<GameObject> tempResponseList = new List<GameObject>();

    #endregion

    #region Start

    /// <summary>Start runs when scene loads. Gets UI component.</summary>
    void Start()
    {
        dialogueUI = GetComponent<DialogueMenu>();

        responseBox.gameObject.SetActive(false);
    }

    #endregion

    #region Show Response Boxes

    /// <summary>Shows the responses to a DialogueObject if they have any. Creates an interactible button.</summary>
    /// <param DialogueResponse[] name="responses">The number of response options on the DialogueObject.</param>
    /// <returns>Void.</returns>
    public void ShowResponses(DialogueObject dialogueObj)
    {
        progressDialogueHelper.SetActive(false);
        signpost.gameObject.SetActive(true);

        float responseBoxHeight = 0.0f;

        DialogueResponse[] responses = dialogueObj.Responses;

        for (int i = 0; i < responses.Length; i++) 
        {
            DialogueResponse response = responses[i];
            int responseIndex = i;

            string localizedText = response.ResponseText;
            if (dialogueObj.TranslationTable != "None")
            {
                localizedText = LocalizationHelper.GetLocalizedString(dialogueObj.TranslationTable, response.LocaleName);
                if (localizedText == string.Empty) { localizedText = response.ResponseText; }
            }

            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponentInChildren<TMP_Text>().text = localizedText;
            responseButton.GetComponentInChildren<Button>().onClick.AddListener(() => OnPickedResponse(response, responseIndex));

            if (responseIndex == 0)
            {
                SelectLogic.SetSelectionFromButton(responseButton.GetComponentInChildren<Button>().gameObject);
            }

            tempResponseList.Add(responseButton);

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    #endregion

    #region On Response Chosen

    /// <summary>Runs the dialogue tied to the clicked button.</summary>
    /// <param DialogueResponse name="response">Holds the next block of dialogue to be printed.</param>
    /// <param int name="responseIndex">Used to run any events on a specific response option if valid.</param>
    /// <returns>Void.</returns>
    void OnPickedResponse(DialogueResponse response, int responseIndex)
    {
        progressDialogueHelper.SetActive(true);
        responseBox.gameObject.SetActive(false);
        signpost.gameObject.SetActive(false);

        foreach (GameObject button in tempResponseList)
        {
            Destroy(button);
        }
        tempResponseList.Clear();

        if (responseEvents != null && responseIndex <= responseEvents.Length)
        {
            responseEvents[responseIndex].OnPickedResponse?.Invoke();
        }

        responseEvents = null;

        // IF dialogue object contains a valid string, output. Otherwise, close dialogue UI.
        if (response.DialogueObject)
        {
            dialogueUI.BeginDialogue(response.DialogueObject);
        }
        else
        {
            dialogueUI.EndDialogue();
        }
    }

    #endregion

    #region Response Events

    /// <summary>Adds any response events of DialogueObject to this component's local responseEvents.</summary>
    /// <param ResponseEvent[] name="responseEvents">The response events on the dialogue object.</param>
    /// <returns>Void.</returns>
    public void AddResponseEvents(ResponseEvent[] responseEvents)
    {
        this.responseEvents = responseEvents;
    }

    #endregion
}
