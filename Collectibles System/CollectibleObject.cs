using UnityEngine;

[CreateAssetMenu(menuName = "Item/CollectibleItem", fileName = "New Collectible")]
public class CollectibleObject : ScriptableObject
{
    [SerializeField] CollectiblesType type;
    [SerializeField] RenderTexture collectibleSprite;
    [SerializeField] DialogueObject description;

    public CollectiblesType Type => type;
    public RenderTexture CollectibleSprite => collectibleSprite;
    public DialogueObject Description => description;
}

public enum CollectiblesType
{
    AcornMug = 0,
    Teacup,
    BarrelMug,
    Bottle,
    Carton,
    CoffeeCup,
}