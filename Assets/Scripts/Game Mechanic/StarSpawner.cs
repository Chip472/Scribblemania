using System.Collections;
using UnityEngine;

public class StarSpawner : MonoBehaviour
{
    [Header("Star Settings")]
    public float fadeDuration = 2f; // How long the star takes to fade away
    public float shootInterval = 0.1f; // Time between shooting sticks
    public int sticksPerRevolution = 36; // Number of sticks in one revolution (e.g., 10-degree steps)
    public int totalRevolutions = 1; // Number of revolutions to shoot
    public GameObject stickPrefab; // Prefab for the stick projectile

    private LineRenderer lineRenderer;
    private Color startColor;
    private Color endColor;
    private float fadeTimer;

    private SavedShapeNameAndScore savedShape;
    private float currentAngle = 0f; // Tracks the current angle for shooting
    private int totalSticksShot = 0; // Tracks how many sticks have been fired

    Rigidbody2D rb;
    Collider2D collider2D;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();

        rb.gravityScale = 0;
        collider2D.enabled = false;

        lineRenderer = GetComponentInChildren<LineRenderer>();
        savedShape = GetComponent<SavedShapeNameAndScore>();
        startColor = lineRenderer.startColor;
        endColor = lineRenderer.endColor;

        fadeTimer = fadeDuration;

        // Start fading and shooting
        StartCoroutine(FadeAndShoot());
    }

    private IEnumerator FadeAndShoot()
    {
        int sticksToShoot = sticksPerRevolution * totalRevolutions;

        while (fadeTimer > 0 || totalSticksShot < sticksToShoot)
        {
            fadeTimer -= Time.deltaTime;

            // Fade the LineRenderer
            float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);
            Color fadedStartColor = startColor;
            Color fadedEndColor = endColor;

            fadedStartColor.a = alpha;
            fadedEndColor.a = alpha;

            lineRenderer.startColor = fadedStartColor;
            lineRenderer.endColor = fadedEndColor;

            // Shoot sticks periodically
            if (totalSticksShot < sticksToShoot)
            {
                ShootStick();
                totalSticksShot++;

                // Wait for the interval before shooting the next stick
                yield return new WaitForSeconds(shootInterval);
            }
            else
            {
                yield return null;
            }
        }

        Destroy(gameObject); // Destroy the star object once fully faded
    }

    private void ShootStick()
    {
        // Calculate the direction from the current angle
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));

        // Instantiate the stick prefab at the current position of the StarSpawner
        GameObject stick = Instantiate(stickPrefab, FindFirstObjectByType<PlayerMovement>().gameObject.transform.position, Quaternion.identity, gameObject.transform);

        // Rename and set sorting layer
        stick.name = "Drawn shape";
        SpriteRenderer stickSprite = stick.GetComponent<SpriteRenderer>();
        stickSprite.sortingLayerName = "shapes";

        // Rotate the stick to face its direction
        float stickRotation = currentAngle - 90f; // Adjust by -90 degrees to align correctly
        stick.transform.rotation = Quaternion.Euler(0, 0, stickRotation);

        // Add Rigidbody2D for movement
        Rigidbody2D stickRb = stick.GetComponent<Rigidbody2D>();
        stickRb.gravityScale = 0; // Ensure it doesn't fall due to gravity
        stickRb.AddForce(direction * 5f, ForceMode2D.Impulse); // Adjust force for desired speed

        // Add SavedShapeNameAndScore component and copy data
        SavedShapeNameAndScore saved = stick.AddComponent<SavedShapeNameAndScore>();
        saved.shapeName = savedShape.shapeName;
        saved.shapeScore = savedShape.shapeScore;

        // Destroy the stick after a certain time
        Destroy(stick, 3f);

        // Update the angle for the next stick
        float angleStep = 360f / sticksPerRevolution;
        currentAngle += angleStep;

        // Reset angle to 0 after a full revolution
        if (currentAngle >= 360f)
        {
            currentAngle -= 360f;
        }
    }

}
