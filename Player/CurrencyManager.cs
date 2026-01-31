using UnityEngine;

/// <summary>
/// Manages all currency held by the Player.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    #region Member Variables

    [SerializeField] int currency = 0;
    [SerializeField] int maxCurrency = 500;

    PlayerUI playerUI;

    public int MaxCurrency => maxCurrency;
    public int Currency => currency;

    #endregion

    #region Start

    // Start is called before the first frame update
    void Start()
    {
        currency = Mathf.Clamp(currency, 0, maxCurrency);

        if (GameManager.Get())
        {
            playerUI = GameManager.Get().GetUIManager().GetPlayerUIComp();
        }
    }

    #endregion

    #region Functions

    /// <summary>
    /// Adds to the currency count from a given amount.
    /// Will show the currency UI for a time.
    /// </summary>
    /// <param name="currencyValue">Amount of currency to add.</param>
    public virtual void AddCurrency(int currencyValue)
    {
        currency += currencyValue;

        // Clamp new currency between 0 and maximum.
        currency = Mathf.Clamp(currency, 0, maxCurrency);

        //Debug.Log("got " + currencyValue + "gazillion dollars");

        playerUI?.ShowCurrency(true);
    }

    /// <summary>
    /// For use in Unit Testing.
    /// </summary>
    /// <returns>Current held currency.</returns>
    public virtual int GetCurrency()
    {
        return currency;
    }

    /// <summary>
    /// Decreases the total amount of currency when an item is purchased.
    /// </summary>
    /// <param name="priceOfItem">Price of the item bought.</param>
    public void ItemBought(int priceOfItem)
    {
        if (currency - priceOfItem < 0)
        {
            return;
        }

        currency -= priceOfItem;
    }

    #endregion

    #region Save and Load

    public void Save(ref PlayerCurrencyData data)
    {
        data._currency = currency;
        data._maxCurrency = maxCurrency;
    }

    public void Load(PlayerCurrencyData data)
    {
        currency = data._currency;
        maxCurrency = data._maxCurrency;
    }

    #endregion

#if UNITY_EDITOR

    private void Update()
    {
        // Max money for debug purposes.
        if (Input.GetKeyDown(KeyCode.M))
        {
            AddCurrency(maxCurrency);
        }
    }

#endif
}

[System.Serializable]
public struct PlayerCurrencyData
{
    // Currency
    public int _currency;
    public int _maxCurrency;
}
