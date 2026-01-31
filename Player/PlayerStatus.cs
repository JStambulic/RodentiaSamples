using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

/// <summary>
/// Contains the current status of the Player; Health and Energy.
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    #region Member Variables

    [Header("Health Values")]
    [SerializeField] int health;
    [SerializeField] int maxHealth = 100;
    [SerializeField] bool isDead = false;
    bool isInvincible = false;
    const float playerIFramesTime = 0.2f;

    [Header("Energy Values")]
    [SerializeField] float energy = 0.0f;
    [SerializeField] float maxEnergy = 100.0f;
    float energyRegenRate = 10.0f;
    bool isEnergyRegenerating = true;
    bool isEnergyInfinite = false;

    [SerializeField] VisualEffect VFXFur;
    [SerializeField] Animator animator;

    private AbilityManager abilityManager;
    private HUDEffectsHandler hudEffectsHandler;

    // Event invoked when health is <= 0. Will run all Listeners.
    UnityEvent onPlayerDeath = new UnityEvent();
    public UnityEvent OnPlayerDeath => onPlayerDeath;

    public UnityEvent onPlayerHealthChanged = new UnityEvent();
    public UnityEvent onPlayerEnergyChanged = new UnityEvent();

    public float Energy => energy;
    public float MaxEnergy => maxEnergy;
    public float EnergyRegenRate => energyRegenRate;

    public int Health => health;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    #endregion

    #region Awake, Start, FixedUpdate

    void Awake()
    {
        health = maxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        abilityManager = GetComponent<AbilityManager>();

        hudEffectsHandler = GameManager.Get().GetUIManager().GetPlayerUIComp().HUDEffects;
        if (hudEffectsHandler != null) { hudEffectsHandler.UpdateHurtVisual(health); }

        onPlayerEnergyChanged.AddListener(EnergyRegenGain);
    }

    private void OnDestroy()
    {
        onPlayerEnergyChanged.RemoveListener(EnergyRegenGain);
    }

    #endregion

    #region Modify Health

    /// <summary>
    /// Adds a passed in value to the Player's health value. If it results in health being <= 0, invoke PlayerDeath event.
    /// </summary>
    /// <param float name="healthChange">Passed in health value to add/subtract from Player energy.</param>
    public virtual void ModifyHealth(int healthChange, bool isEntity = false)
    {
        // IF invincibility is active, do not drain any health.
        if (((healthChange < 0 && isInvincible) || isDead) && isEntity == false)
        {
            return;
        }

        if (healthChange < 0 &&
            (GetComponent<TailComponent>().isGrappling || GetComponent<TailComponent>().isPulling))
        {
            GetComponent<TailComponent>().Detach();
        }

        //Debug.Log("Health Change: " + healthChange.ToString());

        health += healthChange;
        health = Mathf.Clamp(health, 0, maxHealth);

        hudEffectsHandler.UpdateHurtVisual(health);
        onPlayerHealthChanged?.Invoke();

        if (health <= 0)
        {
            isDead = true;

            animator.SetBool("IsDead", true);

            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<PlayerMovement>().ModifyGravityScale(0.0f);

            GameManager.Get().GetUIManager().HidePlayerUI();
            GetComponent<PlayerInput>().currentActionMap.Disable();

            if (AudioManager.instance != null)
                AudioManager.PlaySound(SFXType.BinkDeath, true);

            StartCoroutine(QueuePlayerDeath(1.5f));
        }
        else if (healthChange < 0)
        {
            AudioManager.PlaySound(SFXType.EntityHit, true);
            AudioManager.PlaySound(SFXType.BinkHurtVoice, true);

            //Play damage effect
            if (VFXFur)
            {
                VFXEventAttribute EventSettings = VFXFur.CreateVFXEventAttribute();
                VFXFur.SendEvent("Burst", EventSettings);
            }

            // Stop all coroutines before calling the StartCoroutine to ensure no other IFrame coroutines are currently running.
            StopAllCoroutines();
            StartCoroutine(PlayerIFrames());
        }
    }

    IEnumerator QueuePlayerDeath(float time)
    {
        yield return new WaitForSeconds(time);
        animator.SetBool("IsDead", false);
        onPlayerDeath?.Invoke();
    }

    /// <summary>
    /// For use in Unit Testing.
    /// </summary>
    /// <returns>Current Health value.</returns>
    public virtual int GetHealth()
    {
        return health;
    }

    #endregion

    #region Modify Energy

    /// <summary>
    /// Drains / Increases Energy depending on weapon equipped.
    /// </summary>
    private void EnergyRegenGain()
    {
        if (energy <= 0.0f)
        {
            abilityManager?.QuickResetUI();
        }

        //if (!isEnergyRegenerating || (isEnergyInfinite && energyRegenRate < 0) || energy.Equals(maxEnergy))
        //{
        //    return;
        //}

        //energy += energyRegenRate * Time.fixedDeltaTime;
        //energy = Mathf.Clamp(energy, 0, maxEnergy);

        //onPlayerEnergyChanged?.Invoke();
    }

    /// <summary>
    /// Adds a passed in value to the Player's energy value.
    /// </summary>
    /// <param float name="energyChange">Passed in energy value to add/subtract from Player energy.</param>
    public virtual void ModifyEnergy(float energyChange)
    {
        //Debug.Log("energy usage: " + energyChange);

        // IF an energy boost is active, do not lose any energy.
        if (energyChange < 0 && isEnergyInfinite)
        {
            return;
        }

        if (energy <= maxEnergy)
        {
            energy += energyChange;
            energy = Mathf.Clamp(energy, 0, maxEnergy);
        }

        onPlayerEnergyChanged?.Invoke();
    }

    /// <summary>
    /// For use in Unit Testing.
    /// </summary>
    /// <returns>Current Energy value.</returns>
    public virtual float GetEnergy()
    {
        return energy;
    }

    /// <summary>
    /// Manually change the energy regen rate.
    /// </summary>
    /// <param name="energyRegen">Rate at which energy will now regenerate.</param>
    public void ModifyEnergyRegenRate(float energyRegen)
    {
        energyRegenRate = energyRegen;
    }

    /// <summary>
    /// Toggles energy regen.
    /// </summary>
    /// <param name="enabled">Whether energy regen should be true or false.</param>
    public void ToggleRegen(bool enabled)
    {
        isEnergyRegenerating = enabled;
    }

    #endregion

    #region IFrames and Boosts

    /// <summary>
    /// Gives player temporary invincibility after taking damage.
    /// </summary>
    IEnumerator PlayerIFrames()
    {
        isInvincible = true;
        animator.SetBool("TookDamage", true);

        yield return new WaitForSeconds(playerIFramesTime);

        animator.SetBool("TookDamage", false);
        isInvincible = false;
    }

    /// <summary>
    /// Sets Player invincible
    /// </summary>
    /// <param name="state">State to set invincibility.</param>
    public void SetInvincible(bool state)
    {
        isInvincible = state;
    }

    /// <summary>
    /// Temporarily boosts health to max and makes the player invincible.
    /// </summary>
    /// <param name="time">How long the invincibility lasts.</param>
    /// <returns></returns>
    public void HealthBoost(float time)
    {
        StartCoroutine(ApplyHealthBoost(time));
        StartCoroutine(hudEffectsHandler.EnableHealthBoostVisual(time));
    }

    IEnumerator ApplyHealthBoost(float time)
    {
        ModifyHealth(maxHealth);

        isInvincible = true;

        yield return new WaitForSeconds(time);

        isInvincible = false;
    }

    /// <summary>
    /// Temporarily boosts energy to max and stops all drain from weapons.
    /// </summary>
    /// <param name="time">How long the infinite energy lasts.</param>
    /// <returns></returns>
    public void EnergyBoost(float time)
    {
        StartCoroutine(ApplyEnergyBoost(time));
        StartCoroutine(hudEffectsHandler.EnableEnergyBoostVisual(time));

        onPlayerEnergyChanged?.Invoke();
    }

    IEnumerator ApplyEnergyBoost(float time)
    {
        energy = maxEnergy;

        isEnergyInfinite = true;

        yield return new WaitForSeconds(time);

        isEnergyInfinite = false;
    }

    #endregion

    /// <summary>
    /// Resets health to max, and energy to 0.
    /// </summary>
    public void ResetStatus()
    {
        health = maxHealth;
        energy = 0;
    }

    #region Save and Load

    public void Save(ref PlayerStatusData data)
    {
        data._health = health;
        data._maxHealth = maxHealth;

        data._energy = energy;
        data._maxEnergy = maxEnergy;
        data._energyRegenRate = energyRegenRate;
    }

    public void Load(PlayerStatusData data)
    {
        health = data._health;
        maxHealth = data._maxHealth;

        energy = data._energy;
        maxEnergy = data._maxEnergy;
        energyRegenRate = data._energyRegenRate;

        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    #endregion
}

[System.Serializable]
public struct PlayerStatusData
{
    // Health
    public int _health;
    public int _maxHealth;

    // Energy
    public float _energy;
    public float _maxEnergy;
    public float _energyRegenRate;
}