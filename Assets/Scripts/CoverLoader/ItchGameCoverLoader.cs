using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

public class ItchGameCoverLoader : MonoBehaviour
{
    [Header("Google Apps Script Proxy")]
    public string proxyBaseUrl = "https://script.google.com/macros/s/ВАШ_ID/exec?url=";

    /// <summary>
    /// Загружает обложку игры по URL её страницы на itch.io.
    /// gameUrl: https://username.itch.io/game-slug
    /// </summary>
    public void LoadCover(string gameUrl, Action<Texture2D> onLoaded)
    {
        StartCoroutine(LoadCoverCoroutine(gameUrl, onLoaded));
    }

    private IEnumerator LoadCoverCoroutine(string gameUrl, Action<Texture2D> onLoaded)
    {
        if (string.IsNullOrEmpty(gameUrl))
        {
            onLoaded?.Invoke(null);
            yield break;
        }

        // Шаг 1: получаем data.json
        string dataUrl = gameUrl.TrimEnd('/') + "/data.json";
        string requestUrl = proxyBaseUrl + Uri.EscapeDataString(dataUrl);

        using (UnityWebRequest req = UnityWebRequest.Get(requestUrl))
        {
            req.SetRequestHeader("Accept", "application/json");
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Ошибка запроса data.json: {req.error} | Код: {req.responseCode}");
                onLoaded?.Invoke(null);
                yield break;
            }

            string json = req.downloadHandler.text;
            string coverUrl = ExtractCoverUrl(json);

            if (string.IsNullOrEmpty(coverUrl))
            {
                Debug.LogWarning("cover_image не найден в data.json");
                onLoaded?.Invoke(null);
                yield break;
            }

            // Шаг 2: загружаем саму картинку через прокси
            string coverRequestUrl = proxyBaseUrl + Uri.EscapeDataString(coverUrl);
            using (UnityWebRequest texReq = UnityWebRequestTexture.GetTexture(coverRequestUrl))
            {
                yield return texReq.SendWebRequest();

                if (texReq.result == UnityWebRequest.Result.Success)
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(texReq);
                    onLoaded?.Invoke(tex);
                }
                else
                {
                    Debug.LogError($"Ошибка загрузки обложки: {texReq.error} | {texReq.responseCode}");
                    onLoaded?.Invoke(null);
                }
            }
        }
    }

    private string ExtractCoverUrl(string json)
    {
        // Ищем "cover_image":"https:\/\/..."
        Match match = Regex.Match(json, "\"cover_image\"\\s*:\\s*\"(.*?)\"");
        if (match.Success)
        {
            // Заменяем экранированные слэши \/ на обычные /
            string url = match.Groups[1].Value.Replace("\\/", "/");
            return url;
        }
        return null;
    }
}