using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class CoverLoader : MonoBehaviour
{
    [Header("Google Apps Script Proxy")]
    public string proxyBaseUrl = "https://script.google.com/macros/s/ВАШ_ID/exec";

    public void LoadCover(string imageUrl, Action<Texture2D> onLoaded)
    {
        StartCoroutine(LoadTextureBase64(imageUrl, onLoaded));
    }

    private IEnumerator LoadTextureBase64(string imageUrl, Action<Texture2D> onLoaded)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            onLoaded?.Invoke(null);
            yield break;
        }

        string proxyUrl = proxyBaseUrl + "?url=" + Uri.EscapeDataString(imageUrl) + "&mode=image";
        Debug.Log(proxyUrl);

        using (UnityWebRequest req = UnityWebRequest.Get(proxyUrl))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("Proxy request failed: " + req.responseCode);
                onLoaded?.Invoke(null);
                yield break;
            }

            string response = req.downloadHandler.text.Trim();

            // Если начинается с '{' — это JSON, изображения нет
            if (response.StartsWith("{") || response.StartsWith("["))
            {
                Debug.LogWarning("Обложка не получена (JSON): " + response);
                onLoaded?.Invoke(null);
                yield break;
            }

            // Пробуем декодировать base64
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(response);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Base64 decode error: " + e.Message);
                onLoaded?.Invoke(null);
                yield break;
            }

            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(bytes))
            {
                onLoaded?.Invoke(tex);
            }
            else
            {
                Destroy(tex);
                onLoaded?.Invoke(null);
            }
        }
    }
}