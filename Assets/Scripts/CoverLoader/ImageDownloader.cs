using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ImageDownloader : MonoBehaviour
{
    [Tooltip("URL вашего Google Apps Script")]
    public string webAppUrl = "https://script.google.com/macros/s/AKfycbw6BsvFh3S11aahpdOy0KSt9XyFF8771-hPMQ6b8jBYqKIxTYt5KkPqjjJf0DYMWF8/exec";

    public Sprite detalutImage;    // спрайт по умолчанию
    public Image uiImage;          // UI Image для отображения
    public Renderer targetRenderer; // 3D-рендерер (если нужно)

    /// <summary>
    /// Загружает изображение через Google Apps Script и вызывает колбэк с текстурой.
    /// </summary>
    public IEnumerator DownloadImage(string imageUrl, Action<Texture2D> onComplete = null)
    {
        if (string.IsNullOrEmpty(imageUrl))
        {
            SetDefaultImage();
            onComplete?.Invoke(null);
            yield break;
        }

        string fullUrl = $"{webAppUrl}?url={UnityWebRequest.EscapeURL(imageUrl)}";

        using (UnityWebRequest request = UnityWebRequest.Get(fullUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка запроса: " + request.error);
                SetDefaultImage();
                onComplete?.Invoke(null);
                yield break;
            }

            string response = request.downloadHandler.text.Trim();

            // Если ответ начинается с '{' — это JSON (например, {"success":false}), изображение отсутствует
            if (response.StartsWith("{") || response.StartsWith("["))
            {
                Debug.LogWarning("Обложка не получена: " + response);
                SetDefaultImage();
                onComplete?.Invoke(null);
                yield break;
            }

            // Пробуем декодировать base64
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(response);
            }
            catch (Exception e)
            {
                Debug.LogError("Ошибка декодирования base64: " + e.Message);
                SetDefaultImage();
                onComplete?.Invoke(null);
                yield break;
            }

            // Создаём текстуру
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(imageBytes))
            {
                ApplyTexture(tex);
                onComplete?.Invoke(tex);
            }
            else
            {
                Destroy(tex);
                Debug.LogError("Не удалось создать текстуру из base64");
                SetDefaultImage();
                onComplete?.Invoke(null);
            }
        }
    }

    private void ApplyTexture(Texture2D tex)
    {
        if (uiImage != null)
        {
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            uiImage.sprite = sprite;
        }
        if (targetRenderer != null)
        {
            targetRenderer.material.mainTexture = tex;
        }
    }

    private void SetDefaultImage()
    {
        if (uiImage != null && detalutImage != null)
        {
            uiImage.sprite = detalutImage;
        }
    }
}