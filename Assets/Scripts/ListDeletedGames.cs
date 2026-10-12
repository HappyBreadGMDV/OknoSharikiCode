using System.Collections.Generic;
using UnityEngine;

public class ListDeletedGames : MonoBehaviour
{
    public GameObject Prefab;
    public SuperSimpleFetcher GamesScript;
    public List<GameObject> Prefabs = new List<GameObject>();

    private int lastCount = -1;   // отслеживаем изменения

    void FixedUpdate()
    {
        if (GamesScript.deletedGames.Count != lastCount)
        {
            lastCount = GamesScript.deletedGames.Count;
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
            pr.GetComponent<Game>().Author = GamesScript.deletedGames[i].Author;
            pr.GetComponent<Game>().Url = GamesScript.deletedGames[i].Url;
            Prefabs.Add(pr);
        }
    }
}