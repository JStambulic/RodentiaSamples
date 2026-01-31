using UnityEngine;
using System;

/// <summary>Manages the events placed onto a response option.</summary>
public class DialogueResponseEvents : MonoBehaviour
{
    #region Member Variables

    [SerializeField] DialogueObject dialogueObject;
    [SerializeField] ResponseEvent[] events;

    public DialogueObject DialogueObject => dialogueObject;

    public ResponseEvent[] Events => events;

    #endregion

    /// <summary>Executed by Unity when events are added to dialogue responses.</summary>
    /// <returns>Void.</returns>
    public void OnValidate()
    {
        if (dialogueObject == null) { return; }
        if (dialogueObject.Responses == null) { return; }
        if (events != null && events.Length == dialogueObject.Responses.Length) { return; }

        if (events == null)
        {
            events = new ResponseEvent[dialogueObject.Responses.Length];
        }
        else
        {
            Array.Resize(ref events, dialogueObject.Responses.Length);
        }

        for (int i = 0; i < dialogueObject.Responses.Length; i++)
        {
            DialogueResponse response = dialogueObject.Responses[i];

            if (events[i] != null)
            {
                events[i].name = response.ResponseText;
                continue;
            }

            events[i] = new ResponseEvent() { name = response.ResponseText };
        }
    }
}
