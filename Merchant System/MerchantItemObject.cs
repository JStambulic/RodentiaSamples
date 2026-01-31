using UnityEngine;

[CreateAssetMenu(menuName = "Item/ItemToBuy", fileName = "New Item To Buy")]
public class MerchantItemObject : ScriptableObject
{
    public ConsumableObject consumableToBuy;
    public int priceOfItem;
    public DialogueObject merchantTooltip;
}
