using System.Collections.Generic;
using UnityEngine;

public class ListGamesJam : MonoBehaviour
{
    public GameObject Prefab;
    public GameJamApi GamesScript;
    public List<GameObject> Prefabs = new List<GameObject>();

    private int lastCount = -1;
    private int idGameJam = -1;

    void FixedUpdate()
    {
        if (GamesScript.games.Count != lastCount || !GamesScript.enabled || GamesScript.GameJamId != idGameJam)
        {
            idGameJam = GamesScript.GameJamId;
            lastCount = GamesScript.games.Count;
            Restart();
        }
    }

    public void Restart()
    {
        foreach (var item in Prefabs)
        {
            if (item != null) Destroy(item);
        }
        Prefabs.Clear();

        for (int i = 0; i < GamesScript.games.Count; i++)
        {
            var pr = Instantiate(Prefab, transform);
            pr.SetActive(true);   // не забываем включить
            pr.GetComponent<Game>().Name = GamesScript.games[i].Name;
            pr.GetComponent<Game>().Url = GamesScript.games[i].Url;
            Prefabs.Add(pr);
        }
    }
}