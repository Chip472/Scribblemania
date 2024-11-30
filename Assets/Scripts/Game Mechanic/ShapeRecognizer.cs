using UnityEngine;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.UI;

public class ShapeRecognizer : MonoBehaviour
{
    public GameObject player;

    public RectTransform drawAreaUI;
    public Transform gestureOnScreenPrefab;
    public float arrowSpd, triaSpd, cirSpd;

    private List<Gesture> trainingSet = new List<Gesture>();
    private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector3 virtualKeyPosition = Vector2.zero;
    private int vertexCount = 0;

    private LineRenderer currentGestureLineRenderer;
    private LineRenderer savedGestureLineRenderer;

    private bool recognized;
    public string drawnShapeName;
    public float drawnShapeScore;

    public ParticleSystem psArrow, psTriangle, psCircle;

    public AudioSource shootSFX;
    public AudioSource triangleSFX, arrowSFX;

    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float rechargeRate = 10f;
    public bool atSavePoint = false;
    
    public Slider batterySlider;

    public bool isUsingUltimate = false;
    public GameObject stickPrefab;

    private Dictionary<string, bool> unlockedShapes = new Dictionary<string, bool>
    {
        { "arrow", false },
        { "triangle", false },
        { "circle", false },
        { "rectangle", false },
        { "heart", false },
        { "star", false }
    };

    private Dictionary<string, float> shapeBatteryCosts = new Dictionary<string, float>
    {
        { "arrow up", 1f },
        { "arrow down", 1f },
        { "arrow left", 1f },
        { "arrow right", 1f },
        { "triangle up", 5f },
        { "triangle down", 5f },
        { "triangle left", 5f },
        { "triangle right", 5f },
        { "circle", 5f },
        { "rectangle", 3f },
        { "heart", 10f },
        { "star", 15f }
    };


