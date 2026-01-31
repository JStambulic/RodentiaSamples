using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] audioClips;
    public AudioClip[] AudioClips => audioClips;
}

/// 
/// !DO NOT ADD TO ANYWHERE BUT TO THE BOTTOM OF THE LIST!
/// Adding anything in the middle will cause every single enum with an array below the new one to shift down one!
/// 
/// This will mean having to manually go in and reset every affected enum.
/// Heed this warning!
/// 
public enum MusicType
{
    Menu,
    Forest,
    Cave,
    Castle,
    Fight,
    Entity,
}

/// 
/// !DO NOT ADD TO ANYWHERE BUT TO THE BOTTOM OF THE LIST!
/// Adding anything in the middle will cause every single enum with an array below the new one to shift down one!
///
/// This will mean having to manually go in and reset every affected enum.
/// Heed this warning!
/// 
public enum AmbienceType
{
    Forest,
    Birds,
    Wind,
    Crickets,

    Cave,
    CaveWater,
    CaveWind,

    IntenseWind,

    Castle,
}

/// 
/// !DO NOT ADD TO ANYWHERE BUT TO THE BOTTOM OF THE LIST!
/// Adding anything in the middle will cause every single enum with an array below the new one to shift down one!
/// i.e. Every sound setup in BinkRun will now be in BinkJump, BinkWallSlide will now have all of BinkJump sounds, etc.
/// 
/// This will mean having to manually go in and reset every affected enum.
/// Heed this warning!
/// 
public enum SFXType
{
    // Binkus
    BinkSwordSwing,
    BinkStaffSwing,
    BinkAxeSwing,
    BinkHammerSwing,
    BinkWeaponSwap,
    BinkAttackVoice,
    BinkRun,
    BinkJump,
    BinkWallSlide,
    BinkDash,
    BinkTailAttach,
    BinkTailDetach,
    BinkHurtVoice,
    BinkDeath,
    BinkWaterDeath,
    ConsumableUse,
    ConsumableSwap,

    // Interactables
    ConsumablePickup,
    CollectiblePickup,

    // Spawners
    SpawnerDirt,
    SpawnerRock,

    // Barrels
    WoodHit,
    Explosion,

    // User Interface
    MenuHovered,
    MenuPressed,
    SliderDrag,
    MerchantHover,
    MerchantNoCash,
    MerchantPurchase,
    Interact,

    // Misc
    BookRead,
    BookPlace,
    BookPickup,
    KeyPickup,

    // Attack
    Throw,
    SwordHit,
    HammerHit,
    StaffHit,
    SwordChargeReady,
    HammerSpecial,
    StaffSpecial,
    EntityHit,
    WeaponSwing,

    // Enemies
    MediumHiss,
    MediumAngry,
    MediumDeath,
    LightAngry,
    LightDeath,
    HeavyAngry,
    HeavyDeath,
    LightHurt,
    MediumHurt, 
    HeavyHurt,
    HeavyAoE,
    CapyHurt,
    OwlHurt,
    OwlAttack,
    OwlScreech,
    OwlJump,
    OwlSlamdown,
    OwlDeath,
    HeavyCharge,

    Entity_Static,
}

/// 
/// !DO NOT ADD TO ANYWHERE BUT TO THE BOTTOM OF THE LIST!
/// Adding anything in the middle will cause every single enum with an array below the new one to shift down one!
/// 
/// This will mean having to manually go in and reset every affected enum.
/// Heed this warning!
/// 
public enum FoleyType
{
    BinkRun,
}

/// 
/// !DO NOT ADD TO ANYWHERE BUT TO THE BOTTOM OF THE LIST!
/// Adding anything in the middle will cause every single enum with an array below the new one to shift down one!
/// 
/// This will mean having to manually go in and reset every affected enum.
/// Heed this warning!
/// 
public enum DialogueType
{
    Bink,
    Unknown,
    Merchant,
    MerchantHivemind,
    UnknownAlt,
    Punctuation,
    Calibur,
    Entity_Window,
    Entity_00,
    Entity_01,
    Entity_02,
    Entity_03,
    Entity_04,
    Entity_05,
    Entity_06,
    Entity_07,
    Entity_08,
    Entity_09,
    Entity_10,
    Entity_11,
    Entity_12,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public SoundList[] music, amb, sfx, foley, dia;

    public AudioSource musicSource, ambSource, sfxSource, foleySource, diaSource;

