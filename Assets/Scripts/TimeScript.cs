using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeScript : MonoBehaviour
{
    public TMP_InputField inputField;
    public void SetTimeScale(float timaScale = 1f)
    {
        Time.timeScale = timaScale;
        inputField.text = timaScale.ToString();
    }

    public void SetTimeScaleInput(TMP_InputField input)
    {
        if (input && float.TryParse(input.text, out float value))
        {
            SetTimeScale(value);
        }
        else
        {
            SetTimeScale(1f);
        }
    }
}
