using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Triggers a cutscene to play and saves that it has played.
/// Ensures it will not play again on load like StoryTrigger.
/// </summary>
public class CutsceneTrigger : GameEvent
{
    public override ProgressionObject objectType => ProgressionObject.cutscene;
    public bool shouldUseTriggerEnter = true;

    public UnityEvent doOnTrigger = new UnityEvent();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (shouldUseTriggerEnter)
        {
            if (other.CompareTag("Player") && !wasTriggered)
            {
                wasTriggered = true;

                GameManager.SaveThisObject(this);

                doOnTrigger.Invoke();
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
        }
    }

    public override string SaveAction()
    {
        return base.SaveAction();
    }

    public override void LoadAction(string data)
    {
        base.LoadAction(data);
    }
}
