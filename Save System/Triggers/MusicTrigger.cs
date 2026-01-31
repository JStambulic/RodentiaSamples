using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField] MusicType areaMusicToPlay = MusicType.Forest;

    public bool willBeTriggered = true;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (willBeTriggered)
        {
            if (other.CompareTag("Player"))
            {
                AudioManager.instance.SwitchAreaMusic((int)areaMusicToPlay);

                willBeTriggered = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!willBeTriggered)
        {
            willBeTriggered = true;
        }
    }
}
