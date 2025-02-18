using UnityEngine;
using UnityEngine.UI;

public class CarStopUIManager : MonoBehaviour
{
    [Tooltip("The UI Button that appears when the car stops. Must have a Button component.")]
    public GameObject continueButton;

    [Tooltip("Reference to the car's transform.")]
    public Transform carTransform;

    [Tooltip("Offset from the car's position where the button should appear (in world units).")]
    public Vector3 buttonOffset = new Vector3(2f, 1f, 0f);

    [Tooltip("Reference to the CarCollisionDeceleration script.")]
    public CarCollisionDeceleration carCollisionDeceleration;

    private Button continueBtn;

    void Start()
    {
        // Ensure the continueButton reference is assigned.
        if (continueButton == null)
        {
            Debug.LogError("Continue button is not assigned in CarStopUIManager!");
            return;
        }

        // Get the Button component from the continueButton GameObject.
        continueBtn = continueButton.GetComponent<Button>();
        if (continueBtn == null)
        {
            Debug.LogError("Continue button GameObject does not have a Button component!");
            return;
        }

        // Remove any existing listeners and add our listener.
        continueBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.AddListener(OnContinueButtonClicked);

        // Hide the Continue button at start.
        continueButton.SetActive(false);
    }

    /// <summary>
    /// Called by the deceleration script when the car has fully stopped.
    /// Displays the Continue button next to the car.
    /// </summary>
    public void OnCarStopped()
    {
        Debug.Log("UI Manager: Car has stopped. Activating Continue button...");
        if (carTransform != null)
        {
            // Position the button relative to the car's position.
            continueButton.transform.position = carTransform.position + buttonOffset;
            continueButton.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Car transform is not assigned in CarStopUIManager!");
        }
    }

    /// <summary>
    /// Called when the Continue button is clicked.
    /// Hides the button and signals the deceleration script to resume the car.
    /// </summary>
    public void OnContinueButtonClicked()
    {
        Debug.Log("Continue button clicked. Resuming car movement.");
        continueButton.SetActive(false);

        if (carCollisionDeceleration != null)
        {
            carCollisionDeceleration.ResumeCar();
        }
        else
        {
            Debug.LogWarning("CarCollisionDeceleration reference missing in CarStopUIManager!");
        }
    }
}
