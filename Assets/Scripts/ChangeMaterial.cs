using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private Image previewColor;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private string saveKey;

    private void Start()
    {
        if (string.IsNullOrEmpty(saveKey))
            saveKey = $"{gameObject.name}_Color";

        // Загружаем сохранённый цвет или берём текущий цвет материала как HEX по умолчанию
        string defaultHex = "#FFFFFF";
        if (material != null)
            defaultHex = "#" + ColorUtility.ToHtmlStringRGB(material.color);

        string savedColor = PlayerPrefs.GetString(saveKey, defaultHex);

        if (ColorUtility.TryParseHtmlString(savedColor, out Color color))
        {
            ApplyColor(color);
            if (inputField != null)
                inputField.text = savedColor;
        }
    }

    public void ChangeColorFromInput()
    {
        if (inputField == null) return;

        string hex = inputField.text;
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            ApplyColor(color);
            PlayerPrefs.SetString(saveKey, hex);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogWarning($"Некорректный HEX: {hex}");
            inputField.text = PlayerPrefs.GetString(saveKey, "#FFFFFF");
        }
    }

    private void ApplyColor(Color color)
    {
        if (material != null)
            material.color = color;
        if (previewColor != null)
            previewColor.color = color;
    }
}