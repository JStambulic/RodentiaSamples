using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDItemPickup : MonoBehaviour
{
    // Item UI
    [Header("Shopfront Items")]
    [SerializeField] RectTransform pickupText;
    [SerializeField] RectTransform pickupTextContainer;

    List<GameObject> items = new List<GameObject>();

    const float secondsBeforeFade = 3.0f;

    private void OnEnable()
    {
        if (items.Count > 0)
        {
            foreach (GameObject item in items)
            {
                StartCoroutine(FadeAwayItem(item));
            }
        }
    }

    /// <summary>
    /// Adds text for a newly picked up consumable item.
    /// </summary>
    /// <param name="itemName">Localized consumable object name.</param>
    public void NewConsumablePickup(string itemName)
    {
        GameObject newItem = Instantiate(pickupText.gameObject, pickupTextContainer);

        newItem.gameObject.SetActive(true);
        newItem.GetComponentInChildren<TMP_Text>().text = itemName;

        items.Add(newItem);

        if (gameObject.activeSelf)
        {
            StartCoroutine(FadeAwayItem(newItem));
        }
    }

    /// <summary>
    /// Waits for a time before triggering the fade animation, destroying the UI.
    /// </summary>
    /// <param name="go">UI to fade and destroy</param>
    /// <returns></returns>
    IEnumerator FadeAwayItem(GameObject go)
    {
        yield return new WaitForSeconds(secondsBeforeFade);

        go.GetComponent<Animator>().SetTrigger("Fade");

        yield return new WaitForSeconds(0.5f);

        if (items.Contains(go)) { items.Remove(go); }
        Destroy(go);
    }
}
