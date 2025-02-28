using UnityEngine;
using UnityEngine.Splines;

public class SplineCarController : MonoBehaviour
{
    [Header("Spline Settings")]
    [Tooltip("Reference to the SplineContainer that holds the spline.")]
    public SplineContainer splineContainer;

    [Tooltip("Speed at which the car moves along the spline (in spline parameter units per second).")]
    public float speed = 0.2f;

    // Current position along the spline (a value between 0 and 1)
    private float t = 0f;

    // This multiplier scales the player's input (1 = normal, 0 = stopped).
    private float movementMultiplier = 1f;

    /// <summary>
    /// Returns the current movement multiplier.
    /// </summary>
    public float MovementMultiplier => movementMultiplier;

    /// <summary>
    /// Sets the movement multiplier. Valid values are between 0 and 1.
    /// </summary>
    /// <param name="newMultiplier">The new multiplier value.</param>
    public void SetMovementMultiplier(float newMultiplier)
    {
        movementMultiplier = Mathf.Clamp01(newMultiplier);
    }

    /// <summary>
    /// Switches the car to a new spline, finding the nearest t value based on the current position.
    /// </summary>
    /// <param name="newSpline">The new spline to follow.</param>
    public void SwitchSpline(SplineContainer newSpline)
    {
        Debug.Log($"Switching to new spline: {newSpline.name}, Previous Spline: {splineContainer?.name}");
        if (newSpline != null)
        {
            splineContainer = newSpline;
            // Find the nearest t value on the new spline based on the car's current position
            t = FindNearestTOnSpline(newSpline, transform.position);
            UpdateCarPosition(); // Force update to ensure the car moves to the correct position on the new spline
        }
        else
        {
            Debug.LogError("New spline is null!");
        }
    }

    /// <summary>
    /// Finds the t value on a spline closest to a given world position.
    /// </summary>
    private float FindNearestTOnSpline(SplineContainer spline, Vector3 position)
    {
        if (spline == null || spline.Spline == null) return 0f;

        float closestT = 0f;
        float minDistance = float.MaxValue;

        // Sample the spline at regular intervals to find the closest point
        int samples = 100; // Number of points to sample (adjust for precision)
        for (int i = 0; i <= samples; i++)
        {
            float sampleT = i / (float)samples;
            Vector3 samplePosition = spline.EvaluatePosition(sampleT);
            float distance = Vector3.Distance(position, samplePosition);

            if (distance < minDistance)
            {
                minDistance = distance;
                closestT = sampleT;
            }
        }

        Debug.Log($"Nearest t on new spline: {closestT}, Distance: {minDistance}");
        return Mathf.Clamp01(closestT);
    }

    void Update()
    {
        // Get input for forward (W) and backward (S).
        float input = 0f;
        if (Input.GetKey(KeyCode.W))
        {
            input = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            input = -1f;
        }

        // Multiply the input by the movement multiplier.
        input *= movementMultiplier;

        // Update the spline parameter based on effective input and speed.
        t += input * speed * Time.deltaTime;

        // Wrap the spline parameter to allow multiple laps.
        if (t > 1f)
        {
            t -= 1f;
        }
        else if (t < 0f)
        {
            t += 1f;
        }

        // Evaluate position and tangent along the spline.
        UpdateCarPosition();
    }

    private void UpdateCarPosition()
    {
        if (splineContainer != null)
        {
            Vector3 newPosition = splineContainer.EvaluatePosition(t);
            Vector3 newTangent = splineContainer.EvaluateTangent(t);

            // Update the car's position.
            transform.position = newPosition;

            // Rotate the car to face the direction of the tangent.
            if (newTangent != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(newTangent);
            }
        }
    }
}