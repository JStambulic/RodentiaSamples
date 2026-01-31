using UnityEngine;

/// <summary>Contains the dialogue a selected response will run, if any.</summary>
[System.Serializable]
public class DialogueResponse
{
    [SerializeField] string responseText;
    [SerializeField] string localeName;
    [SerializeField] DialogueObject dialogueObject;

    public string ResponseText => responseText;

    public DialogueObject DialogueObject => dialogueObject;

    public string LocaleName => localeName;
}
