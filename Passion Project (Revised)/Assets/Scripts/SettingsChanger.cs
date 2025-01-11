using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsChanger : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI settingText;
    [Tooltip("Which decimal place to convert into an integer (10 - tenths, 100 - hundredths, etc.")]
    [Min(0), SerializeField] private int moveDecimalPlaces = 10;
    [SerializeField] private string settingType = string.Empty;

    private void Start()
    {
        slider.value = PlayerPrefs.GetFloat(settingType, 1);
    }

    private void Update()
    {
        AdjustSettings();
    }

    private void AdjustSettings()
    {
        if (moveDecimalPlaces % 10 != 0)
        {
            throw new ArgumentException("Decimal place must match the decimal place value");
        }

        if (settingType == string.Empty)
        {
            throw new ArgumentException("Setting type requires a name.");
        }

        if (!slider.wholeNumbers)
        {
            slider.value = Mathf.Round(slider.value * moveDecimalPlaces) / moveDecimalPlaces;
        }

        PlayerPrefs.SetFloat(settingType, slider.value);
        settingText.text = $"{PlayerPrefs.GetFloat(settingType) * moveDecimalPlaces}";
    }

    public void AddSliderValue()
    {
        slider.value++;
    }

    public void DecreaseSliderValue()
    {
        slider.value--;
    }
}
