using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Script for better sliders, allowing for big steps through.
/// </summary>
[RequireComponent(typeof(Slider))]
public class InputSlider : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    Slider parentSlider;
    GameObject thisSlider;
    float minValue = 0.0f;
    float maxValue = 1.0f;
    float sliderRange;

    [SerializeField] float sliderStep = 0.0f;
    [SerializeField] float sliderBigStep = 0.0f;

    [SerializeField] GameObject UIHelper;

    InputAction navigateAction;
    InputAction stepAction;

    // Start is called before the first frame update
    void Start()
    {
        parentSlider = GetComponentInParent<Slider>();
        thisSlider = gameObject;

        navigateAction = GameManager.Get().PlayerInputCachedRef.actions.FindAction("Navigate");
        if (navigateAction != null) { navigateAction.started += SliderMove; }

        stepAction = GameManager.Get().PlayerInputCachedRef.actions.FindAction("Step");
        if (stepAction != null) { stepAction.performed += SliderStep; }

        if (parentSlider)
        {
            minValue = parentSlider.minValue;
            maxValue = parentSlider.maxValue;
            sliderRange = maxValue - minValue;
        }
    }

    /// <summary>
    /// Moves the slider quicker along the bar.
    /// </summary>
    /// <param name="context">Input context data.</param>
    void SliderMove(InputAction.CallbackContext context)
    {
        if (thisSlider == EventSystem.current.currentSelectedGameObject)
        {
            if (context.started)
            {
                Vector2 nav = context.ReadValue<Vector2>();

                if (Mathf.Abs(nav.x) > 0.0f)
                {
                    float sliderChange = Mathf.Sign(nav.x) * sliderRange / sliderStep;
                    float changedVal = parentSlider.value + sliderChange;
                    Mathf.Clamp(changedVal, minValue, maxValue);

                    parentSlider.value = changedVal;
                }
            }
        }
    }

    /// <summary>
    /// Allows for big steps to be made along the slider.
    /// </summary>
    /// <param name="context">Input context data.</param>
    void SliderStep(InputAction.CallbackContext context)
    {
        if (thisSlider == EventSystem.current.currentSelectedGameObject)
        {
            if (context.performed)
            {
                float action = context.ReadValue<float>();

                if (action > 0.0f)
                {
                    //Forward step.
                    parentSlider.value += sliderBigStep;
                }
                else
                {
                    // Backwards step.
                    parentSlider.value -= sliderBigStep;
                }
            }
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (UIHelper)
        {
            UIHelper.SetActive(true);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (UIHelper)
        {
            UIHelper.SetActive(false);
        }
    }
}
