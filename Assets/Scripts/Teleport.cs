using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform portal;   // точка назначения (Transform портала)

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Триггер сработал с объектом: " + other.name);
        if (portal == null)
        {
            Debug.LogError("Portal Transform не назначен!");
            return;
        }
        if (other.CompareTag("Player"))
        {
            Debug.LogError("Player Попал В портал!!!!!");
            return;
        }
        Vector3 newPos = other.transform.position;
        newPos.x = portal.position.x;
        other.transform.position = newPos;
    }
}