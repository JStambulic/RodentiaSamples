using UnityEngine;
using UnityEngine.Events;

/// <summary>Allows custom events to be run when responses are chosen.</summary>
[System.Serializable]
public class ResponseEvent 
{
    [HideInInspector] public string name;
    [SerializeField] UnityEvent onPickedResponse;

    public UnityEvent OnPickedResponse => onPickedResponse;
}