    void Start()
    {
        currentBattery = PlayerPrefs.GetFloat("Battery", 100);

        string[] filePaths = System.IO.Directory.GetFiles(Application.dataPath + "/SavedShapes/", "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
    }

    void Update()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / maxBattery;
        }

        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                virtualKeyPosition = Input.GetTouch(0).position;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = Input.mousePosition;
            }
        }

        Stuff();

        if (!drawAreaUI.gameObject.activeSelf)
        {
            ClearDrawing();
        }
    }

    void Stuff()
    {
        currentBattery = PlayerPrefs.GetFloat("Battery", 100);

        if (currentBattery <= 0f)
        {
            Debug.Log("Battery depleted! Cannot draw.");
            return;
        }

        if (IsWithinDrawArea(virtualKeyPosition) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ++strokeId;
                gestureOnScreenPrefab.GetComponent<LineRenderer>().enabled = true;
                gestureOnScreenPrefab.GetComponent<LineRenderer>().sortingLayerName = "shapes";
                currentGestureLineRenderer = gestureOnScreenPrefab.GetComponent<LineRenderer>();

                vertexCount = 0;
            }

            if (Input.GetMouseButton(0))
            {
                points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));

                currentGestureLineRenderer.positionCount = ++vertexCount;
                currentGestureLineRenderer.SetPosition(vertexCount - 1,
                    Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
            }

            if (Input.GetMouseButtonUp(0))
            {
                recognized = true;
                gestureOnScreenPrefab.GetComponent<LineRenderer>().enabled = false;

                Gesture candidate = new Gesture(points.ToArray());
                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());

                drawnShapeName = gestureResult.GestureClass;
                drawnShapeScore = gestureResult.Score;
                Debug.Log(gestureResult.GestureClass + " " + gestureResult.Score);

                CreateShapeGameObject(gestureResult.GestureClass, gestureResult.Score);

                if (recognized)
                {
                    ClearDrawing();
                }
            }
        }
    }


    private bool IsWithinDrawArea(Vector3 inputPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            drawAreaUI,
            inputPosition,
            null,
            out Vector2 localPoint
        );
        return drawAreaUI.rect.Contains(localPoint);
    }

    private void ClearDrawing()
    {
        recognized = false;
        strokeId = -1;

        points.Clear();

        if (currentGestureLineRenderer != null)
        {
            currentGestureLineRenderer.positionCount = 0;
        }
        StartCoroutine(DelayResetShapesName());
    }

    IEnumerator DelayResetShapesName()
    {
        yield return new WaitForSeconds(0.5f);
        drawnShapeName = null;
    }


    private void CreateShapeGameObject(string shapeName, float gestureScore)
    {
        if (!IsShapeUnlocked(shapeName) || gestureScore <= 0.9f)
        {
            Debug.Log($"Shape {shapeName} not unlocked or low score. Vanishing...");
            Vanish(CreateTemporaryShapeObject(), Color.black);
            return;
        }

        // Check battery cost
        if (!shapeBatteryCosts.TryGetValue(shapeName.ToLower(), out float batteryCost))
        {
            Debug.LogWarning($"Shape {shapeName} has no battery cost defined.");
            return;
        }

        if (currentBattery < batteryCost)
        {
            Debug.Log("Not enough battery to draw this shape!");
            return;
        }

        // Deduct battery
        currentBattery -= batteryCost;
        PlayerPrefs.SetFloat("Battery", currentBattery);
        Debug.Log($"Battery used: {batteryCost}. Remaining: {currentBattery}/{maxBattery}");

        GameObject shapeObject = new GameObject("Drawn shape");
        Rigidbody2D rb = shapeObject.AddComponent<Rigidbody2D>();
        PolygonCollider2D collider = shapeObject.AddComponent<PolygonCollider2D>();
        shapeObject.layer = LayerMask.NameToLayer("Ground");

        collider.points = GetColliderPointsFromLineRenderer(currentGestureLineRenderer);

        GameObject lineRendererObject = new GameObject("LineRenderer");
        lineRendererObject.transform.SetParent(shapeObject.transform);
        lineRendererObject.transform.localPosition = Vector3.zero;

        LineRenderer shapeLineRenderer = lineRendererObject.AddComponent<LineRenderer>();
        shapeLineRenderer.useWorldSpace = false;
        shapeLineRenderer.sortingLayerName = "shapes";

        Vector3[] worldPositions = GetWorldPositionsFromLineRenderer(currentGestureLineRenderer);

        for (int i = 0; i < worldPositions.Length; i++)
        {
            worldPositions[i] = shapeObject.transform.InverseTransformPoint(worldPositions[i]);
        }

        shapeLineRenderer.positionCount = worldPositions.Length;
        shapeLineRenderer.SetPositions(worldPositions);

        shapeLineRenderer.material = currentGestureLineRenderer.material;
        shapeLineRenderer.widthMultiplier = currentGestureLineRenderer.widthMultiplier;
        shapeLineRenderer.startColor = currentGestureLineRenderer.startColor;
        shapeLineRenderer.endColor = currentGestureLineRenderer.endColor;
        shapeLineRenderer.startWidth = currentGestureLineRenderer.startWidth;
        shapeLineRenderer.endWidth = currentGestureLineRenderer.endWidth;

        savedGestureLineRenderer = shapeLineRenderer;
        points.Clear();

        HandleShapeBehavior(shapeObject, shapeName, rb);
    }

    private void HandleShapeBehavior(GameObject shape, string shapeName, Rigidbody2D rb)
    {
        switch (shapeName.ToLower())
        {
            case "arrow up":
                shootSFX.Play();
                rb.velocity = Vector2.up * arrowSpd;
                ExplodeOnCollision(shape, psArrow, arrowSFX);
                SavedShape(shape);
                break;

            case "arrow down":
                shootSFX.Play();
                rb.velocity = Vector2.down * arrowSpd;
                ExplodeOnCollision(shape, psArrow, arrowSFX);
                SavedShape(shape);
                break;

            case "arrow right":
                shootSFX.Play();
                rb.velocity = Vector2.right * arrowSpd;
                ExplodeOnCollision(shape, psArrow, arrowSFX);
                SavedShape(shape);
                break;

            case "arrow left":
                shootSFX.Play();
                rb.velocity = Vector2.left * arrowSpd;
                ExplodeOnCollision(shape, psArrow, arrowSFX);
                SavedShape(shape);
                break;

            case "triangle up":
                shootSFX.Play();
                rb.velocity = Vector2.up * triaSpd;
                ExplodeOnCollision(shape, psTriangle, triangleSFX);
                SavedShape(shape);
                break;

            case "triangle down":
                shootSFX.Play();
                rb.velocity = Vector2.down * triaSpd;
                ExplodeOnCollision(shape, psTriangle, triangleSFX);
                SavedShape(shape);
                break;

            case "triangle right":
                shootSFX.Play();
                rb.velocity = Vector2.right * triaSpd;
                ExplodeOnCollision(shape, psTriangle, triangleSFX);
                SavedShape(shape);
                break;

            case "triangle left":
                shootSFX.Play();
                rb.velocity = Vector2.left * triaSpd;
                ExplodeOnCollision(shape, psTriangle, triangleSFX);
                SavedShape(shape);
                break;

            case "rectangle":
                rb.gravityScale = 1f;
                BreakOnTrigger(shape);
                SavedShape(shape);
                break;

            case "circle":
                AddCircleMovement(shape, rb, player.transform);
                break;

            case "heart":
                Vanish(shape, new Color(255, 0, 190));
                break;

            case "star":
                StarUltimate(shape);
                SavedShape(shape);
                break;

            default:
                Debug.LogWarning("Shape behavior not defined for: " + shapeName);
                break;
        }
    }

    private void Vanish(GameObject shape, Color fadeColor)
    {
        Rigidbody2D rb = shape.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.isKinematic = true;
        }

        Collider2D collider = shape.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        StartCoroutine(FadeAndDestroyShape(shape, fadeColor));
    }

    private IEnumerator FadeAndDestroyShape(GameObject shape, Color fadeColor)
    {
        LineRenderer lineRenderer = shape.GetComponentInChildren<LineRenderer>();
        if (lineRenderer == null) yield break;

        float fadeDuration = 0.3f;
        float startTime = Time.time;

        Color startColor = fadeColor;
        Color endColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0); // Fade to transparent

        lineRenderer.startColor = startColor;
        lineRenderer.endColor = startColor;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            Color fadedColor = Color.Lerp(startColor, endColor, t);

            lineRenderer.startColor = fadedColor;
            lineRenderer.endColor = fadedColor;
            yield return null;
        }

        Destroy(shape);
    }


    #region Line renderer stuff

    private Vector3[] GetWorldPositionsFromLineRenderer(LineRenderer lineRenderer)
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        return positions;
    }

    private Vector2[] GetColliderPointsFromLineRenderer(LineRenderer lineRenderer)
    {
        Vector3[] positions = GetWorldPositionsFromLineRenderer(lineRenderer);
        List<Vector2> colliderPoints = new List<Vector2>();

        for (int i = 0; i < positions.Length; i++)
        {
            colliderPoints.Add(new Vector2(positions[i].x, positions[i].y));
        }

        return colliderPoints.ToArray();
    }

    #endregion

    #region After shape recognition 
    private void SavedShape(GameObject shape)
    {
        SavedShapeNameAndScore saved = shape.AddComponent<SavedShapeNameAndScore>();
        saved.shapeName = drawnShapeName;
        saved.shapeScore = drawnShapeScore;
    }

    private void StarUltimate(GameObject shape)
    {
        StarSpawner starSpawner = shape.AddComponent<StarSpawner>();
        starSpawner.stickPrefab = stickPrefab;
        starSpawner.transform.position = shape.transform.position;

        isUsingUltimate = true;
    }

    private void ExplodeOnCollision(GameObject shape, ParticleSystem particle, AudioSource sfx)
    {
        Collider2D collider = shape.GetComponent<Collider2D>();
        collider.isTrigger = false;
        ExplosionBehavior stuff = collider.gameObject.AddComponent<ExplosionBehavior>();

        stuff.explosionEffect = particle;
        stuff.explosionSoundSource = sfx;
    }

    private void BreakOnTrigger(GameObject shape)
    {
        BreakOnImpactBehavior breakScript = shape.AddComponent<BreakOnImpactBehavior>();
        breakScript.lineRenderer = savedGestureLineRenderer;
    }


    public void UpdateSavedGestureLineRenderer(GameObject shape)
    {
        if (savedGestureLineRenderer == null) return;

        LineRenderer lineRenderer = shape.GetComponentInChildren<LineRenderer>();
        Vector3[] worldPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(worldPositions);

        for (int i = 0; i < worldPositions.Length; i++)
        {
            worldPositions[i] = shape.transform.TransformPoint(worldPositions[i]);
        }

        savedGestureLineRenderer.positionCount = worldPositions.Length;
        savedGestureLineRenderer.SetPositions(worldPositions);
    }


    private void AddCircleMovement(GameObject shape, Rigidbody2D rb, Transform player)
    {
        ExplodeOnCollision(shape, psCircle, triangleSFX);

        shape.AddComponent<OrbBehavior>();

        shape.GetComponent<OrbBehavior>().rb = rb;
        shape.GetComponent<OrbBehavior>().player = player;
        shape.GetComponent<OrbBehavior>().cirSpd = cirSpd;
    }
    #endregion

    #region Unlock shapes
    private GameObject CreateTemporaryShapeObject()
    {
        GameObject tempShape = new GameObject("TemporaryShape");

        LineRenderer tempLineRenderer = tempShape.AddComponent<LineRenderer>();
        tempLineRenderer.positionCount = currentGestureLineRenderer.positionCount;
        tempLineRenderer.sortingLayerName = "shapes";

        Vector3[] positions = new Vector3[currentGestureLineRenderer.positionCount];
        currentGestureLineRenderer.GetPositions(positions);
        tempLineRenderer.SetPositions(positions);

        tempLineRenderer.material = currentGestureLineRenderer.material;
        tempLineRenderer.widthMultiplier = currentGestureLineRenderer.widthMultiplier;
        tempLineRenderer.startColor = currentGestureLineRenderer.startColor;
        tempLineRenderer.endColor = currentGestureLineRenderer.endColor;
        tempLineRenderer.startWidth = currentGestureLineRenderer.startWidth;
        tempLineRenderer.endWidth = currentGestureLineRenderer.endWidth;

        Rigidbody2D rb = tempShape.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;

        Collider2D collider = tempShape.AddComponent<PolygonCollider2D>();
        ((PolygonCollider2D)collider).points = GetColliderPointsFromLineRenderer(currentGestureLineRenderer);
        collider.enabled = false;

        return tempShape;
    }


    private bool IsShapeUnlocked(string shapeName)
    {
        if (shapeName.StartsWith("arrow")) return unlockedShapes["arrow"];
        if (shapeName.StartsWith("triangle")) return unlockedShapes["triangle"];
        return unlockedShapes.ContainsKey(shapeName.ToLower()) && unlockedShapes[shapeName.ToLower()];
    }

    public void UnlockShape(string shapeName)
    {
        if (unlockedShapes.ContainsKey(shapeName.ToLower()))
        {
            unlockedShapes[shapeName.ToLower()] = true;
            Debug.Log($"Shape {shapeName} unlocked!");
        }
        else
        {
            Debug.LogWarning($"Shape {shapeName} does not exist to unlock.");
        }
    }

    public void LockShape(string shapeName)
    {
        if (unlockedShapes.ContainsKey(shapeName.ToLower()))
        {
            unlockedShapes[shapeName.ToLower()] = false;
            Debug.Log($"Shape {shapeName} locked!");
        }
        else
        {
            Debug.LogWarning($"Shape {shapeName} does not exist to lock.");
        }
    }

    #endregion
}
