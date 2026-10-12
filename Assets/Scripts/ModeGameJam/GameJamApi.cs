using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GameJamApi : MonoBehaviour
{
    public GameObject CirclePrefab;
    public Transform spawnParent;
    public float radius = 2f;
    public float startAngle = 0f;
    public GameObject LoadingScreen;
    public GameObject WinUi;
    public AudioSource audioSourceRound;
    public FollowLeader followLeaderScript;
    public int GameJamId;

    public List<GameInfo> games = new List<GameInfo>();
    public List<GameInfo> deletedGames = new List<GameInfo>();
    public List<GameObject> objectList = new List<GameObject>();

    private const string DELETED_KEY = "DeletedGamesJam";

    [System.Serializable]
    public class GameInfo
    {
        public string Name;
        public string Url;         // ссылка на itch.io страницу игры
        public string UrlOtcenka;  // ссылка на страницу оценки
    }

    // ---------- Сериализуемые классы под структуру itch.io ----------
    [System.Serializable]
    private class RootObject
    {
        public List<JamGameEntry> jam_games;
    }

    [System.Serializable]
    private class JamGameEntry
    {
        public string url;               // например "/jam/okno-summer-jam/rate/4876340"
        public int id;
        public int rating_count;
        public int coolness;
        public string created_at;
        public GameData game;
        // contributors не обязателен, игнорируем
    }

    [System.Serializable]
    private class GameData
    {
        public string title;
        public string url;               // ссылка на игру (itch.io)
        public string short_text;
        public string[] platforms;
        public UserData user;
        // cover, cover_color не нужны
    }

    [System.Serializable]
    private class UserData
    {
        public string name;
        public string url;
        public int id;
    }
    // ----------------------------------------------------------------

    void Start()
    {
        LoadDeletedGames();
        WinUi.SetActive(true);
        StartCoroutine(Fetch());
    }

    private void LoadDeletedGames()
    {
        // Ключ должен совпадать с тем, что используется при сохранении
        string saved = PlayerPrefs.GetString(DELETED_KEY + GameJamId, "");
        if (!string.IsNullOrEmpty(saved))
        {
            string[] names = saved.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string name in names)
                deletedGames.Add(new GameInfo { Name = name, Url = "" });
        }
    }

    private void SaveDeletedGames()
    {
        string joined = string.Join(";", deletedGames.Select(g => g.Name));
        // Используем тот же ключ, что и при загрузке
        PlayerPrefs.SetString(DELETED_KEY + GameJamId, joined);
        PlayerPrefs.Save();
    }

    public void StartRound()
    {
        if (!this.enabled) return;
        if (audioSourceRound != null && audioSourceRound.clip != null)
            audioSourceRound.Play();

#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval("playUploadedAudio()");
#endif

        foreach (var item in objectList)
        {
            if (item != null)
                item.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
    }

    public void Restart()
    {
        if (!this.enabled) return;
        foreach (var obj in objectList)
        {
            if (obj != null) Destroy(obj);
        }
        objectList.Clear();

        var deletedNames = new HashSet<string>(deletedGames.Where(d => d != null).Select(d => d.Name));
        games.RemoveAll(g => deletedNames.Contains(g.Name));

        if (games.Count == 0) return;

        for (int i = 0; i < games.Count; i++)
        {
            Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized;
            float randomRadius = UnityEngine.Random.Range(0, radius);
            Vector3 randomPos = transform.position + (Vector3)(randomCircle * randomRadius);
            randomPos.z = 0f;

            var newObj = Instantiate(CirclePrefab, randomPos, Quaternion.identity, spawnParent);
            var circle = newObj.GetComponent<GameCircle>();
            circle.Name = games[i].Name;
            circle.Url = games[i].Url;
            objectList.Add(newObj);
        }
    }

    public void Add(Game ButtonData)
    {
        if (!this.enabled) return;
        if (ButtonData == null) return;
        string gameName = ButtonData.Name;

        var gameToDelete = deletedGames.Find(g => g.Name == gameName);
        if (gameToDelete != null)
        {
            deletedGames.Remove(gameToDelete);
            games.Add(gameToDelete);
            SaveDeletedGames();
            Restart();
        }
    }

    public void Delete(TextMeshProUGUI TMP)
    {
        if (!this.enabled) return;
        string text = TMP.text;

        var gameToDelete = games.Find(g => g.Name == text);
        if (gameToDelete != null)
        {
            deletedGames.Add(gameToDelete);
        }
        else
        {
            deletedGames.Add(new GameInfo { Name = text, Url = "" });
        }

        SaveDeletedGames();
        games.RemoveAll(g => g.Name == text);

        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            var obj = objectList[i];
            if (obj != null && obj.GetComponent<GameCircle>()?.Name == text)
            {
                Destroy(obj);
                objectList.RemoveAt(i);
            }
        }
        Restart();
    }

    public void DeleteAll()
    {

        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            Destroy(objectList[i]);
        }
        objectList.Clear();
        Restart();
        if (followLeaderScript != null)
            followLeaderScript.RefreshPlayerList();
    }

    public void ResetGames()
    {
        if (!this.enabled) return;
        LoadingScreen.SetActive(true);
        StartCoroutine(Fetch());
    }

    IEnumerator Fetch()
    {
        LoadingScreen.SetActive(true);

        string targetUrl = $"https://itch.io/jam/{GameJamId}/entries.json";
        string proxyUrl = "https://script.google.com/macros/s/AKfycbzAEpbKPUWcm6x-mioGLZTNTIpUq6CBr5Y7dFvGq1Wt59KriOug-DL8plaszz-ANnZXog/exec?url="
                          + Uri.EscapeDataString(targetUrl);

        UnityWebRequest req = UnityWebRequest.Get(proxyUrl);
        req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        yield return req.SendWebRequest();

        Debug.Log("URL: " + proxyUrl);

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка запроса: " + req.error + " | Код: " + req.responseCode);
            req.Dispose();
            LoadingScreen.SetActive(false);
            yield break;
        }

        string json = req.downloadHandler.text;
        req.Dispose();

        RootObject data = null;
        try
        {
            data = JsonUtility.FromJson<RootObject>(json);
        }
        catch (Exception e)
        {
            Debug.LogError("Ошибка парсинга JSON: " + e.Message);
            LoadingScreen.SetActive(false);
            yield break;
        }

        if (data == null || data.jam_games == null)
        {
            Debug.LogError("Пустой ответ или неверный формат JSON");
            LoadingScreen.SetActive(false);
            yield break;
        }

        games.Clear();
        var deletedNames = new HashSet<string>(deletedGames.Where(d => d != null).Select(d => d.Name));

        foreach (var entry in data.jam_games)
        {
            if (entry == null || entry.game == null)
                continue;

            string name = entry.game.title;
            string gameUrl = entry.game.url;
            string reviewUrl = "https://itch.io" + entry.url; // полная ссылка на оценку

            if (deletedNames.Contains(name))
                continue;

            games.Add(new GameInfo
            {
                Name = name,
                Url = gameUrl,
                UrlOtcenka = reviewUrl
            });
        }

        Debug.Log($"Загружено игр: {games.Count}");
        Restart();
        LoadingScreen.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}