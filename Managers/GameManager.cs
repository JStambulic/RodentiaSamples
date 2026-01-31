using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Game Manager for holding references to all major systems/objects in the game.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// Creates a singleton of a GameManager Instance that can be accessed anywhere.
    /// </summary>
    #region Game Manager Instance (Singleton)

    private static GameManager gameManager_Instance; // = null
    private static bool gameManager_ShouldRun = true;

    public static GameManager Get()
    {
        if (gameManager_ShouldRun == false)
        {
            return null;
        }

        if (!gameManager_Instance)
        {
            // Spawns GameManager prefab and sets component reference.
            GameObject manager = Instantiate(Resources.Load<GameObject>("Managers/GameManager"));
        }

        return gameManager_Instance;
    }

    /// <summary>
    /// Disables and blocks use of the Game Manager. 
    /// *DANGEROUS. DO NOT USE WITHOUT PURPOSE*
    /// </summary>
    public static void DisableGameManager()
    {
        if (gameManager_Instance)
        {
            gameManager_Instance = null;
        }
        gameManager_ShouldRun = false;
    }

    /// <summary>
    /// Enables Game Manager.
    /// </summary>
    public static void EnableGameManager()
    {
        gameManager_ShouldRun = true;
    }

    #endregion

    #region Game Pause

    // Is Paused
    bool isPaused = false;
    public bool IsPaused => isPaused;

    bool canPause = true;
    public bool CanPause => canPause;

    public void PauseGame()
    {
        // Set game TimeScale to 0. Is Paused.
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        // Set game TimeScale back to 1. Is not paused.
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    #endregion

    #region Awake

    void Awake()
    {
        Debug.LogError("Test error, ignore this and have fun! :)");

        if (gameManager_Instance == null)
        {
            gameManager_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Cursor.lockState = CursorLockMode.Confined;

        SettingsManager.ApplyGameSettings();

        // Creates Event System for UI Input.
        GetEventSystem();
        GetUIManager();
        GetPlayer();
    }

    private void Start()
    {
        UpdateActions();
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

    // On Scene Loaded, Spawn an Event System
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GetEventSystem();
        GetUIManager();
        GetPlayer();
    }

    #endregion

    #region Scene Management

    public SceneData SceneData { get; set; }
    public SceneLoadManager SceneLoadManager { get; set; }
    public ProgressionManager ProgressionManager { get; set; }

    public CheckpointManager CheckpointManager { get; set; }

    public static void SaveThisObject(IProgressionObject obj)
    {
        Get().ProgressionManager.SaveMe(obj);
    }

    public static void UnsaveThisObject(IProgressionObject obj)
    {
        Get().ProgressionManager.UnsaveMe(obj);
    }

    #endregion

    #region UI Manager

    private UIManager UIManagerCachedRef;

    /// <summary>
    /// If there is not already a cached UIManager, create one and return it.
    /// </summary>
    /// <returns> Created UIManager Reference. </returns>
    public UIManager GetUIManager()
    {
        if (!UIManagerCachedRef)
        {
            // Spawns UIManager prefab and sets component reference.
            GameObject manager = Instantiate(Resources.Load<GameObject>("Managers/UIManager"));
            UIManagerCachedRef = manager.GetComponent<UIManager>();
        }

        return UIManagerCachedRef;
    }

    #endregion

    #region Cutscenes

    public CutscenesManager CutscenesManager { get; set; }
    public TutorialManager TutorialManager { get; set; }

    bool isInCutscene = false;
    public bool IsInCutscene => isInCutscene;

    bool isInDialogue = false;
    public bool IsInDialogue => isInDialogue;

    public void StartCutscene(StoryCutscene cs, bool pauseGame = true, bool isUnskippable = false, float timeBeforeBegin = 0.0f)
    {
        if (CutscenesManager)
        {
            if (timeBeforeBegin > 0.0f)
            {
                ShowScreenFadeOut();
                StartCoroutine(BeginCutsceneAfterTime(cs, pauseGame, isUnskippable, timeBeforeBegin));
            }
            else
            {
                var csMenu = GetUIManager().CreateCutsceneMenu();
                csMenu.StartCutscene(CutscenesManager.CutsceneDataObjects[(int)cs], pauseGame, isUnskippable);
            }
        }
    }

    IEnumerator BeginCutsceneAfterTime(StoryCutscene cs, bool pauseGame = true, bool isUnskippable = false, float timeBeforeBegin = 1.0f)
    {
        yield return new WaitForSeconds(timeBeforeBegin);

        var csMenu = GetUIManager().CreateCutsceneMenu();
        csMenu.StartCutscene(CutscenesManager.CutsceneDataObjects[(int)cs], pauseGame, isUnskippable);
    }

    public void EnterCutscene()
    {
        if (playerInputCachedRef)
        {
            playerInputCachedRef.SwitchCurrentActionMap("Cutscene");
            UIManagerCachedRef.HidePlayerUI();
            PlayerStatus.SetInvincible(true);
            if (UIManagerCachedRef.IsActive(UIType.Tutorial))
            {
                UIManagerCachedRef.HideTutorialUI();
            }
            isInCutscene = true;
        }
    }

    public void ExitCutscene()
    {
        if (isInCutscene && playerInputCachedRef)
        {
            playerInputCachedRef.SwitchCurrentActionMap("Player");
            UIManagerCachedRef.ShowPlayerUI();
            UIManagerCachedRef.GetPlayerUIComp().ResetBars();
            PlayerStatus.SetInvincible(false);
            if (UIManagerCachedRef.GetTutorialMenuComp())
            {
                UIManagerCachedRef.ShowTutorialUI();
            }
            isInCutscene = false;
        }
    }

    public void SetIsInDialogue(bool state)
    {
        isInDialogue = state;
    }

    #endregion

    #region Loading Screen

    [SerializeField] GameObject loadingUI;
    GameObject loadingUI_Instance;

    GameObject GetLoadingMenu()
    {
        if (!loadingUI_Instance)
        {
            loadingUI_Instance = Instantiate(loadingUI);
            DontDestroyOnLoad(loadingUI_Instance);
        }

        return loadingUI_Instance;
    }

    void ShowLoadingUI()
    {
        PlayerInputCachedRef.DeactivateInput();

        if (!loadingUI_Instance)
        {
            GetLoadingMenu();
        }

        canPause = false;
        loadingUI_Instance.SetActive(true);
    }

    IEnumerator HideLoadingUI()
    {
        if (loadingUI_Instance)
        {
            PlayerInputCachedRef.ActivateInput();

            loadingUI_Instance.GetComponent<Animator>().SetBool("isDisabling", true);
            yield return new WaitForSeconds(0.5f);

            Destroy(loadingUI_Instance);
            canPause = true;
        }
    }

    #endregion

    #region Screen Fade

    [SerializeField] GameObject fadeUI;
    GameObject fadeUI_Instance;

    GameObject GetScreenFadeMenu()
    {
        if (!fadeUI_Instance)
        {
            fadeUI_Instance = Instantiate(fadeUI);
            DontDestroyOnLoad(fadeUI_Instance);
        }

        return fadeUI_Instance;
    }

    /// <summary>
    /// Fades from gameplay to a black screen.
    /// </summary>
    public void ShowScreenFadeOut()
    {
        if (!fadeUI_Instance)
        {
            GetScreenFadeMenu();
        }

        canPause = false;
        fadeUI_Instance.SetActive(true);

        StartCoroutine(HideScreenFadeUI());
    }

    /// <summary>
    /// Fades from a black screen to gameplay.
    /// </summary>
    public void ShowScreenFadeIn()
    {
        if (!fadeUI_Instance)
        {
            GetScreenFadeMenu();
        }

        canPause = false;
        fadeUI_Instance.SetActive(true);

        fadeUI_Instance.GetComponent<Animator>().SetTrigger("FadeIn");

        StartCoroutine(HideScreenFadeUI());
    }

    IEnumerator HideScreenFadeUI()
    {
        if (fadeUI_Instance)
        {
            yield return new WaitForSeconds(1.0f);

            Destroy(fadeUI_Instance);
            canPause = true;
        }
    }

    #endregion

    #region Saving Indicator

    [SerializeField] GameObject savingUI;
    GameObject savingUI_Instance;

    GameObject GetSavingMenu()
    {
        if (!savingUI_Instance)
        {
            savingUI_Instance = Instantiate(savingUI);
            DontDestroyOnLoad(savingUI_Instance);
        }

        return savingUI_Instance;
    }

    void ShowSavingUI()
    {
        if (!savingUI_Instance)
        {
            GetSavingMenu();
        }
        canPause = false;
        savingUI_Instance.SetActive(true);
    }

    IEnumerator HideSavingUI()
    {
        if (savingUI_Instance)
        {
            yield return new WaitForSeconds(0.25f);
            if (savingUI_Instance)
            {
                savingUI_Instance.GetComponent<Animator>().SetBool("isDisabling", true);
            }
            yield return new WaitForSeconds(0.5f);

            Destroy(savingUI_Instance);
            canPause = true;
        }
    }

    #endregion

    #region Codex Indicator

    [SerializeField] GameObject codexUI;
    GameObject codexUI_Instance;

    GameObject GetCodexMenu()
    {
        if (!codexUI_Instance)
        {
            codexUI_Instance = Instantiate(codexUI);
            DontDestroyOnLoad(codexUI_Instance);
        }

        return codexUI_Instance;
    }

    public void ShowCodexUI()
    {
        if (!codexUI_Instance)
        {
            GetCodexMenu();
        }
        codexUI_Instance.SetActive(true);

        StartCoroutine(HideCodexUI());
    }

    IEnumerator HideCodexUI()
    {
        if (codexUI_Instance)
        {
            yield return new WaitForSeconds(1.5f);
            if (codexUI_Instance)
            {
                codexUI_Instance.GetComponent<Animator>().SetBool("isDisabling", true);
            }
            yield return new WaitForSeconds(0.5f);

            Destroy(codexUI_Instance);
        }
    }

    #endregion

    #region Player Related References

    private GameObject playerCachedRef;
    [SerializeField] private InputActionAsset inputActions;
    public InputActionAsset InputActionsAsset => inputActions;

    private PlayerInput playerInputCachedRef;
    public PlayerInput PlayerInputCachedRef => playerInputCachedRef;

    public ConsumablesList ConsumablesList { get; set; }

    /// <summary>
    /// If not already stored, checks the scene for the Player game object and stores it as a reference.
    /// </summary>
    /// <returns> GameObject of the Player, containing all the relevant scripts. </returns>
    public GameObject GetPlayer()
    {
        if (!playerCachedRef)
        {
            playerCachedRef = GameObject.Find("Player");
            if (playerCachedRef)
            {
                PlayerStatus = playerCachedRef.GetComponent<PlayerStatus>();
                RespawnComponent = playerCachedRef.GetComponent<RespawnComponent>();
                CurrencyManager = playerCachedRef.GetComponent<CurrencyManager>();
                Inventory = playerCachedRef.GetComponent<PlayerInventory>();
                PlayerMovement = playerCachedRef.GetComponent<PlayerMovement>();
                CombatManager = playerCachedRef.GetComponentInChildren<CombatManager>();
                AbilityManager = playerCachedRef.GetComponent<AbilityManager>();
                UnlockManager = playerCachedRef.GetComponent<UnlockManager>();
                CameraLock = playerCachedRef.transform.parent.GetComponentInChildren<CameraLocking>();
            }
        }

        return playerCachedRef;
    }

    public static GameObject Player()
    {
        return Get().GetPlayer();
    }

    public static void SetInput(PlayerInput input)
    {
        Get().playerInputCachedRef = input;
    }

    /// <summary>
    /// Sets the mouse cursor to be active or inactive, and changes lock state accordingly.
    /// </summary>
    public void SetMouseCursor(bool enabled)
    {
        // Always hide the cursor if gamepad is active device.
        if (playerInputCachedRef.currentControlScheme == "Gamepad")
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = enabled;
            Cursor.lockState = enabled ? CursorLockMode.Confined : CursorLockMode.Locked;
        }
    }

    public PlayerStatus PlayerStatus { get; private set; }
    public RespawnComponent RespawnComponent { get; private set; }
    public CurrencyManager CurrencyManager { get; private set; }
    public PlayerInventory Inventory { get; private set; }
    public CombatManager CombatManager { get; private set; }
    public AbilityManager AbilityManager { get; private set; }
    public UnlockManager UnlockManager { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public CameraLocking CameraLock { get; private set; }

    #endregion

    #region Action Dictionary

    Dictionary<string, string> actionsDictionary = new Dictionary<string, string>
    {
        { "Move", "<sprite name=w>" },
        { "Look", "<sprite name=w>" },
        { "Tutorials", "<sprite name=w>" },
        { "Attack", "<sprite name=w>" },
        { "Jump", "<sprite name=w>" },
        { "Dodge", "<sprite name=w>" },
        { "LockOn", "<sprite name=w>" },
        { "Interact", "<sprite name=w>" },
        { "UseItem", "<sprite name=w>" },
        { "CycleConsumables", "<sprite name=w>" },
        { "AbilityOne", "<sprite name=w>" },
        { "AbilityTwo", "<sprite name=w>" },
        { "AbilityThree", "<sprite name=w>" },
        { "AbilityFour", "<sprite name=w>" },
        { "ActivateTail", "<sprite name=w>" },
        { "ActivateSpecial", "<sprite name=w>" },
        { "ChangeTarget", "<sprite name=w>" },
    };

    public Dictionary<string, string> ActionsDictionary => actionsDictionary;

    /// <summary>
    /// Updates the actions dictionary upon control scheme change.
    /// </summary>
    public void UpdateActions()
    {
        if (playerInputCachedRef != null)
        {
            for (int i = 0; i < actionsDictionary.Count; i++)
            {
                var key = actionsDictionary.ElementAt(i).Key;
                actionsDictionary[key] = GetCurrentActionName(playerInputCachedRef.actions.FindAction(key.ToString()));
            }
        }
    }

    private StringBuilder builder = new StringBuilder();
    private List<string> controls = new List<string>();
    /// <summary>
    /// Gets the sprite name for an input action.
    /// </summary>
    /// <param name="curAction">Input action to use.</param>
    /// <returns>A string with TMP_text sprite formatting.</returns>
    public string GetCurrentActionName(InputAction curAction, int specificBinding = -1)
    {
        if (playerInputCachedRef == null) { return string.Empty; }

        controls.Clear();

        var bindingIndex = curAction.GetBindingIndex(group: playerInputCachedRef.currentControlScheme);
        if (specificBinding != -1)
        {
            bindingIndex = specificBinding;
        }

        if (curAction.bindings[bindingIndex].isPartOfComposite)
        {
            controls.Add(InputControlPath.ToHumanReadableString(curAction.bindings[bindingIndex].effectivePath,
                         InputControlPath.HumanReadableStringOptions.OmitDevice));

            for (var i = bindingIndex + 1; i < curAction.bindings.Count && curAction.bindings[i].isPartOfComposite; ++i)
            {
                controls.Add(InputControlPath.ToHumanReadableString(curAction.bindings[i].effectivePath,
                                                                    InputControlPath.HumanReadableStringOptions.OmitDevice));
            }
        }
        else
        {
            controls.Add(InputControlPath.ToHumanReadableString(curAction.bindings[bindingIndex].effectivePath,
                                                                InputControlPath.HumanReadableStringOptions.OmitDevice));
        }

        builder.Clear();
        foreach (string newControl in controls)
        {
            switch (newControl.ToLower())
            {
                case "0": builder.Append("<sprite name=zero>"); break;
                case "1": builder.Append("<sprite name=one>"); break;
                case "2": builder.Append("<sprite name=two>"); break;
                case "3": builder.Append("<sprite name=three>"); break;
                case "4": builder.Append("<sprite name=four>"); break;
                case "5": builder.Append("<sprite name=five>"); break;
                case "6": builder.Append("<sprite name=six>"); break;
                case "7": builder.Append("<sprite name=seven>"); break;
                case "8": builder.Append("<sprite name=eight>"); break;
                case "9": builder.Append("<sprite name=nine>"); break;
                case ".": builder.Append("<sprite name=period>"); break;
                case "-": builder.Append("<sprite name=minus>"); break;
                default: builder.Append("<sprite name=").Append(newControl.ToLower()).Append(">"); break;
            }
        }

        return builder.ToString();
    }

    #endregion

    #region Event System

    private GameObject eventSystemCachedRef;

    /// <summary>If there is not already a cached UIManager, create one and return it.</summary>
    /// <returns>Created UIManager Reference.</returns>
    public GameObject GetEventSystem()
    {
        if (!eventSystemCachedRef)
        {
            // If an EventSystem already exists in scene, use it instead.
            GameObject obj = GameObject.Find("EventSystem");
            if (obj)
            {
                eventSystemCachedRef = obj;
                return eventSystemCachedRef;
            }
            obj = GameObject.Find("EventSystem(Clone)");
            if (obj)
            {
                eventSystemCachedRef = obj;
                return eventSystemCachedRef;
            }

            // Spawns EventSystem prefab and sets object reference.
            GameObject manager = Instantiate(Resources.Load<GameObject>("Managers/EventSystem"));
            eventSystemCachedRef = manager;
        }

        return eventSystemCachedRef;
    }

    #endregion

    #region Save & Load

    private bool isSaving;
    public bool IsSaving => isSaving;
    private bool isLoading;
    public bool IsLoading => isLoading;

    public void LoadMainMenu()
    {
        ObjectPoolManager.ClearAllPools();
        ProgressionManager?.WipeProgressionManager();

        ShowLoadingUI();

        AsyncSceneLoad("mainMenu");
    }

    public void LoadScene(string sceneDataName)
    {
        ObjectPoolManager.ClearAllPools();
        ProgressionManager?.WipeProgressionManager();

        ShowLoadingUI();

        AsyncSceneLoad(sceneDataName);
    }

    private async void AsyncSceneLoad(string sceneDataName)
    {
        if (Application.isPlaying == false)
        {
            return; // Stop async method execution.
        }

        await Task.Delay(1000);

        await SceneLoadManager.LoadSceneByIndexAsync(sceneDataName);
        await SceneData.WaitForSceneLoad();

        ResumeGame();

        OnFinishLoad();
    }

    private void OnFinishLoad()
    {
        StartCoroutine(HideLoadingUI());
    }

    public async void SaveGameAsync()
    {
        isSaving = true;
        ShowSavingUI();
        await SaveManager.SaveAsyncronously();
        OnFinishSave();
        isSaving = false;
    }

    private void OnFinishSave()
    {
        StartCoroutine(HideSavingUI());
    }

    public async void LoadGameAsync()
    {
        isLoading = true;
        ObjectPoolManager.ClearAllPools();
        ProgressionManager?.WipeProgressionManager();
        await SaveManager.LoadAsync();
        isLoading = false;
    }

    public async void LoadNewLevelAsync(SceneDataObject sceneToLoad)
    {
        isLoading = true;
        ObjectPoolManager.ClearAllPools();
        ProgressionManager?.WipeProgressionManager();
        await SaveManager.LoadNewLevelAsync(sceneToLoad.uniqueSceneName);
        isLoading = false;
    }

    #endregion
}