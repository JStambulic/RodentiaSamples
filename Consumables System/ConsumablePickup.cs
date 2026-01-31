using UnityEngine;

public class ConsumablePickup : Pickup
{
    [SerializeField] ConsumableObject consumableType;
    bool collided = false;

    [SerializeField] TutorialInfoObject tutorialInfo;
    TutorialManager tutorialManager;

    private void Start()
    {
        tutorialManager = GameManager.Get().TutorialManager;
    }

    private void OnEnable()
    {
        collided = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !wasPickedUp && !collided)
        {
            DisplayTutorial();

            collided = true;

            other.gameObject.GetComponent<PlayerInventory>().AddConsumableObject(consumableType, 1);

            PickedUp();

            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Displays the consumables tutorial if it has never yet been played.
    /// </summary>
    void DisplayTutorial()
    {
        if (!SaveManager.IsTutorialCodexUnlocked(tutorialInfo.titleLocale))
        {
            tutorialManager.StartTutorial(tutorialInfo);
        }
    }

    public void SetConsumablePickup(ConsumableObject objToBecome)
    {
        consumableType = objToBecome;
    }
}
