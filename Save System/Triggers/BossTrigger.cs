using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Spawns in a boss event.
/// </summary>
public class BossTrigger : StoryTrigger
{
    public override ProgressionObject objectType => ProgressionObject.bossTrigger;

    [SerializeField] GameObject boss;
    GameObject boss_Instance;
    [SerializeField] GameObject spawnPoint;

    bool wasDefeated = false;
    [Header("Defeat Trigger")]
    public UnityEvent onBossDefeated = new UnityEvent();

    [Header("Defeat Trigger On Load")]
    public UnityEvent afterDefeatedOnLoad = new UnityEvent();

    [System.Serializable]
    class BossTriggerSaveData
    {
        public bool _wasTriggered;
        public bool _wasDefeated;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !wasDefeated && boss_Instance == null)
        {
            boss_Instance = Instantiate(boss, spawnPoint.transform.position, Quaternion.identity);
            // TODO Make sure to use finalized health comp of boss.
            boss_Instance.GetComponent<HealthBase>().OnDeathEvent.AddListener(BossDefeated);

            base.OnTriggerEnter(other);
        }
    }

    void BossDefeated()
    {
        wasDefeated = true;

        boss_Instance.GetComponent<HealthBase>().OnDeathEvent.RemoveListener(BossDefeated);
        boss_Instance = null;

        GameManager.SaveThisObject(this);

        onBossDefeated.Invoke();
        afterDefeatedOnLoad.Invoke();
    }

    public override string SaveAction()
    {
        BossTriggerSaveData data = new BossTriggerSaveData();
        data._wasTriggered = wasTriggered;
        data._wasDefeated = wasDefeated;

        return JsonUtility.ToJson(data);
    }

    public override void LoadAction(string data)
    {
        BossTriggerSaveData load = JsonUtility.FromJson<BossTriggerSaveData>(data);

        wasTriggered = load._wasTriggered;
        wasDefeated = load._wasDefeated;

        if (wasTriggered)
        {
            doOnTrigger.Invoke();
        }

        if (wasTriggered && wasDefeated)
        {
            afterDefeatedOnLoad.Invoke();
        }
    }
}
