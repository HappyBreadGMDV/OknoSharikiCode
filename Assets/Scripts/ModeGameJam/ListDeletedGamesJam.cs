using System.Collections.Generic;
using UnityEngine;

public class ListDeletedGamesJam : MonoBehaviour
{
    public GameObject Prefab;
    public GameJamApi GamesScript;
    public List<GameObject> Prefabs = new List<GameObject>();

    private int lastCount = -1;   // отслеживаем изменения
    private int IdGameJam = -1;   // отслеживаем изменения

    void FixedUpdate()
    {
        if (GamesScript.deletedGames.Count != lastCount || !GamesScript.enabled || IdGameJam != GamesScript.GameJamId)
        {
            lastCount = GamesScript.deletedGames.Count;
            IdGameJam = GamesScript.GameJamId;
            Restart();
        }
    }

    public void Restart()
    {
        // Удаляем старые объекты
        foreach (var item in Prefabs)
        {
            if (item != null) Destroy(item);
        }
        Prefabs.Clear();

        for (int i = 0; i < GamesScript.deletedGames.Count; i++)
        {
            var pr = Instantiate(Prefab, transform);
            pr.SetActive(true);   // активируем созданный объект
            pr.GetComponent<Game>().Name = GamesScript.deletedGames[i].Name;
            pr.GetComponent<Game>().Url = GamesScript.deletedGames[i].Url;
            Prefabs.Add(pr);
        }
    }
}