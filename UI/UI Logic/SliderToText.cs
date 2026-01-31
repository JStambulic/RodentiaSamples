using TMPro;
using Tools;
using UnityEngine;

public class SliderToText : MonoBehaviour
{
    [SerializeField] TMP_Text box;

    public void OnSliderValueChanged(float value)
    {
        box.text = value.ToString("#.00");
    }

    public void OnSliderValueChangedWholeNumber(float value)
    {
        box.text = value.ToString();
    }

    public void OnSliderValueChangedSatCon(float value)
    {
        value += 100;
        box.text = value.ToString();
    }

    public void OnSliderValueChangedVolume(float value)
    {
        box.text = VolumeHelper.LogarithmicVolToPercent(value).ToString();
    }
}
