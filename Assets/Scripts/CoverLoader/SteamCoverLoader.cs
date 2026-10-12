using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class SteamCoverLoader : MonoBehaviour
{
    [Header("Google Apps Script Proxy (опционально)")]
    public string proxyBaseUrl = "";  // если пусто, грузим напрямую

    public void LoadSteamHeader(int appid, Action<Texture2D> onLoaded)
    {
        StartCoroutine(LoadTextureCoroutine(appid, onLoaded));
    }

    private IEnumerator LoadTextureCoroutine(int appid, Action<Texture2D> onLoaded)
    {
        string directUrl = $"https://shared.fastly.steamstatic.com/store_item_assets/steam/apps/{appid}/header.jpg";

        string requestUrl;
        if (!string.IsNullOrEmpty(proxyBaseUrl))
            requestUrl = proxyBaseUrl + Uri.EscapeDataString(directUrl);
        else
            requestUrl = directUrl;

        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(requestUrl))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(req);
                onLoaded?.Invoke(tex);
            }
            else
            {
                Debug.LogError($"Ошибка загрузки обложки Steam (appid {appid}): {req.error} | {req.responseCode}");
                onLoaded?.Invoke(null);
            }
        }
    }
}