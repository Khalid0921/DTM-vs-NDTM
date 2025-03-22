using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class CarCollisionDeceleration : MonoBehaviour
{
    [Tooltip("Reference to the SplineCarController on the car.")]
    public SplineCarController splineCarController;

    [Tooltip("Time (in seconds) over which the car decelerates to a stop.")]
    public float decelerationDuration = 2f;

    [Tooltip("Time (in seconds) over which the car accelerates back to full speed.")]
    public float accelerationDuration = 2f;

    [Tooltip("Reference to the UI Manager that will display the Continue button and track options.")]
    public CarStopUIManager uiManager;

    private bool isDecelerating = false;
    private bool resumeRequested = false;
    private TrackJunction currentJunction = null;

    // Add this helper to update the current junction from outside
    public void SetCurrentJunction(TrackJunction junction)
    {
        currentJunction = junction;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter called with tag: {other.tag}, Object name: {other.gameObject.name}");
        if (!isDecelerating)
        {
            if (other.CompareTag("Junction"))
            {
                currentJunction = other.GetComponent<TrackJunction>();
                if (currentJunction != null)
                {
                    Debug.Log($"Junction found with tracks - A: {currentJunction.nextTrackA?.name ?? "null"}, B: {currentJunction.nextTrackB?.name ?? "null"}");
                    StartCoroutine(DecelerateAndSignalUI(true));
                }
                else
                {
                    Debug.LogError("Junction collider doesn't have TrackJunction component!");
                }
            }
            else if (other.CompareTag("Obstacle"))
            {
                Debug.Log("Obstacle detected, starting deceleration.");
                StartCoroutine(DecelerateAndSignalUI(false));
            }
        }
    }

    private IEnumerator DecelerateAndSignalUI(bool isJunction)
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

        // Signal the UI manager based on whether it's a junction or obstacle
        if (uiManager != null)
        {
            if (isJunction)
            {
                uiManager.OnCarReachedJunction(currentJunction);
            }
            else
            {
                uiManager.OnCarStopped();
            }
        }

        // Wait until the UI signals to resume or choose a track
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

        // Clear the junction reference only after resuming
        currentJunction = null;
    }

    /// <summary>
    /// Called by the UI manager when the Continue button is clicked or Track 1 is chosen.
    /// </summary>
    public void ResumeCar()
    {
        resumeRequested = true;
    }

    /// <summary>
    /// Called by the UI manager when Track 1 is chosen (to continue on or switch back to Track 1).
    /// </summary>
    public void ChooseTrack1()
    {
        Debug.Log("ChooseTrack1 called");
        if (currentJunction == null)
        {
            Debug.LogError("No active junction when choosing Track 1!");
            return;
        }
        if (currentJunction.nextTrackA == null)
        {
            Debug.LogError("Junction's Track A is null!");
            return;
        }

        Vector3 junctionPos = currentJunction.transform.position;
        Debug.Log($"Switching to Track A: {currentJunction.nextTrackA.name} at junction position: {junctionPos}");
        splineCarController.SwitchSpline(currentJunction.nextTrackA, junctionPos);
        resumeRequested = true;
    }

    /// <summary>
    /// Called by the UI manager when Track 2 is chosen at a junction.
    /// </summary>
    /// <param name="chosenTrack">The spline to switch to (Track 2).</param>
    public void ChooseTrack2(SplineContainer chosenTrack)
    {
        Debug.Log("ChooseTrack2 called");
        if (currentJunction == null)
        {
            Debug.LogError("No active junction when choosing Track 2!");
            return;
        }
        if (chosenTrack == null)
        {
            Debug.LogError("Chosen track (Track 2) is null!");
            return;
        }

        Vector3 junctionPos = currentJunction.transform.position;
        Debug.Log($"Switching to Track B: {chosenTrack.name} at junction position: {junctionPos}");
        splineCarController.SwitchSpline(chosenTrack, junctionPos);
        resumeRequested = true;
    }
}