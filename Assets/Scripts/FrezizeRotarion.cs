using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrezizeRotarion : MonoBehaviour
{
    public Transform Parent;
    void FixedUpdate()
    {
        transform.rotation = Quaternion.EulerAngles(0,0,0);
    }
}
