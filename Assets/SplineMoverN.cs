using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;

public class SplineMoverN : MonoBehaviour
{
    public SplineContainer splineContainer;  // Assign your SplineContainer in Inspector
    public float moveSpeed = 5f;  // Speed of movement between knots
    public float stopDuration = 0.5f;  // Pause time at each knot

    private List<Vector3> knotPositions = new List<Vector3>();  // Stores knot positions
    private int currentKnotIndex = 0;  // Tracks current knot
    private bool isMoving = false;  // Prevents movement spam

    void Start()
    {
        if (splineContainer == null || splineContainer.Splines.Count == 0)
        {
            Debug.LogError("SplineContainer is not assigned or has no splines!");
            return;
        }

        Spline spline = splineContainer.Splines[0];

        // Extract all knots' positions
        foreach (BezierKnot knot in spline.Knots)
        {
            knotPositions.Add(knot.Position);
        }

        // Start at first knot
        if (knotPositions.Count > 0)
        {
            transform.position = knotPositions[0];
        }
    }

    void Update()
    {
        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveToNextKnot(1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveToNextKnot(-1);
            }
        }
    }

    void MoveToNextKnot(int direction)
    {
        int nextIndex = currentKnotIndex + direction;

        if (nextIndex >= 0 && nextIndex < knotPositions.Count)
        {
            currentKnotIndex = nextIndex;
            StartCoroutine(MoveToKnotCoroutine(knotPositions[currentKnotIndex]));
        }
    }

    IEnumerator MoveToKnotCoroutine(Vector3 targetPosition)
    {
        isMoving = true;
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        transform.position = targetPosition;
        yield return new WaitForSeconds(stopDuration);
        isMoving = false;
    }
}