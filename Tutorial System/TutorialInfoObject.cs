using UnityEngine;

/// <summary>
/// A data object containing the title, dialogue, and images for a tutorial.
/// </summary>
[CreateAssetMenu(menuName = "Tutorial Object", fileName = "New Tutorial Object")]
public class TutorialInfoObject : ScriptableObject
{
    [Header("Title")]
    public string titleLocale;
    public string tableName = "TutorialSignsTable";

    [Header("Information")]
    public string tutorialTextLocale;
    [SerializeField][TextArea] string tutorialDialogue;

    [Header("Images")]
    [SerializeField] Sprite[] tutorialImages;

    public string TutorialDialogue => tutorialDialogue;
    public Sprite[] TutorialImages => tutorialImages;
}
