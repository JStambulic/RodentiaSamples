using UnityEngine;

/// <summary>
/// Contains the character speaking, dialogue string, and printspeed float data for a block of dialogue.
/// </summary>
[System.Serializable]
public class DialogueInformation
{
    [SerializeField] DialogueCharacters speakingCharacter = DialogueCharacters.bink;
    [SerializeField][TextArea] string dialogue;
    [SerializeField] float printSpeed = 50.0f;
    [SerializeField] string localeName;

    public DialogueCharacters SpeakingCharacter => speakingCharacter;

    public string Dialogue => dialogue;

    public float PrintSpeed => printSpeed;

    public string LocaleName => localeName;

    DialogueInformation()
    {
        printSpeed = 50.0f;
    }
}

/// <summary>
/// The characters who can appear in dialogue.
/// Will be used to change secondary portrait.
/// </summary>
public enum DialogueCharacters
{
    bink,
    unknown,
    merchant,
    merchantHivemind,
    empty
}
