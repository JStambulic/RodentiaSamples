/// <summary>
/// Base Interface for all scene progression objects to inherit from.
/// Will be added to and saved/loaded by ProgressionManager.
/// </summary>
public interface IProgressionObject
{
    uint UniqueID { get; }

    bool isSaveable { get; }

    ProgressionObject objectType { get; }

    public abstract string SaveAction();

    public abstract void LoadAction(string data);
}

// 255 maximum. Don't use 0.
public enum ProgressionObject
{
    gameEvent = 1,
    door = 2,
    cutscene = 3,
    pickup = 4,
    enemySpawner = 5,
    enemy = 6,
    key = 7,
    hazard = 8,
    storyTrigger = 9,
    bossTrigger = 10,
    pedestal = 11,
    musicTrigger = 12,
}