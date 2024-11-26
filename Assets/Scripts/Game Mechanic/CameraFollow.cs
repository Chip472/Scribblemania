using UnityEngine;

public class CameraFollowWithBoundaries : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    public Vector2 minBoundary;
    public Vector2 maxBoundary;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;

            float clampedX = Mathf.Clamp(desiredPosition.x, minBoundary.x, maxBoundary.x);
            float clampedY = Mathf.Clamp(desiredPosition.y, minBoundary.y, maxBoundary.y);

            // Move the camera instantly to the target position
            transform.position = new Vector3(clampedX, clampedY, desiredPosition.z);
        }
    }
}
