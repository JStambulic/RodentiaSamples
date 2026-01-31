using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The logic for the Merchant shop UI. 
/// </summary>
public class MerchantMenu : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.Merchant;

    // Starting Selection
    [SerializeField] GameObject merchantStart;
    public GameObject startSelection => merchantStart;

    [Header("Item Selection Cursor")]
    [SerializeField] GameObject cursor;

    // Text
    [Header("Sprites")]
    [SerializeField] Image merchantImage;
    [SerializeField] Sprite merchantDefault;
    [SerializeField] Sprite merchantTalk;

    [Header("Text")]
    [SerializeField] TMP_Text merchantText;
    [SerializeField] DialogueObject insufficientFunds;
    [SerializeField] DialogueObject purchaseSuccessful;

    // Item UI
    [Header("Shopfront Items")]
    [SerializeField] RectTransform itemToBuy;
    [SerializeField] RectTransform itemToBuyContainer;
    List<GameObject> itemsInShop = new List<GameObject>();

    [Header("Player Currency")]
    [SerializeField] TMP_Text playerCurrencyAmount;
    CurrencyManager currencyManager;

    public Merchant merchantInventory { get; set; }

    TypewriterEffect typewriterEffect;

    bool isInShop;
    public bool IsInShop => isInShop;

    #endregion

    #region Start

    void Start()
    {
        isInShop = true;

        currencyManager = GameManager.Get().CurrencyManager;
        UpdateDisplayedCurrency();

        typewriterEffect = GetComponent<TypewriterEffect>();

        // Create all shop items held by the given Merchant inventory.
        foreach (MerchantItemObject item in merchantInventory.ConsumableWares)
        {
            GameObject newItem = Instantiate(itemToBuy.gameObject, itemToBuyContainer);

            newItem.gameObject.SetActive(true);
            newItem.GetComponent<Image>().sprite = item.consumableToBuy.ConsumableIcon;
            newItem.GetComponent<Button>().onClick.AddListener(delegate { ItemBought(newItem); });
            newItem.GetComponentInChildren<TMP_Text>().text = item.priceOfItem.ToString();

            newItem.GetComponent<ItemDescription>().Intialize(this, item);

            itemsInShop.Add(newItem.gameObject);

            if (item == merchantInventory.ConsumableWares[0])
            {
                merchantStart = newItem.gameObject;
            }
        }

        // Ensures all final dialogue can be played out before opening the shop menu.
        if (GameManager.Get().IsInDialogue)
        {
            GameManager.Get().GetUIManager().HideMerchantUI();
        }
    }

    private void OnDestroy()
    {
        isInShop = false;

        foreach (GameObject item in itemsInShop)
        {
            item.GetComponent<Button>().onClick.RemoveAllListeners();
        }
        itemsInShop.Clear();
    }

    private void OnEnable()
    {
        if (merchantStart)
        {
            UpdateCursor(merchantStart.transform);
        }
    }

    #endregion

    #region Selection

    /// <summary>
    /// Updates the cursor to appear on the currently selected item in the shop.
    /// </summary>
    /// <param name="newSelection">The position of the item.</param>
    public void UpdateCursor(Transform newSelection)
    {
        cursor.transform.position = newSelection.position;
    }

    #endregion

    #region Manage Currency

    /// <summary>
    /// Checks if Player has enough money to buy the selected item.
    /// If so, subtract from their currency, and add the item to their inventory and give purchase successful text. Otherwise, give insufficient funds message.
    /// </summary>
    /// <param name="boughtItem">The button containing the Item Description for what is being bought</param>
    public void ItemBought(GameObject boughtItem)
    {
        if (boughtItem)
        {
            if (currencyManager)
            {
                int priceOfItem = boughtItem.GetComponent<ItemDescription>().Price;

                if (currencyManager.Currency - priceOfItem >= 0)
                {
                    boughtItem.GetComponent<HoverSelection>().selectSound = SFXType.MerchantPurchase;

                    currencyManager.ItemBought(priceOfItem);

                    GameManager.Get().Inventory.AddConsumableObject(boughtItem.GetComponent<ItemDescription>().ItemType, 1);

                    DisplayText(purchaseSuccessful);
                    UpdateDisplayedCurrency();
                }
                else
                {
                    boughtItem.GetComponent<HoverSelection>().selectSound = SFXType.MerchantNoCash;
                    DisplayText(insufficientFunds);
                }
            }
        }
    }

    /// <summary>
    /// Updates the currency text to what the Player's currencyManager currently holds.
    /// </summary>
    void UpdateDisplayedCurrency()
    {
        if (currencyManager)
        {
            playerCurrencyAmount.text = currencyManager.Currency.ToString();
        }
    }

    #endregion

    #region End Shop

    /// <summary>
    /// Reopens Player UI, sets action map back to Player, and closes the Shop.
    /// </summary>
    public void CloseShop()
    {
        isInShop = false;

        var gm = GameManager.Get();

        if (gm.GetUIManager().GetTutorialMenuComp())
        {  
            gm.GetUIManager().ShowTutorialUI();
        }

        gm.SetMouseCursor(false);
        gm.PlayerInputCachedRef.SwitchCurrentActionMap("Player");
        gm.GetUIManager().ShowPlayerUI();
        
        gm.ResumeGame();
        
        gm.GetUIManager().CloseMerchantUI();
    }

    #endregion

    #region Item Description Dialogue Handler

    /// <summary>
    /// Prints the item tooltip dialogue.
    /// If switching to another item and text is currently still being printed, stop printing and clear the textbox.
    /// </summary>
    /// <param name="dialogueObject">Holds the item tooltip dialogue to be printed.</param>
    void DisplayText(DialogueObject dialogueObject)
    {
        if (typewriterEffect.isRunning)
        {
            typewriterEffect.Stop();
            StopAllCoroutines();
            merchantText.text = string.Empty;
        }

        if (merchantImage)
        {
            merchantImage.sprite = merchantTalk;
        }
        StartCoroutine(StepThroughDialogue(dialogueObject));
    }

    /// <summary>
    /// Passes in the item tooltip dialogue to be printed.
    /// </summary>
    /// <param name="dialogueObject">Holds the item tooltip dialogue to be printed.</param>
    public void ReadItemDescription(DialogueObject dialogueObject)
    {
        DisplayText(dialogueObject);
    }

    /// <summary>
    /// Goes through the dialogue within a DialogueObject. 
    /// </summary>
    /// <param DialogueObject name="dialogueObject">Holds the item tooltip dialogue to be printed.</param>
    IEnumerator StepThroughDialogue(DialogueObject dialogueObject)
    {
        if (dialogueObject != null)
        {
            for (int i = 0; i < dialogueObject.DialogueInfo.Length; i++)
            {
                // Localize the string.
                string dialogue = dialogueObject.DialogueInfo[i].Dialogue;
                if (dialogueObject.TranslationTable != "None")
                {
                    dialogue = LocalizationHelper.GetLocalizedString(dialogueObject.TranslationTable, dialogueObject.DialogueInfo[i].LocaleName);
                    if (dialogue == string.Empty)
                    {
                        dialogue = dialogueObject.DialogueInfo[i].Dialogue;
                    }
                }

                yield return RunTypingEffect(dialogue, dialogueObject.DialogueInfo[i].PrintSpeed);

                merchantText.text = dialogue;

                if (i == dialogueObject.DialogueInfo.Length - 1 || !typewriterEffect.isRunning)
                {
                    break;
                }

                yield return null;
            }
        }
    }

    /// <summary>
    /// Runs the TypewriterEffect for the printing of the item tooltip dialogue.
    /// </summary>
    /// <param string name="dialogue">Holds the item tooltip dialogue to be printed.</param>
    IEnumerator RunTypingEffect(string dialogue, float printSpeed = 50.0f)
    {
        typewriterEffect.Run(dialogue, merchantText, printSpeed, DialogueCharacters.merchant);

        while (typewriterEffect.isRunning)
        {
            yield return null;
        }

        if (merchantImage)
        {
            merchantImage.sprite = merchantDefault;
        }
    }

    #endregion
}
