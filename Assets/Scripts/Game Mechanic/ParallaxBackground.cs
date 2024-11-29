using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Vector2 parallaxFactor = new Vector2(0.5f, 0.5f);

    private Vector3 startPosition;
    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        startPosition = transform.position;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 cameraDelta = cameraTransform.position - lastCameraPosition;

        Vector3 parallaxMovement = new Vector3(
            cameraDelta.x * parallaxFactor.x,
            cameraDelta.y * parallaxFactor.y,
            0f);

        transform.position += parallaxMovement;
        lastCameraPosition = cameraTransform.position;
    }
}
