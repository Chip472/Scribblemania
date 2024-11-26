using UnityEngine;

public class BreakOnImpactBehavior : MonoBehaviour
{
    public float defaultBreakForceThreshold = 6f;
    public float playerBreakForceThreshold = 30f;
    public float dissipateTime = 2f;

    public LineRenderer lineRenderer;
    public int piecesToCreate = 15;

    private ShapeRecognizer shapeRecognizer;

    private void Start()
    {
        shapeRecognizer = FindFirstObjectByType<ShapeRecognizer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        float breakForceThreshold = defaultBreakForceThreshold;

        if (collision.collider.CompareTag("Player"))
        {
            breakForceThreshold = playerBreakForceThreshold; 
        }

        if (collision.relativeVelocity.magnitude >= breakForceThreshold)
        {
            shapeRecognizer.UpdateSavedGestureLineRenderer(gameObject);
            BreakApart();
        }
    }

    void BreakApart()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        int step = Mathf.Max(1, positions.Length / piecesToCreate);

        for (int i = 0; i < positions.Length - 1; i += step)
        {
            int nextIndex = Mathf.Min(i + step, positions.Length - 1);
            CreateLinePiece(positions[i], positions[nextIndex]);
        }

        Destroy(gameObject);
    }

    void CreateLinePiece(Vector3 start, Vector3 end)
    {
        GameObject linePiece = new GameObject("LinePiece");

        Rigidbody2D pieceRb = linePiece.AddComponent<Rigidbody2D>();

        LineRenderer pieceRenderer = linePiece.AddComponent<LineRenderer>();
        pieceRenderer.widthMultiplier = lineRenderer.widthMultiplier;
        pieceRenderer.positionCount = 2;
        pieceRenderer.SetPosition(0, start);
        pieceRenderer.SetPosition(1, end);
        pieceRenderer.material = lineRenderer.material;
        pieceRenderer.startColor = lineRenderer.startColor;
        pieceRenderer.endColor = lineRenderer.endColor;
        pieceRenderer.startWidth = lineRenderer.startWidth;
        pieceRenderer.endWidth = lineRenderer.endWidth;

        BoxCollider2D pieceCollider = linePiece.AddComponent<BoxCollider2D>();
        Vector2 lineDirection = end - start;
        float length = lineDirection.magnitude;

        pieceCollider.size = new Vector2(length, lineRenderer.endWidth + 0.1f);
        pieceCollider.offset = new Vector2(length / 2, 0);

        float angle = Mathf.Atan2(lineDirection.y, lineDirection.x) * Mathf.Rad2Deg;
        linePiece.transform.position = start;
        linePiece.transform.rotation = Quaternion.Euler(0, 0, angle);

        LinePieceBehavior dissipateScript = linePiece.AddComponent<LinePieceBehavior>();
        dissipateScript.dissipateTime = dissipateTime;
    }
}
