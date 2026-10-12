using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SetValuveJam : MonoBehaviour
{
    [System.Serializable]
    public class SetValve
    {
        public string Option;
        public string SetValuve;
    }

    public TMP_InputField inputField;
    public TMP_Dropdown Options;
    public List<SetValve> setValves = new List<SetValve>();

    public void UpdateValuve()
    {
        foreach (var item in setValves)
        {
            if (Options.options[Options.value].text == item.Option)
            {
                inputField.text = item.SetValuve;
            }
        }
    }
}
