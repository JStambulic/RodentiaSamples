using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tools;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Stores and maintains all Player items/collectibles.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    #region Member Variables

    public UnityEvent onConsumablesChanged = new UnityEvent();

    // Current Consumables.
    Dictionary<ConsumableObject, int> consumables = new Dictionary<ConsumableObject, int>();
    ConsumableObject currentConsumable;
    public ConsumableObject CurrentConsumable => currentConsumable;

    int currentConsumableAmount = 0;
    public int CurrentConsumableAmount => currentConsumableAmount;

    // Consumables Storage.
    public const int maxConsumableCount = 10;

    // For cooldown.
    public const float consumablesUseCooldown = 1.5f;
    bool canUseConsumable = true;
    public bool CanUseConsumable => canUseConsumable;

    [Header("VFX")]
    [SerializeField] GameObject VFXHeal;
    GameObject VFXHeal_instance;
    [SerializeField] GameObject VFXEnergy;
    GameObject VFXEnergy_instance;
    [SerializeField] GameObject VFXBoost;
    GameObject VFXBoost_instance;

    HUDItemPickup itemPickupHUD;
    StringBuilder sb = new StringBuilder();

    #endregion

    private void Start()
    {
        onConsumablesChanged.Invoke();

        itemPickupHUD = GameManager.Get().GetUIManager().GetPlayerUIObject().GetComponent<HUDItemPickup>();
    }

    #region Consumable Items - Add, Equip, Use

    /// <summary>
    /// Adds a new consumable item into the Inventory.
    /// If a consumable item isn't currently equipped, the new one will be automatically.
    /// </summary>
    /// <param name="newConsumable">Consumable to add to the inventory.</param>
    /// <param name="quantity">The amount of that consumable to add.</param>
    public void AddConsumableObject(ConsumableObject newConsumable, int quantity = 1)
    {
        // IF the consumable type already exists in the dictionary...
        // ELSE add this new type to the dictionary with it's quantity.
        if (consumables.ContainsKey(newConsumable))
        {
            if (consumables[newConsumable] + quantity > maxConsumableCount)
            {
                consumables[newConsumable] = maxConsumableCount;
            }
            else
            {
                consumables[newConsumable] += quantity;
            }
        }
        else
        {
            consumables.Add(newConsumable, quantity);
        }

        if (itemPickupHUD)
        {
            sb.Append("+ 1 ").Append(LocalizationHelper.GetLocalizedString("MerchantMenuTable", newConsumable.LocaleName));
            itemPickupHUD.NewConsumablePickup(sb.ToString());
            sb.Clear();
        }

        //Debug.Log("+ 1 " + newConsumable.ToString());

        // IF a consumable isn't equipped, equip this new one.
        if (currentConsumable == null)
        {
            currentConsumable = newConsumable;
        }
        // Update the displayed count and invoke the onConsumablesChanged event for Player HUD.
        if (currentConsumable == newConsumable)
        {
            UpdateCount();
            onConsumablesChanged.Invoke();
        }
    }

    /// <summary>
    /// Changes the currently equipped consumable to one that is passed in, if it exists in the dictionary.
    /// </summary>
    /// <param name="newEquippedConsumable">The consumable to equip.</param>
    public void ChangeCurrentConsumable(ConsumableObject newEquippedConsumable)
    {
        if (consumables.ContainsKey(newEquippedConsumable))
        {
            currentConsumable = newEquippedConsumable;
            UpdateCount();

            //Debug.Log("Current Consumable: " + currentConsumable.ToString() + " | Quantity: " + consumables[currentConsumable]);
            onConsumablesChanged.Invoke();
        }
    }

    /// <summary>
    /// Specific for Player Input.
    /// Cycles through the currently stored consumables.
    /// </summary>
    /// <param name="context">The event data from the Input.</param>
    public void CycleConsumablesInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canUseConsumable && consumables.Count > 0)
            {
                AudioManager.PlaySound(SFXType.ConsumableSwap, true);

                CycleCurrentConsumable();
            }
        }
    }

    /// <summary>
    /// Cycles through the currently stored consumables and equips the next one in line.
    /// </summary>
    /// <returns>True if the dictionary is not empty and a new consumable is found.</returns>
    public bool CycleCurrentConsumable()
    {
        if (consumables.Count > 0)
        {
            int index = 0;
            foreach (var consumable in consumables.Keys)
            {
                if (consumable)
                {
                    if (currentConsumable == consumable)
                    {
                        break;
                    }
                    index++;
                }
            }

            if (++index >= consumables.Count)
            {
                index = 0;
            }

            currentConsumable = consumables.ElementAt(index).Key;
            UpdateCount();

            //Debug.Log("Current Consumable: " + currentConsumable.ToString() + " | Quantity: " + consumables[currentConsumable]);

            onConsumablesChanged.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Uses the currently equipped consumable, applying it's effect to the Player.
    /// Automatically cycles to the next consumable if use brings the quantity to 0.
    /// </summary>
    /// <param name="context"></param>
    public void UseCurrentConsumable(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (currentConsumable != null && canUseConsumable)
            {
                PlayVFX();

                AudioManager.PlaySound(SFXType.ConsumableUse, true);

                // Subtract 1 from current consumable. Apply effect.
                consumables[currentConsumable] -= 1;

                currentConsumable.ApplyEffect(gameObject);

                UpdateCount();

                //Debug.Log("Current Consumable: " + currentConsumable.ToString() + " | Quantity: " + consumables[currentConsumable]);

                // If use has resulted in current consumable to become <= 0 in quantity, remove the item from the dictionary.
                if (consumables[currentConsumable] <= 0)
                {
                    consumables.Remove(currentConsumable);

                    // Cycle through available consumables to equip the next possible one. Otherwise, current consumable is now empty.
                    if (!CycleCurrentConsumable())
                    {
                        currentConsumable = null;
                    }
                }

                // Start a cooldown on consumables.
                StartCoroutine(ConsumablesUseCooldown());

                onConsumablesChanged.Invoke();
            }
        }
    }

    /// <summary>
    /// Plays the corresponding type VFX.
    /// </summary>
    private void PlayVFX()
    {
        switch (currentConsumable.Type)
        {
            case IConsumableBase.ConsumableType.EnergyRestore:
                if (VFXEnergy)
                {
                    VFXEnergy_instance = ObjectPoolManager.Get(VFXEnergy);
                    VFXEnergy_instance.transform.position = transform.position;
                    VFXEnergy_instance.transform.parent = transform;
                }
                break;

            case IConsumableBase.ConsumableType.TemporaryBoost:
                if (VFXBoost)
                {
                    VFXBoost_instance = ObjectPoolManager.Get(VFXBoost);
                    VFXBoost_instance.transform.position = transform.position;
                    VFXBoost_instance.transform.parent = transform;
                }
                break;

            case IConsumableBase.ConsumableType.HealthRestore:
            default:
                if (VFXHeal)
                {
                    VFXHeal_instance = ObjectPoolManager.Get(VFXHeal);
                    VFXHeal_instance.transform.position = transform.position;
                    VFXHeal_instance.transform.parent = transform;
                }
                break;
        }
    }

    /// <summary>
    /// Prevents spamming of consumables by applying a short cooldown on how quickly they can be used.
    /// </summary>
    /// <returns>IEnumerator</returns>
    IEnumerator ConsumablesUseCooldown()
    {
        canUseConsumable = false;

        yield return new WaitForSeconds(consumablesUseCooldown);

        if (VFXHeal)
        {
            if (VFXHeal_instance)
            {
                VFXHeal_instance.SetActive(false);
                VFXHeal_instance = null;
            }
        }

        canUseConsumable = true;
    }

    /// <summary>
    /// Updates consumable count on the UI.
    /// </summary>
    void UpdateCount()
    {
        currentConsumableAmount = consumables[currentConsumable];
    }

    #endregion

    #region Save and Load

    public void Save(ref PlayerInventoryData data)
    {
        data._consumables = new string[consumables.Count];
        data._numConsumables = new int[consumables.Count];
        for (int i = 0; i < consumables.Count; i++)
        {
            data._consumables[i] = consumables.ElementAt(i).Key.LocaleName;
            data._numConsumables[i] = consumables.ElementAt(i).Value;
        }

        if (currentConsumable)
        {
            data._currentConsumable = currentConsumable.LocaleName;
            data.currentConsumableAmount = currentConsumableAmount;
        }
    }

    public void Load(PlayerInventoryData data)
    {
        var consumableObjects = GameManager.Get().ConsumablesList.ConsumableObjects;

        for (int i = 0; i < data._consumables.Length; i++)
        {
            foreach (var consumableItem in consumableObjects)
            {
                if (consumableItem.LocaleName == data._consumables[i])
                {
                    consumables[consumableItem] = data._numConsumables[i];

                    if (data._consumables[i] == data._currentConsumable)
                    {
                        currentConsumable = consumableItem;
                        currentConsumableAmount = data.currentConsumableAmount;
                    }
                }
            }
        }
    }

    #endregion
}

[System.Serializable]
public struct PlayerInventoryData
{
    public string[] _consumables;
    public int[] _numConsumables;

    public string _currentConsumable;
    public int currentConsumableAmount;
}
