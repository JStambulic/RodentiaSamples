using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles and updates the player UI logic from the player components.
/// </summary>
public class PlayerUI : MonoBehaviour, IBaseUI
{
    #region Member Variables

    // UI Type
    public UIType type => UIType.Player;

    public GameObject startSelection => null;

    GameObject playerRef;
    PlayerInventory inv;
    PlayerStatus status;

    [SerializeField] HUDEffectsHandler hudEffects;
    public HUDEffectsHandler HUDEffects => hudEffects;

    [Header("Interaction")]
    [SerializeField] Image interactIcon;
    public Image InteractIcon => interactIcon;

    [Header("Targeting")]
    [SerializeField] Image targetIcon;
    public Image TargetIcon => targetIcon;
    [SerializeField] GameObject[] bars;

    [Header("Tail Mode")]
    [SerializeField] GameObject tailInteraction;
    public GameObject TailInteraction => tailInteraction;

    [Header("Status")]
    [SerializeField] Image healthBar;
    [SerializeField] Image energyBar;

    [Header("Currency")]
    [SerializeField] GameObject currencyHUD;
    [SerializeField] TMP_Text currencyAmount;
    float hideCurrencyCounter = 0.0f;
    const float timeUntilHide = 3.0f;
    bool shouldCurrencyHide = true;
    public bool ShouldCurrencyHide => shouldCurrencyHide;

    [Header("Consumables")]
    [SerializeField] Image consumableItem;
    [SerializeField] TMP_Text consumableAmount;
    [SerializeField] Sprite defaultConsumableIcon;
    [SerializeField] Animator consumablesCooldown;
    bool isConsumableEmpty = true;

    #endregion

    #region Awake, OnEnable, Start, Update, OnDestroy
    private void Awake()
    {
        playerRef = GameManager.Get().GetPlayer();
        if (playerRef)
        {
            status = playerRef.GetComponent<PlayerStatus>();
            inv = playerRef.GetComponent<PlayerInventory>();
        }
    }
    private void OnEnable()
    {
        UpdateConsumableItem();

        if (currencyHUD.activeSelf)
        {
            StartCoroutine(HideCurrency());
        }
    }


    private void Start()
    {
        if (status)
        {
            healthBar.fillAmount = (float)status.Health / status.MaxHealth;
            energyBar.fillAmount = (float)status.Energy / status.MaxEnergy;
        }

        if (playerRef)
        {
            status.onPlayerHealthChanged.AddListener(UpdateHealth);
            status.onPlayerEnergyChanged.AddListener(UpdateEnergy);

            inv.onConsumablesChanged.AddListener(UpdateConsumableItem);
        }

        UpdateConsumableItem();
    }

    // Update is called once per frame. Update necessary UI components here.
    void Update()
    {
        if (currencyHUD)
        {
            if (currencyHUD.activeSelf)
            {
                if (currencyAmount)
                {
                    currencyAmount.text = playerRef.GetComponent<CurrencyManager>().Currency.ToString();
                }

                // IF currency HUD is enabled, increase timer until it will disable itself again.
                hideCurrencyCounter += Time.deltaTime;
                if (shouldCurrencyHide && hideCurrencyCounter >= timeUntilHide)
                {
                    StartCoroutine(HideCurrency());
                }
            }
        }

        if (inv)
        {
            if (!inv.CanUseConsumable)
            {
                consumablesCooldown.SetBool("onCooldown", true);
            }
            else
            {
                consumablesCooldown.SetBool("onCooldown", false);
            }
        }
    }

    private void OnDestroy()
    {
        if (playerRef)
        {
            status.onPlayerHealthChanged.RemoveListener(UpdateHealth);
            status.onPlayerEnergyChanged.RemoveListener(UpdateEnergy);

            inv.onConsumablesChanged.RemoveListener(UpdateConsumableItem);
        }
    }
    #endregion

    #region Update Health and Energy
    /// <summary>
    /// Updates the health bar's fill amount based on the current health divided by the max health.
    /// </summary>
    void UpdateHealth()
    {
        if (healthBar)
        {
            healthBar.fillAmount = (float)status.Health / status.MaxHealth;
        }
    }

    /// <summary>
    /// Updates the energy bar's fill amount based on the current energy divided by the max energy.
    /// </summary>
    void UpdateEnergy()
    {
        if (energyBar)
        {
            energyBar.fillAmount = (float)status.Energy / status.MaxEnergy;
        }
    }
    #endregion

    #region Currency

    /// <summary>
    /// Runs fade out animation after a few seconds, lets it play, then sets the currency HUD element to inactive.
    /// </summary>
    IEnumerator HideCurrency()
    {
        currencyHUD.GetComponent<Animator>().SetBool("isFading", true);
        yield return new WaitForSeconds(1.0f);
        currencyHUD.GetComponent<Animator>().SetBool("isFading", false);
        currencyHUD.SetActive(false);
    }

    /// <summary>
    /// Enables the currency HUD.
    /// </summary>
    public void ShowCurrency(bool state)
    {
        currencyHUD.SetActive(state);
        // Reset the hide counter, so if many currency collectables are being picked up, it won't begin fading until all are gone.
        hideCurrencyCounter = 0.0f;
        currencyHUD.GetComponent<Animator>().SetBool("isFading", false);
    }

    #endregion

    #region Cinematic Bars

    /// <summary>
    /// Resets the state of the cinematic bars.
    /// </summary>
    public void ResetBars()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].GetComponent<Animator>().SetBool("isTargetLocked", false);
            bars[i].GetComponent<Animator>().Play("Off");
        }
    }

    /// <summary>
    /// Shows the cinematic bars.
    /// </summary>
    public void ShowBars()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].GetComponent<Animator>().SetBool("isTargetLocked", true);
        }
    }

    /// <summary>
    /// Hides the cinematic bars.
    /// </summary>
    public void HideBars()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].GetComponent<Animator>().SetBool("isTargetLocked", false);
        }
    }

    #endregion

    #region Consumables

    /// <summary>
    /// Updates the UI showing the player's consumables.
    /// </summary>
    void UpdateConsumableItem()
    {
        if (inv)
        {
            if (inv.CurrentConsumable != null)
            {
                if (isConsumableEmpty && consumableAmount != null)
                {
                    consumableAmount.enabled = true;
                }

                if (consumableItem)
                {
                    consumableItem.sprite = inv.CurrentConsumable.ConsumableIcon;
                }
                if (consumableAmount)
                {
                    consumableAmount.text = inv.CurrentConsumableAmount.ToString();
                }
            }
            else
            {
                if (consumableItem)
                {
                    consumableItem.sprite = defaultConsumableIcon;
                }
                if (consumableAmount)
                {
                    consumableAmount.enabled = false;
                }

                isConsumableEmpty = true;
            }
        }
    }

    #endregion
}
