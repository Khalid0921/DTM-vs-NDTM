using UnityEngine;
using UnityEngine.Splines;

public class SplineCarController : MonoBehaviour
{
    [Header("Spline Settings")]
    [Tooltip("Reference to the SplineContainer that holds the spline.")]
    public SplineContainer splineContainer; // Use SplineContainer instead of Spline

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
        t = Mathf.Clamp01(t);

        // Evaluate position and tangent along the spline.
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
