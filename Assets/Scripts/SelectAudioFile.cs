using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SelectAudioFile : MonoBehaviour
{
    public AudioSource audioSource;        // используется только в редакторе/Standalone
    public TextMeshProUGUI fileNameText;

    public void OpenFileBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval("openFileDialog()");
#else
        string[] paths = SFB.StandaloneFileBrowser.OpenFilePanel("Open File", "",
            new SFB.ExtensionFilter[] {
                new SFB.ExtensionFilter("Sound Files", "mp3", "wav", "ogg"),
                new SFB.ExtensionFilter("All Files", "*")
            }, false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string filePath = paths[0];
            string fileName = System.IO.Path.GetFileName(filePath);
            if (fileNameText != null) fileNameText.text = fileName;
            StartCoroutine(LoadAudio("file://" + filePath));
        }
#endif
    }

    public void OnFileSelected(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            Debug.Log("Выбор файла отменён.");
            return;
        }

        // message – просто имя файла
        string fileName = message;
        if (fileNameText != null)
            fileNameText.text = fileName;

        Debug.Log("Выбран файл (будет играть после команды StartRound): " + fileName);
    }

#if !UNITY_WEBGL || UNITY_EDITOR
    private IEnumerator LoadAudio(string url)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                //audioSource.Play();
            }
            else
            {
                Debug.LogError("Ошибка загрузки аудио: " + www.error);
            }
        }
    }
#endif
}