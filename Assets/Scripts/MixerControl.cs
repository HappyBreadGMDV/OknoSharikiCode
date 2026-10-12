using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MixerControl : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedParameterName = "MusicVol";
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        // Загружаем сохранённое значение (или 0.75 по умолчанию)
        float savedVolume = PlayerPrefs.GetFloat(exposedParameterName, 0.75f);
        volumeSlider.value = savedVolume;

        SetVolume(savedVolume);

        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float sliderValue)
    {
        if (sliderValue <= 0.0001f)
        {
            audioMixer.SetFloat(exposedParameterName, -80f);
        }
        else
        {
            float decibelValue = Mathf.Log10(sliderValue) * 20f;
            audioMixer.SetFloat(exposedParameterName, decibelValue);
        }

        // Сохраняем под тем же ключом, что и читаем
        PlayerPrefs.SetFloat(exposedParameterName, sliderValue);
        PlayerPrefs.Save();   // гарантированная запись на диск (особенно в редакторе)
    }
}