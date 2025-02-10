using System.Collections;
using UnityEngine;

public class CarCollisionDeceleration : MonoBehaviour
{
    [Tooltip("Reference to the SplineCarController on the car.")]
    public SplineCarController splineCarController;

    [Tooltip("Time (in seconds) over which the car decelerates to a stop.")]
    public float decelerationDuration = 2f;

    [Tooltip("Time (in seconds) over which the car accelerates back to full speed.")]
    public float accelerationDuration = 2f;

    private bool isDecelerating = false;

    // This method is called when another collider enters a trigger attached to this GameObject.
    private void OnTriggerEnter(Collider other)
    {
        // Check that the collider is the cube (or whichever tag you assign)
        if (other.CompareTag("Obstacle") && !isDecelerating)
        {
            StartCoroutine(DecelerateAndResume());
        }
    }

    private IEnumerator DecelerateAndResume()
    {
        isDecelerating = true;
        float elapsed = 0f;
        float startMultiplier = splineCarController.MovementMultiplier;

        // Deceleration phase: gradually reduce the multiplier from its current value to 0.
        while (elapsed < decelerationDuration)
        {
            elapsed += Time.deltaTime;
            float newMultiplier = Mathf.Lerp(startMultiplier, 0f, elapsed / decelerationDuration);
            splineCarController.SetMovementMultiplier(newMultiplier);
            yield return null;
        }
        // Ensure the multiplier is exactly 0.
        splineCarController.SetMovementMultiplier(0f);

        // Wait until the player presses Space.
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        // Acceleration phase: gradually restore the multiplier from 0 to 1.
        elapsed = 0f;
        while (elapsed < accelerationDuration)
        {
            elapsed += Time.deltaTime;
            float newMultiplier = Mathf.Lerp(0f, 1f, elapsed / accelerationDuration);
            splineCarController.SetMovementMultiplier(newMultiplier);
            yield return null;
        }
        // Ensure the multiplier is fully restored.
        splineCarController.SetMovementMultiplier(1f);

        isDecelerating = false;
    }
}
