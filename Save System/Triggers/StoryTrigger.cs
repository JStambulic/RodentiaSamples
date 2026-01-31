using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

/// <summary>
/// A basic story trigger whose events can be saved to Progression Manager.
/// </summary>
public class StoryTrigger : GameEvent
{
    public override ProgressionObject objectType => ProgressionObject.storyTrigger;
    public bool shouldUseTriggerEnter = true;

    [Header("Animated Triggers")]
    [SerializeField] bool isTimelineAnimated = false;
    [SerializeField] float endTime = 0.0f;

    [Header("On Trigger & On Load")]
    public UnityEvent doOnTrigger = new UnityEvent();
    [Header("On Trigger Only Once")]
    public UnityEvent doOnlyOnce = new UnityEvent();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (shouldUseTriggerEnter)
        {
            if (other.CompareTag("Player") && !wasTriggered)
            {
                wasTriggered = true;

                GameManager.SaveThisObject(this);

                doOnTrigger.Invoke();
                doOnlyOnce.Invoke();
            }
        }
    }

    public void SetEventTriggered()
    {
        if (!wasTriggered)
        {
            wasTriggered = true;

            GameManager.SaveThisObject(this);

            doOnTrigger.Invoke();
            doOnlyOnce.Invoke();
        }
    }

    public override string SaveAction()
    {
        return base.SaveAction();
    }

    public override void LoadAction(string data)
    {
        base.LoadAction(data);

        if (wasTriggered)
        {
            if (isTimelineAnimated)
            {
                GetComponent<PlayableDirector>().initialTime = endTime;
            }

            doOnTrigger.Invoke();
        }
    }

#if UNITY_EDITOR

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (gameObject.name == "GrappleTree")
            {
                SetEventTriggered();
            }
        }
    }

#endif
}

///
/// Story Triggers
/// ~~~~~~~~~~~~~~
/// 
/// Opening cutscene viewed.
///
/// Capy Mini-boss defeated.
/// 
/// Capy dialogue change.
/// 
/// Cave Entrance cleared.
/// -> Full group defeated.
///
/// Cave Enter / Exit
/// -> For choosing which music is playing on load.
///
/// Number of cave groups defeated
/// -> For knowing when to spawn key.
/// 
/// Night time switch.
/// 
/// Fallen log.
/// 
/// Boss fight started.
/// 
/// Boss fight completed.
/// 