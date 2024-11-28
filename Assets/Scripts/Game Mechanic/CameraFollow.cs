using UnityEngine;

public class CameraFollowWithBoundaries : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    public Vector2 minBoundary;
    public Vector2 maxBoundary;

    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.2f;

    private float shakeTimeRemaining = 0f;

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.position + offset;

            float clampedX = Mathf.Clamp(desiredPosition.x, minBoundary.x, maxBoundary.x);
            float clampedY = Mathf.Clamp(desiredPosition.y, minBoundary.y, maxBoundary.y);

            Vector3 shakeOffset = Vector3.zero;
            if (shakeTimeRemaining > 0)
            {
                shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0f;
                shakeTimeRemaining -= Time.deltaTime;
            }

            transform.position = new Vector3(clampedX, clampedY, desiredPosition.z) + shakeOffset;
        }
    }

    public void StartShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeTimeRemaining = duration;
    }
}
