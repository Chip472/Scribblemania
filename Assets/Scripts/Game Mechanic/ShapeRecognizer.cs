using UnityEngine;
using System.Collections.Generic;
using PDollarGestureRecognizer;
using UnityEngine.Rendering;
using System.Collections;

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

    public ParticleSystem psArrow, psTriangle, psCircle;

    // Shape unlock toggles
    private Dictionary<string, bool> unlockedShapes = new Dictionary<string, bool>
    {
        { "arrow", false },
        { "triangle", false },
        { "circle", false },
        { "rectangle", false },
        { "heart", false },
        { "star", false }
    };

    void Start()
    {
        string[] filePaths = System.IO.Directory.GetFiles(Application.dataPath + "/SavedShapes/", "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));
    }

    void Update()
    {
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
        // Check if the shape is unlocked
        if (!IsShapeUnlocked(shapeName) || gestureScore <= 0.9f)
        {
            Debug.Log($"Shape {shapeName} not unlocked or low score. Vanishing...");
            Vanish(CreateTemporaryShapeObject());
            return;
        }

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
                rb.velocity = Vector2.up * arrowSpd;
                ExplodeOnCollision(shape, psArrow);
                break;

            case "arrow down":
                rb.velocity = Vector2.down * arrowSpd;
                ExplodeOnCollision(shape, psArrow);
                break;

            case "arrow right":
                rb.velocity = Vector2.right * arrowSpd;
                ExplodeOnCollision(shape, psArrow);
                break;

            case "arrow left":
                rb.velocity = Vector2.left * arrowSpd;
                ExplodeOnCollision(shape, psArrow);
                break;

            case "triangle up":
                rb.velocity = Vector2.up * triaSpd;
                ExplodeOnCollision(shape, psTriangle);
                break;

            case "triangle down":
                rb.velocity = Vector2.down * triaSpd;
                ExplodeOnCollision(shape, psTriangle);
                break;

            case "triangle right":
                rb.velocity = Vector2.right * triaSpd;
                ExplodeOnCollision(shape, psTriangle);
                break;

            case "triangle left":
                rb.velocity = Vector2.left * triaSpd;
                ExplodeOnCollision(shape, psTriangle);
                break;

            case "rectangle":
                rb.gravityScale = 1f;
                BreakOnTrigger(shape);
                break;

            case "circle":
                AddCircleMovement(shape, rb, player.transform);
                break;

            case "heart":
                //heart
                break;

            case "star":
                //ulti
                break;

            default:
                Debug.LogWarning("Shape behavior not defined for: " + shapeName);
                break;
        }
    }

    private void Vanish(GameObject shape)
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

        StartCoroutine(FadeAndDestroyShape(shape));
    }

    private IEnumerator FadeAndDestroyShape(GameObject shape)
    {
        LineRenderer lineRenderer = shape.GetComponentInChildren<LineRenderer>();
        if (lineRenderer == null) yield break;

        float fadeDuration = 0.3f;
        float startTime = Time.time;

        Color startColor = lineRenderer.startColor;
        Color blackColor = new Color(0, 0, 0, startColor.a);
        lineRenderer.startColor = blackColor;
        lineRenderer.endColor = blackColor;

        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            Color fadedColor = Color.Lerp(blackColor, new Color(0, 0, 0, 0), t);

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
    private void ExplodeOnCollision(GameObject shape, ParticleSystem particle)
    {
        Collider2D collider = shape.GetComponent<Collider2D>();
        collider.isTrigger = false;
        collider.gameObject.AddComponent<ExplosionBehavior>();

        shape.GetComponent<ExplosionBehavior>().explosionEffect = particle;
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
        ExplodeOnCollision(shape, psCircle);

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
