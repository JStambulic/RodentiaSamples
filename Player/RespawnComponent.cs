using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles checkpoints and respawning after death for the Player.
/// </summary>
public class RespawnComponent : MonoBehaviour
{
    #region Member Variables

    Vector3 respawnPosition = Vector3.zero;
    string currentCheckpoint = "";

    [SerializeField] Vector3 respawnOffset = new Vector3(0.0f, 1.0f, 0.0f);

    bool isColliding = false;

    public Vector3 RespawnPosition => respawnPosition;
    public string CurrentCheckpoint => currentCheckpoint;

    #endregion

    #region Start

    void Start()
    {
        // Set initial checkpoint position to be Player spawn.
        //if (respawnPosition == Vector3.zero)
        //{
        //    respawnPosition = transform.position;
        //}
        //if (currentCheckpoint == "")
        //{
        //    currentCheckpoint = "LevelBegin";
        //}

        //if (!SaveManager.CheckForSaveFile())
        //{
        //    if (GameManager.Get().SceneData.sceneData.uniqueSceneName == "forestLevel")
        //    {
        //        if (!GameManager.Get().IsSaving)
        //        {
        //            GameManager.Get().SaveGameAsync();
        //        }
        //    }
        //}

        // Listens for OnPlayerDeath Event and runs function when Invoked.
        GetComponent<PlayerStatus>().OnPlayerDeath.AddListener(PlayerDeath);
    }

    private void OnDestroy()
    {
        GetComponent<PlayerStatus>().OnPlayerDeath.RemoveListener(PlayerDeath);
    }

    #endregion

    #region Trigger Enter Check

    /// <summary>
    /// Checks a collided GameObject. IF player, check collider tag to check whether it was a Death plane or Checkpoint.
    /// </summary>
    /// <param Collider name="other"> Collided trigger box. </param>
    void OnTriggerEnter(Collider other)
    {
        if (gameObject.activeSelf)
        {
            if (isColliding)
            {
                return;
            }

            if (other.CompareTag("Death"))
            {
                isColliding = true;

                if (AudioManager.instance != null)
                    AudioManager.PlaySound(SFXType.BinkWaterDeath, true);

                PlayerDeath();
            }
            if (other.CompareTag("Checkpoint"))
            {
                var checkpoint = other.GetComponent<Checkpoint>();
                if (checkpoint.CheckpointName != currentCheckpoint)
                {
                    //Debug.Log("Checkpoint Reached.");
                    isColliding = true;

                    currentCheckpoint = checkpoint.CheckpointName;
                    respawnPosition = checkpoint.RespawnPoint.position + respawnOffset;

                    if (!GameManager.Get().IsSaving)
                    {
                        GameManager.Get().SaveGameAsync();
                    }
                }
            }

            StartCoroutine(ResetColliding());
        }
    }

    /// <summary>
    /// Waits until the end of the frame before Respawn Comp can collide with a trigger box again. 
    /// This is done to prevent trigger code from being called more than once on a given trigger.
    /// </summary>
    IEnumerator ResetColliding()
    {
        yield return new WaitForEndOfFrame();

        isColliding = false;
    }

    #endregion

    #region Death Event Listeners

    /// <summary>
    /// Handles respawning the player after death. 
    /// Loads the game from previous state.
    /// </summary>
    void PlayerDeath()
    {
        //Debug.Log("Player Died. Respawn.");

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<PlayerMovement>().ModifyGravityScale(0.0f);

        GameManager.Get().GetUIManager().HidePlayerUI();
        GetComponent<PlayerInput>().currentActionMap.Disable();

        if (!SaveManager.CheckForSaveFile() ||
            GameManager.Get().SceneData.sceneData.sceneIndex > 2)
        {
            GameManager.Get().LoadMainMenu();
        }
        else if (!GameManager.Get().IsLoading)
        {
            GameManager.Get().LoadGameAsync();
        }
    }

    #endregion

    #region Save and Load

    public void Save(ref PlayerRespawnData data)
    {
        data._respawnPosition = new float[3];
        data._respawnPosition[0] = respawnPosition.x;
        data._respawnPosition[1] = respawnPosition.y;
        data._respawnPosition[2] = respawnPosition.z;

        data._respawnRotation = new float[3];
        data._respawnRotation[0] = transform.rotation.eulerAngles.x;
        data._respawnRotation[1] = transform.rotation.eulerAngles.y;
        data._respawnRotation[2] = transform.rotation.eulerAngles.z;

        data._currentCheckpoint = currentCheckpoint;
    }

    public void Load(PlayerRespawnData data)
    {
        Vector3 position;
        position.x = data._respawnPosition[0];
        position.y = data._respawnPosition[1];
        position.z = data._respawnPosition[2];
        respawnPosition = position;

        currentCheckpoint = data._currentCheckpoint;

        transform.position = respawnPosition;
        transform.rotation = Quaternion.Euler(data._respawnRotation[0], data._respawnRotation[1], data._respawnRotation[2]);
    }

    #endregion

#if UNITY_EDITOR

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) { PlayerDeath(); }
    }

#endif
}

[System.Serializable]
public struct PlayerRespawnData
{
    // Current Checkpoint/Respawn
    public float[] _respawnPosition;
    public float[] _respawnRotation;
    public string _currentCheckpoint;
}
