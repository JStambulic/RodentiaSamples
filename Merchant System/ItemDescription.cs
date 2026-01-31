using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Contained on a shop item UI. Contains all relevant information of the item, and holds reference to the current Merchant Menu.
/// </summary>
public class ItemDescription : MonoBehaviour, ISelectHandler
{
    MerchantMenu merchantMenuRef;
    MerchantItemObject itemInfo;

    Transform cursorPoint;

    public ConsumableObject ItemType => itemInfo.consumableToBuy;

    public int Price => itemInfo.priceOfItem;

    /// <summary>
    /// Initializes with passed in values
    /// </summary>
    /// <param name="merchant">The Merchant menu creating this.</param>
    /// <param name="item">The information for this shop item</param>
    public void Intialize(MerchantMenu merchant, MerchantItemObject item)
    {
        cursorPoint = transform.Find("Cursor");
        merchantMenuRef = merchant;
        itemInfo = item;
    }

    /// <summary>
    /// Reads the item tooltip for this selected object, and updates the Merchant Menu cursor to it's position.
    /// </summary>
    /// <param name="eventData">The data of the selection event.</param>
    public void OnSelect(BaseEventData eventData)
    {
        merchantMenuRef.ReadItemDescription(itemInfo.merchantTooltip);

        StartCoroutine(PlaceCursor());
    }

    IEnumerator PlaceCursor()
    {
        yield return new WaitForEndOfFrame();
        merchantMenuRef.UpdateCursor(cursorPoint);
    }
}
