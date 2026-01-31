using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handler class for additional Player HUD effects.
/// </summary>
public class HUDEffectsHandler : MonoBehaviour
{
    #region Member Variables

    [Header("Boost Effects")]
    [SerializeField] GameObject healthBoostVisual;
    [SerializeField] GameObject energyBoostVisual;
    [SerializeField] GameObject speedBoostVisual;

    [Header("Damage Effect")]
    [SerializeField] Image hurtVisual;
    Color hurtBaseColor;
    Color hurtDamagedColor;

    [Header("Health Effect")]
    [SerializeField] Image healingVisual;

    #endregion

    private void Start()
    {
        hurtBaseColor = hurtVisual.color;
        hurtDamagedColor = hurtBaseColor;
    }

    #region Boosts

    /// <summary>
    /// Enables the health boost UI effect for a time.
    /// </summary>
    /// <param name="t">Time to remain active.</param>
    /// <returns></returns>
    public IEnumerator EnableHealthBoostVisual(float t)
    {
        healthBoostVisual.SetActive(true);
        yield return new WaitForSeconds(t);
        healthBoostVisual.SetActive(false);
    }

    /// <summary>
    /// Enables the energy boost UI effect for a time.
    /// </summary>
    /// <param name="t">Time to remain in effect.</param>
    /// <returns></returns>
    public IEnumerator EnableEnergyBoostVisual(float t)
    {
        energyBoostVisual.SetActive(true);
        yield return new WaitForSeconds(t);
        energyBoostVisual.SetActive(false);
    }

    /// <summary>
    /// Enables the speed boost UI effect for a time.
    /// </summary>
    /// <param name="t">Time to remain in effect.</param>
    /// <returns></returns>
    public IEnumerator EnableSpeedBoostVisual(float t)
    {
        speedBoostVisual.SetActive(true);
        yield return new WaitForSeconds(t);
        speedBoostVisual.SetActive(false);
    }

    #endregion

    #region Damage

    /// <summary>
    /// Updates the hurt UI visual based on remaining Player health.
    /// </summary>
    /// <param name="healthLeft">Health the Player currently has.</param>
    public void UpdateHurtVisual(int healthLeft)
    {
        /// Only enable if Health is less than 50.
        if (healthLeft > 50) 
        { 
            hurtVisual.color = hurtBaseColor;
            hurtVisual.gameObject.SetActive(false); 
            return; 
        }
        else
        {
            hurtVisual.gameObject.SetActive(true);

            /// Modify the alpha of the screen damage effect based on health remaining.
            hurtDamagedColor.a = ( 255 - ( healthLeft * 4 ) ) / 255.0f;

            hurtVisual.color = hurtDamagedColor;
        }
    }

    #endregion

    #region

    /// <summary>
    /// Updates the health visual to be enabled or disabled on screen.
    /// </summary>
    /// <param name="state">State the UI element should be.</param>
    public void UpdateHealingVisual(bool state)
    {
        if (state)
        {
            healingVisual.gameObject.SetActive(true);
        }
        else
        {
            healingVisual.gameObject.SetActive(false);
        }
    }

    #endregion
}
