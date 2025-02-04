using UnityEngine;
using SplineMesh;

public class SplineCarController : MonoBehaviour
{
    public SplineAnimator splineAnimator; // Reference to the Spline Animator
    public float[] stopSignPositions;     // Normalized positions for stop signs (e.g., [0.2f, 0.5f, 0.8f])
    public float moveSpeed = 0.1f;        // Speed of the car along the spline
    private int currentStateIndex = 0;    // Current state (starts at q0)
    private bool isMoving = false;

    void Update()
    {
        // Check for button press (e.g., Spacebar or UI button)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveToNextState();
        }

        // Move the car along the spline
        if (isMoving)
        {
            MoveCar();
        }
    }

    void MoveToNextState()
    {
        if (currentStateIndex < stopSignPositions.Length - 1)
        {
            currentStateIndex++; // Advance to the next state
            isMoving = true;     // Start moving the car
        }
        else
        {
            Debug.Log("Car has reached the final state.");
        }
    }

    void MoveCar()
    {
        // Get the target position for the current state
        float targetPosition = stopSignPositions[currentStateIndex];

        // Move the car towards the target position
        splineAnimator.NormalizedTime = Mathf.MoveTowards(splineAnimator.NormalizedTime, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the car has reached the stop sign
        if (Mathf.Abs(splineAnimator.NormalizedTime - targetPosition) < 0.01f)
        {
            isMoving = false; // Stop moving
            Debug.Log("Car has reached state: q" + currentStateIndex);
        }
    }
}