using UnityEngine;

/// <summary>Contains the information data for a set of dialogue. Can have any number of responses tied to a DialogueObject.</summary>
[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{
    [SerializeField] bool hasMultipleSpeakers = false;
    [SerializeField] string translationTable = "None";

    [SerializeField] DialogueInformation[] dialogueInfo;
    [SerializeField] DialogueResponse[] responses;

    public DialogueInformation[] DialogueInfo => dialogueInfo;

    public DialogueResponse[] Responses => responses;

    public bool HasResponses => Responses != null && Responses.Length > 0;

    public bool HasMultipleSpeakers => hasMultipleSpeakers;


    public string TranslationTable => translationTable;
}