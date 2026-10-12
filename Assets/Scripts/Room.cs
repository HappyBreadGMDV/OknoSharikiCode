using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public RoomType roomType;

    public enum RoomType
    {
        Big,
        Small
    }
}
