using System.Collections;
using Tools;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Collectible : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] CollectibleObject collectibleToDisplay;
    [SerializeField] RawImage collectibleImage;
    string textToDisplay = "???";

    [SerializeField] CollectiblesUI collectiblesUIRef;

    GameObject renderObjectRef = null;
    RotateObject renderObjectRotate;
    Quaternion startRot;

    public void OnSelect(BaseEventData eventData)
    {
        collectiblesUIRef.ReadCollectibleDescription(textToDisplay);

        if (renderObjectRef)
        {
            renderObjectRotate.enabled = true;
        }

        StartCoroutine(PlaceCursor());
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (renderObjectRef)
        {
            renderObjectRef.transform.rotation = startRot;
            renderObjectRotate.enabled = false;
        }
    }

    private void Awake()
    {
        if (SaveManager.IsCollectibleObtained((int)collectibleToDisplay.Type))
        {
            collectibleImage.texture = collectibleToDisplay.CollectibleSprite;

            textToDisplay = LocalizationHelper.GetLocalizedString(
                collectibleToDisplay.Description.TranslationTable,
                collectibleToDisplay.Description.DialogueInfo[0].LocaleName
                );

            FindRenderObject();
        }
    }

    /// <summary>
    /// Moves the cursor of Collectibles UI to be on this UI object.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlaceCursor()
    {
        yield return new WaitForEndOfFrame();
        collectiblesUIRef.UpdateCursor(transform.position);
    }

    #region Render Object

    private void OnDisable()
    {
        if (renderObjectRef)
        {
            renderObjectRef.transform.rotation = startRot;
            renderObjectRotate.enabled = false;
        }
    }

    void OnDestroy()
    {
        if (renderObjectRef)
        {
            renderObjectRef.transform.rotation = startRot;
            renderObjectRotate.enabled = false;
        }
    }

    /// <summary>
    /// Finds the Render Object in the scene to affect it's rotation.
    /// </summary>
    void FindRenderObject()
    {
        switch (collectibleToDisplay.Type)
        {
            case CollectiblesType.AcornMug:
                renderObjectRef = GameObject.Find("Acorn");
                break;

            case CollectiblesType.Teacup:
                renderObjectRef = GameObject.Find("Teacup");
                break;

            case CollectiblesType.BarrelMug:
                renderObjectRef = GameObject.Find("Barrel");
                break;

            case CollectiblesType.Bottle:
                renderObjectRef = GameObject.Find("Bottle");
                break;

            case CollectiblesType.Carton:
                renderObjectRef = GameObject.Find("Carton");
                break;

            case CollectiblesType.CoffeeCup:
                renderObjectRef = GameObject.Find("Coffee");
                break;

            default: break;
        }

        if (renderObjectRef)
        {
            renderObjectRotate = renderObjectRef.GetComponent<RotateObject>();
            startRot = renderObjectRef.transform.rotation;
        }
    }

    #endregion
}
