using UnityEngine;

/// <summary>Interface class for UI scripts to inherit from. Contains a getter for their UIType.</summary>
public interface IBaseUI
{
    UIType type { get; }

    public GameObject startSelection { get; }
}
