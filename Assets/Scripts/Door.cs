using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Room;

public class Door : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public Rotation doorType;

    public enum Rotation
    {
        BigToSmall,
        SmallToBig
    }
}
