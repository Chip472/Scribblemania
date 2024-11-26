using UnityEngine;
using System.Collections;

public class LinePieceBehavior : MonoBehaviour
{
    public float dissipateTime = 2f;

    private LineRenderer lineRenderer;
    private Vector3 localStart;
    private Vector3 localEnd;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        localStart = transform.InverseTransformPoint(lineRenderer.GetPosition(0));
        localEnd = transform.InverseTransformPoint(lineRenderer.GetPosition(1));

        StartCoroutine(DissipateLinePiece());
    }

    void Update()
    {
        lineRenderer.SetPosition(0, transform.TransformPoint(localStart));
        lineRenderer.SetPosition(1, transform.TransformPoint(localEnd));
    }

    IEnumerator DissipateLinePiece()
    {
        float elapsedTime = 0f;

        while (elapsedTime < dissipateTime)
        {
            elapsedTime += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / dissipateTime);
            lineRenderer.startColor = new Color(1f, 1f, 1f, alpha);
            lineRenderer.endColor = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        Destroy(gameObject);
    }
}