    string currentSceneName = "Null";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Subscribe to the sceneLoaded event.
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unsubscribe to the sceneLoaded event.
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(MusicType));
        Array.Resize(ref music, names.Length);
        for (int i = 0; i < music.Length; i++)
            music[i].name = names[i];

        names = Enum.GetNames(typeof(AmbienceType));
        Array.Resize(ref amb, names.Length);
        for (int i = 0; i < amb.Length; i++)
            amb[i].name = names[i];

        names = Enum.GetNames(typeof(SFXType));
        Array.Resize(ref sfx, names.Length);
        for (int i = 0; i < sfx.Length; i++)
            sfx[i].name = names[i];

        names = Enum.GetNames(typeof(FoleyType));
        Array.Resize(ref foley, names.Length);
        for (int i = 0; i < foley.Length; i++)
            foley[i].name = names[i];

        names = Enum.GetNames(typeof(DialogueType));
        Array.Resize(ref dia, names.Length);
        for (int i = 0; i < dia.Length; i++)
            dia[i].name = names[i];
    }

    // On Scene Loaded, Spawn an Event System
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // We're in the same level, do not reset music. Only effects.
        if (currentSceneName == scene.name)
        {
            StopAllEffects();
        }
        else
        {
            StopAllAudio();

            StartMusicAmbience();

            currentSceneName = scene.name;
        }
    }

    public void StartMusicAmbience()
    {
        var currentScene = GameManager.Get().SceneData.sceneData;

        if (currentScene != null)
        {
            if (currentScene.uniqueSceneName == "castleLevel")
            {
                PlaySound(MusicType.Castle, false, true);
                PlaySound(AmbienceType.Castle, false, true);
            }
            if (currentScene.uniqueSceneName == "forestLevel")
            {
                PlaySound(MusicType.Forest, false, true);
                PlaySound(AmbienceType.Forest, false, true);
                PlaySound(AmbienceType.Wind, true, true);
                PlaySound(AmbienceType.Crickets, true, true);
            }
            if (currentScene.uniqueSceneName == "mainMenu")
            {
                PlaySound(MusicType.Menu, false, true);
                PlaySound(AmbienceType.Birds, false, true);
                PlaySound(AmbienceType.Wind, true, true);
            }
        }
    }

    public void SwitchAreaMusic(int music)
    {
        // If not valid gameplay area music, do not play.
        if (music <= 0 || music > (int)MusicType.Fight) { return; }

        // Do not restart if this music is already playing.
        if (instance.music[music].AudioClips[0] == instance.musicSource.clip) { return; }

        StopAllMusicAmbience();

        switch (music)
        {
            case (int)MusicType.Forest:
                PlaySound(MusicType.Forest, false, true);
                PlaySound(AmbienceType.Forest, false, true);
                PlaySound(AmbienceType.Wind, true, true);
                PlaySound(AmbienceType.Crickets, true, true);
                break;

            case (int)MusicType.Cave:
                PlaySound(MusicType.Cave, false, true);
                PlaySound(AmbienceType.Cave, false, true);
                PlaySound(AmbienceType.CaveWater, true, true);
                PlaySound(AmbienceType.CaveWind, true, true);
                break;

            case (int)MusicType.Castle:
                PlaySound(MusicType.Castle, false, true);
                PlaySound(AmbienceType.Castle, false, true);
                break;

            case (int)MusicType.Fight:
                PlaySound(MusicType.Fight, false, true);
                PlaySound(AmbienceType.IntenseWind, false, true);
                break;

            default: break;
        }
    }

    public void StopAllMusicAmbience()
    {
        musicSource.Stop();
        musicSource.clip = null;
        ambSource.Stop();
        ambSource.clip = null;
    }

    public void StopAllAudio()
    {
        musicSource.Stop();
        musicSource.clip = null;
        StopAllEffects();
    }

    public void StopAllEffects()
    {
        ambSource.Stop();
        ambSource.clip = null;
        sfxSource.Stop();
        sfxSource.clip = null;
        foleySource.Stop();
        foleySource.clip = null;
    }

    static public void PlaySound(MusicType musicType, bool isOneShot = false, bool shouldLoop = false, float volume = 1.0f)
    {
        if (instance == null)
            return;

        AudioClip[] clips = instance.music[(int)musicType].AudioClips;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (isOneShot == true)
        {
            instance.musicSource.PlayOneShot(randomClip, volume);
        }
        else
        {
            instance.musicSource.clip = randomClip;
            instance.musicSource.loop = shouldLoop;
            instance.musicSource.Play();
        }
    }

    static public void PlaySound(AmbienceType ambienceType, bool isOneShot = false, bool shouldLoop = false, float volume = 1.0f)
    {
        if (instance == null)
            return;

        AudioClip[] clips = instance.amb[(int)ambienceType].AudioClips;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (isOneShot == true)
        {
            instance.ambSource.PlayOneShot(randomClip, volume);
        }
        else
        {
            instance.ambSource.clip = randomClip;
            instance.ambSource.loop = shouldLoop;
            instance.ambSource.Play();
        }
    }

    static public void PlaySound(SFXType sfxType, bool isOneShot = false, bool shouldLoop = false, float volume = 1.0f)
    {
        if (instance == null)
            return;

        AudioClip[] clips = instance.sfx[(int)sfxType].AudioClips;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (isOneShot == true)
        {
            instance.sfxSource.PlayOneShot(randomClip, volume);
        }
        else
        {
            instance.sfxSource.clip = randomClip;
            instance.sfxSource.loop = shouldLoop;
            instance.sfxSource.Play();
        }
    }

    static public void PlaySound(FoleyType foleyType, bool isOneShot = false, bool shouldLoop = false, float volume = 1.0f)
    {
        if (instance == null)
            return;

        AudioClip[] clips = instance.foley[(int)foleyType].AudioClips;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (isOneShot == true)
        {
            instance.foleySource.PlayOneShot(randomClip, volume);
        }
        else
        {
            instance.foleySource.clip = randomClip;
            instance.foleySource.loop = shouldLoop;
            instance.foleySource.Play();
        }
    }

    static public void PlaySound(DialogueType dialogueType, bool isOneShot = false, bool shouldLoop = false, float volume = 1.0f)
    {
        if (instance == null)
            return;

        AudioClip[] clips = instance.dia[(int)dialogueType].AudioClips;
        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];

        if (isOneShot == true)
        {
            instance.diaSource.PlayOneShot(randomClip, volume);
        }
        else
        {
            instance.diaSource.clip = randomClip;
            instance.diaSource.loop = shouldLoop;
            instance.diaSource.Play();
        }
    }
}
