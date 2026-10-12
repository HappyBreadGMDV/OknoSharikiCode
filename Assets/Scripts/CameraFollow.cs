using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class FollowLeader : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public List<Transform> players = new List<Transform>();
    public bool follow = true;

    public float refreshInterval = 1f;
    private float nextRefreshTime;

    void Start()
    {
        RefreshPlayerList();
    }

    void LateUpdate()
    {
        // Периодически ищем новых игроков
        if (Time.time >= nextRefreshTime)
        {
            RefreshPlayerList();
            nextRefreshTime = Time.time + refreshInterval;
        }

        // Удаляем уничтоженных (пустые ссылки) перед поиском лидера
        players.RemoveAll(p => p == null);

        if (players.Count == 0)
            return;

        // Поиск лидера: здесь ищем игрока с **наименьшей** позицией по Y
        // Если нужен наибольший (лидер выше всех), замените < на >
        Transform leader = players[0];
        float bestY = leader.position.y;

        for (int i = 1; i < players.Count; i++)
        {
            if (players[i].position.y < bestY)   // < – минимальный, > – максимальный
            {
                bestY = players[i].position.y;
                leader = players[i];
            }
        }

        if (follow && leader != null)
        {
            virtualCamera.Follow = leader;
        }
    }

    public void RefreshPlayerList()
    {
        // Сначала чистим мёртвые ссылки
        players.RemoveAll(p => p == null);

        // Добавляем новых игроков
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var obj in allPlayers)
        {
            if (!players.Contains(obj.transform))
                players.Add(obj.transform);
        }
    }
}