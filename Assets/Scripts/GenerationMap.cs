using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GenerationMap : MonoBehaviour
{
    [System.Serializable]
    public struct MapPrefab
    {
        public GameObject prefab;
        public bool useInGeneration;
    }

    public MapPrefab[] Maps;                  // теперь массив с флагом
    public GameObject DoorBigToSmall;
    public GameObject DoorSmallToBig;
    public GameObject End;
    [SerializeField] private TMP_InputField inputField;
    public int Count;

    public Room LastRoom;
    public List<GameObject> Map = new List<GameObject>();   // обычный список

    public void ChangeLen()
    {
        Count = int.Parse(inputField.text);
    }

    public void DeleteMap()
    {
        foreach (var item in Map)
        {
            Destroy(item);              // item теперь GameObject
        }
        Map.Clear();
    }

    public void ResetMap()
    {
        foreach (var item in Map)
        {
            Destroy(item);
        }
        Map.Clear();
        GenerateMap();
    }

    private void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        if (Maps == null || Maps.Length == 0) return;

        // Собираем индексы префабов, разрешённых для генерации
        var allowedIndices = new List<int>();
        for (int i = 0; i < Maps.Length; i++)
        {
            if (Maps[i].useInGeneration && Maps[i].prefab != null)
                allowedIndices.Add(i);
        }
        if (allowedIndices.Count == 0) return;

        // Первая комната
        var obj = Instantiate(Maps[allowedIndices[Random.Range(0, allowedIndices.Count)]].prefab, transform);
        Map.Add(obj);
        LastRoom = obj.GetComponent<Room>();
        if (LastRoom == null || LastRoom.StartPoint == null) return;
        obj.transform.position = transform.position - LastRoom.StartPoint.localPosition;

        for (int i = 0; i < Count; i++)
        {
            if (LastRoom.EndPoint == null) break;

            var newObj = Instantiate(Maps[allowedIndices[Random.Range(0, allowedIndices.Count)]].prefab, transform);
            var newRoom = newObj.GetComponent<Room>();
            Map.Add(newObj);

            newObj.transform.position = LastRoom.EndPoint.position - newRoom.StartPoint.localPosition;

            // Дверь при смене типа комнаты
            if (LastRoom.roomType != newRoom.roomType)
            {
                GameObject doorPrefab = null;
                if (newRoom.roomType == Room.RoomType.Big && LastRoom.roomType == Room.RoomType.Small)
                    doorPrefab = DoorSmallToBig;
                else if (newRoom.roomType == Room.RoomType.Small && LastRoom.roomType == Room.RoomType.Big)
                    doorPrefab = DoorBigToSmall;

                if (doorPrefab != null)
                {
                    var door = Instantiate(doorPrefab, transform);
                    Map.Add(door);
                    var doorComp = door.GetComponent<Door>();
                    if (doorComp != null && doorComp.StartPoint != null && doorComp.EndPoint != null)
                    {
                        AlignTransform(door.transform, doorComp.StartPoint, LastRoom.EndPoint.position, LastRoom.EndPoint.rotation);
                        newObj.transform.position -= newRoom.StartPoint.position - doorComp.EndPoint.position;
                    }
                }
            }

            LastRoom = newRoom;
        }

        // Конечная комната
        var endObj = Instantiate(End, transform);
        Map.Add(endObj);
        var endComp = endObj.GetComponent<Room>();
        if (LastRoom.roomType == Room.RoomType.Big)
        {
            var door = Instantiate(DoorBigToSmall, transform);
            Map.Add(door);
            var doorComp = door.GetComponent<Door>();
            AlignTransform(door.transform, doorComp.StartPoint, LastRoom.EndPoint.position, LastRoom.EndPoint.rotation);
            AlignTransform(endObj.transform, endComp.StartPoint, doorComp.EndPoint.position, doorComp.EndPoint.rotation);
        }
        else
        {
            AlignTransform(endComp.transform, endComp.StartPoint, LastRoom.EndPoint.position, LastRoom.EndPoint.rotation);
        }
    }

    void AlignTransform(Transform root, Transform localPoint, Vector3 targetPos, Quaternion targetRot)
    {
        root.rotation = targetRot * Quaternion.Inverse(localPoint.localRotation);
        root.position = targetPos - root.rotation * localPoint.localPosition;
    }
}