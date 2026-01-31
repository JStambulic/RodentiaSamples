using UnityEngine;
using static IConsumableBase;

/// <summary>
/// Consumable Item Object to be held within the Player Inventory and used for temporary effects.
/// </summary>
[CreateAssetMenu(menuName = "Item/ConsumableObject", fileName = "New Consumable")]
public class ConsumableObject : ScriptableObject, IConsumableBase
{
    [HideInInspector] public ConsumableType Type;
    [HideInInspector] public ConsumableSubtype TypeSubtype;

    [SerializeField] string localeName;
    public string LocaleName => localeName;
    [SerializeField] Sprite consumableIcon;
    public Sprite ConsumableIcon => consumableIcon;

    public void ApplyEffect(GameObject player)
    {
        switch (Type)
        {
            case ConsumableType.HealthRestore:
                ApplyHealthRestore(player);
                break;

            case ConsumableType.EnergyRestore:
                ApplyEnergyRestore(player);
                break;

            case ConsumableType.TemporaryBoost:
                ApplyTemporaryBoost(player);
                break;

            default:
                break;
        }
    }

    void ApplyHealthRestore(GameObject player)
    {
        var health = player.GetComponent<PlayerStatus>();

        switch (TypeSubtype)
        {
            case ConsumableSubtype.SmallHealth:
                health.ModifyHealth(smallRestoreAmount);
                break;

            case ConsumableSubtype.MediumHealth:
                health.ModifyHealth(mediumRestoreAmount);
                break;

            case ConsumableSubtype.LargeHealth:
                health.ModifyHealth(largeRestoreAmount);
                break;

            default:
                break;
        }
    }

    void ApplyEnergyRestore(GameObject player)
    {
        var energy = player.GetComponent<PlayerStatus>();

        switch (TypeSubtype)
        {
            case ConsumableSubtype.SmallEnergy:
                energy.ModifyEnergy(smallRestoreAmount);
                break;

            case ConsumableSubtype.MediumEnergy:
                energy.ModifyEnergy(mediumRestoreAmount);
                break;

            case ConsumableSubtype.LargeEnergy:
                energy.ModifyEnergy(largeRestoreAmount);
                break;

            default:
                break;
        }
    }

    void ApplyTemporaryBoost(GameObject player)
    {
        var status = player.GetComponent<PlayerStatus>();

        switch (TypeSubtype)
        {
            case ConsumableSubtype.HealthBoost:
                status.HealthBoost(boostLength);
                break;

            case ConsumableSubtype.EnergyBoost:
                status.EnergyBoost(boostLength);
                break;

            case ConsumableSubtype.SpeedBoost:
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                movement.BoostSpeed(boostLength);
                break;

            default:
                break;
        }
    }
}
