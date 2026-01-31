using UnityEngine;

public class ConsumablesList : MonoBehaviour
{
    [SerializeField] ConsumableObject[] consumableObjects;

    public ConsumableObject[] ConsumableObjects => consumableObjects;

    private void Awake()
    {
        GameManager.Get().ConsumablesList = this;
    }
}
