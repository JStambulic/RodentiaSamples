using UnityEngine;

/// <summary>
/// Script for a save file button.
/// </summary>
public class SaveFile : MonoBehaviour
{
    public int fileSlotIndex;

    /// <summary>
    /// Sets the game's active save file upon selection.
    /// </summary>
    public void SetActiveSaveSlot()
    {
        SaveManager.activeSaveSlot = fileSlotIndex;
    }
}
