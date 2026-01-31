using Tools;
using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    [SerializeField] CollectibleObject collectibleType;

    bool isCollected = false;

    private void Start()
    {
        if (SaveManager.IsCollectibleObtained((int)collectibleType.Type))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true;

            if (!SaveManager.IsCollectiblesSaving)
            {
                SaveManager.CollectbleObtained((int)collectibleType.Type);
            }

            HUDItemPickup itemPickupHUD = GameManager.Get().GetUIManager().GetPlayerUIObject().GetComponent<HUDItemPickup>();
            if (itemPickupHUD)
            {
                string collect = LocalizationHelper.GetLocalizedString("TutorialSignsTable", "COLLECTIBLE_OBTAINED");
                itemPickupHUD.NewConsumablePickup(collect);
            }

            if (AudioManager.instance != null)
                AudioManager.PlaySound(SFXType.CollectiblePickup, true);

            Destroy(gameObject);
        }
    }
}
