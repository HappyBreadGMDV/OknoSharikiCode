using UnityEngine;

public class AnimationPosition : MonoBehaviour
{
    public float speedPosition = 90f;   // градусов в секунду

    public bool xPosition;
    public bool yPosition;
    public bool zPosition;

    void Update()
    {
        // Собираем вектор угловой скорости (градусов в секунду)
        Vector3 rotationVector = Vector3.zero;
        if (xPosition) rotationVector.x = 1f;
        if (yPosition) rotationVector.y = 1f;
        if (zPosition) rotationVector.z = 1f;

        // Вращаем объект в локальных осях
        transform.position += rotationVector * speedPosition * Time.deltaTime;
    }
}