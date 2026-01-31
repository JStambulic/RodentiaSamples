using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains the inventory of the current Merchant NPC. 
/// Allows for the Merchant to stock different things along the level.
/// </summary>
public class Merchant : MonoBehaviour
{
    [SerializeField] List<MerchantItemObject> consumableWares;
    public List<MerchantItemObject> ConsumableWares => consumableWares;

    /// <summary>
    /// Event to be run by a ResponseEvent. Creates the Merchant Menu and hides DialogueUI.
    /// </summary>
    public void BeginMerchantShop()
    {
        GameManager.Get().GetUIManager().CreateMerchantUI(this);

        GameManager.Get().GetUIManager().HideDialogueUI();
    }
}
