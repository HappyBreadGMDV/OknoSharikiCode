using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class SuperSimpleFetcher : MonoBehaviour
{
    public GameObject CirclePrefab;
    public Transform spawnParent;
    public float radius = 2f;
    public float startAngle = 0f;
    public GameObject LoadingScreen;
    public GameObject WinUi;
    public AudioSource audioSourceRound;
    public FollowLeader followLeaderScript;

    public List<GameInfo> games = new List<GameInfo>();
    public List<GameInfo> deletedGames = new List<GameInfo>();
    public List<GameObject> objectList = new List<GameObject>();

    private const string DELETED_KEY = "DeletedGames";

    [System.Serializable]
    public class GameInfo
    {
        public string Name;
        public string Url;
        public string Author;
        public int Id;
    }

    [System.Serializable]
    private class Submission
    {
        public string id;
        public string status;
        public string name;
        public string link;
        public string author;
    }

    [System.Serializable]
    private class SubmissionListWrapper
    {
        public List<Submission> items;
    }

    void Start()
    {
        LoadDeletedGames();
        WinUi.SetActive(true);
        StartCoroutine(Fetch());
    }

    private void LoadDeletedGames()
    {
        string saved = PlayerPrefs.GetString(DELETED_KEY, "");
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
        PlayerPrefs.SetString(DELETED_KEY, joined);
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
            circle.Id = games[i].Id;
            circle.Author = games[i].Author;
            // circle.CoverUrl можно заполнить, если API предоставит, но пока оставляем null
            objectList.Add(newObj);
        }
        followLeaderScript.RefreshPlayerList();
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

        string targetUrl = "https://oknoweb.ru/api/submissions";
        string proxyUrl = "https://script.google.com/macros/s/AKfycbzAEpbKPUWcm6x-mioGLZTNTIpUq6CBr5Y7dFvGq1Wt59KriOug-DL8plaszz-ANnZXog/exec?url="
                          + Uri.EscapeDataString(targetUrl);

        UnityWebRequest req = UnityWebRequest.Get(proxyUrl);
        req.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Ошибка запроса: " + req.error + " | Код: " + req.responseCode);
            req.Dispose();
            LoadingScreen.SetActive(false);
            yield break;
        }

        string json = req.downloadHandler.text;
        req.Dispose();

        string wrappedJson = "{\"items\":" + json + "}";
        SubmissionListWrapper data = null;
        try
        {
            data = JsonUtility.FromJson<SubmissionListWrapper>(wrappedJson);
        }
        catch (Exception e)
        {
            Debug.LogError("Ошибка парсинга JSON: " + e.Message);
            LoadingScreen.SetActive(false);
            yield break;
        }

        if (data == null || data.items == null)
        {
            Debug.LogError("Пустой ответ или неверный формат JSON");
            LoadingScreen.SetActive(false);
            yield break;
        }

        games.Clear();
        var deletedNames = new HashSet<string>(deletedGames.Where(d => d != null).Select(d => d.Name));

        foreach (var item in data.items)
        {
            if (item == null || item.status == "reviewed") continue;
            if (deletedNames.Contains(item.name)) continue;

            games.Add(new GameInfo
            {
                Name = item.name,
                Url = item.link,
                Id = int.Parse(item.id),
                Author = item.author,
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