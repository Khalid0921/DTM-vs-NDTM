using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CarStopUIManager : MonoBehaviour
{
    [Tooltip("The UI Button that appears when the car stops.")]
    public GameObject continueButton; // Should be under a World Space Canvas

    [Tooltip("Reference to the car's transform.")]
    public Transform carTransform;

    [Tooltip("Offset from the car's position where the button should appear (in world units).")]
    public Vector3 buttonOffset = new Vector3(2f, 1f, 0f);

    [Tooltip("Reference to the SplineCarController (to resume movement if needed).")]
    public SplineCarController splineCarController;

    [Tooltip("Reference to the ObstacleLifter script on the obstacle.")]
    public ObstacleLifter obstacleLifter;

    [Tooltip("Reference to the CarCollisionDeceleration script.")]
    public CarCollisionDeceleration carCollisionDeceleration;

    void Start()
    {
        // Hide the Continue button initially.
        if (continueButton != null)
            continueButton.SetActive(false);
    }

    /// <summary>
    /// Called by the deceleration script when the car has fully stopped.
    /// Displays the Continue button next to the car.
    /// </summary>
    public void OnCarStopped()
    {
        Debug.Log("UI Manager: Car has stopped. Activating Continue button...");
        if (continueButton != null && carTransform != null)
        {
            continueButton.transform.position = carTransform.position + buttonOffset;
            continueButton.SetActive(true);
        }
    }

    /// <summary>
    /// Called by the Continue button's OnClick event.
    /// Hides the button, resumes the car, and triggers the obstacle to lift.
    /// </summary>
    public void OnContinueButtonClicked()
    {
        print("Entered the method");
        if (continueButton != null)
            continueButton.SetActive(false);

        // Signal the deceleration script to resume the car.
        if (carCollisionDeceleration != null)
            carCollisionDeceleration.ResumeCar();

             if (splineCarController != null)
        {
            splineCarController.SetMovementMultiplier(1f);
            // Do not enable autoMove; this allows the player to control movement with W and S.
        }

        // Optionally, trigger the obstacle to lift.
        if (obstacleLifter != null)
            StartCoroutine(obstacleLifter.LiftObstacle());
    }
}
