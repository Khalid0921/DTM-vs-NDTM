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

    [Tooltip("Reference to the UI Manager that will display the Continue button.")]
    public CarStopUIManager uiManager;

    private bool isDecelerating = false;
    private bool resumeRequested = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with tag: " + other.tag);
        if (other.CompareTag("Obstacle") && !isDecelerating)
        {
            Debug.Log("Obstacle detected, starting deceleration.");
            StartCoroutine(DecelerateAndSignalUI());
        }
    }

    private IEnumerator DecelerateAndSignalUI()
    {
        isDecelerating = true;
        float elapsed = 0f;
        float startMultiplier = splineCarController.MovementMultiplier;

        // Deceleration phase: gradually reduce the multiplier to 0.
        while (elapsed < decelerationDuration)
        {
            elapsed += Time.deltaTime;
            float newMultiplier = Mathf.Lerp(startMultiplier, 0f, elapsed / decelerationDuration);
            splineCarController.SetMovementMultiplier(newMultiplier);
            yield return null;
        }
        splineCarController.SetMovementMultiplier(0f);
        Debug.Log("Car decelerated to 0.");

        // Signal the UI manager to show the Continue button.
        if (uiManager != null)
            uiManager.OnCarStopped();

        // Wait until the UI signals that the user clicked the Continue button.
        resumeRequested = false;
        while (!resumeRequested)
        {
            yield return null;
        }
        Debug.Log("Resume requested from UI, accelerating...");

        // Acceleration phase: gradually restore the multiplier from 0 to 1.
        elapsed = 0f;
        while (elapsed < accelerationDuration)
        {
            elapsed += Time.deltaTime;
            float newMultiplier = Mathf.Lerp(0f, 1f, elapsed / accelerationDuration);
            splineCarController.SetMovementMultiplier(newMultiplier);
            yield return null;
        }
        splineCarController.SetMovementMultiplier(1f);
        isDecelerating = false;
    }

    /// <summary>
    /// Called by the UI manager when the Continue button is clicked.
    /// </summary>
    public void ResumeCar() 
    {
        resumeRequested = true;
    }
}
