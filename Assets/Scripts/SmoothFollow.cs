using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;

    private Vector3 currentVelocity;

    void LateUpdate() // Use LateUpdate if following a physics-based player
    {
        if (target != null)
        {
            // Gradually changes a vector towards a desired goal over time
            transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentVelocity, smoothTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
    }
}
