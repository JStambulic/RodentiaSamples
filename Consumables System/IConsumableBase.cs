using UnityEngine;
/// <summary>
/// Base abstract class for inventory items to inherit from.
/// </summary>
public interface IConsumableBase
{
    /// <summary>
    /// Enum for types of consumable items.
    /// </summary>
    public enum ConsumableType
    {
        HealthRestore,
        EnergyRestore,
        TemporaryBoost,
    }

    /// <summary>
    /// Enum for subtypes of consumable item types.
    /// </summary>
    public enum ConsumableSubtype
    {
        SmallHealth,
        MediumHealth,
        LargeHealth,

        SmallEnergy,
        MediumEnergy,
        LargeEnergy,

        HealthBoost,
        EnergyBoost,
        SpeedBoost,
    }

    protected const int smallRestoreAmount = 25;
    protected const int mediumRestoreAmount = 50;
    protected const int largeRestoreAmount = 80;

    protected const float boostLength = 10.0f;

    public void ApplyEffect(GameObject player);
}
