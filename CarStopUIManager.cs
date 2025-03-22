using UnityEngine;
using UnityEngine.UI;

public class CarStopUIManager : MonoBehaviour
{
    [Tooltip("The UI Button that appears when the car stops at an obstacle. Must have a Button component.")]
    public GameObject continueButton;

    [Tooltip("The UI Button that appears when the car reaches a junction for Track 1. Must have a Button component.")]
    public GameObject track1Button;

    [Tooltip("The UI Button that appears when the car reaches a junction for Track 2. Must have a Button component.")]
    public GameObject track2Button;

    [Tooltip("Reference to the CarCollisionDeceleration script.")]
    public CarCollisionDeceleration carCollisionDeceleration;

    private Button continueBtn;
    private Button track1Btn;
    private Button track2Btn;
    private TrackJunction currentJunction;

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

        // Set up Continue button listener
        continueBtn.onClick.RemoveAllListeners();
        continueBtn.onClick.AddListener(OnContinueButtonClicked);

        // Ensure track1Button is set up
        if (track1Button == null)
        {
            Debug.LogError("Track 1 button is not assigned in CarStopUIManager!");
            return;
        }
        track1Btn = track1Button.GetComponent<Button>();
        if (track1Btn == null)
        {
            Debug.LogError("Track 1 button GameObject does not have a Button component!");
            return;
        }
        track1Btn.onClick.RemoveAllListeners();
        track1Btn.onClick.AddListener(OnTrack1ButtonClicked);

        // Ensure track2Button is set up
        if (track2Button == null)
        {
            Debug.LogError("Track 2 button is not assigned in CarStopUIManager!");
            return;
        }
        track2Btn = track2Button.GetComponent<Button>();
        if (track2Btn == null)
        {
            Debug.LogError("Track 2 button GameObject does not have a Button component!");
            return;
        }
        track2Btn.onClick.RemoveAllListeners();
        track2Btn.onClick.AddListener(OnTrack2ButtonClicked);

        // Hide all buttons at start
        HideAllButtons();
    }

    /// <summary>
    /// Called by the deceleration script when the car has fully stopped at an obstacle.
    /// Displays the Continue button on the screen.
    /// </summary>
    public void OnCarStopped()
    {
        Debug.Log("UI Manager: Car has stopped at obstacle. Activating Continue button...");
        HideAllButtons();
        continueButton.SetActive(true);
    }

    /// <summary>
    /// Called by the deceleration script when the car reaches a junction.
    /// Displays both Track 1 and Track 2 buttons on the screen.
    /// </summary>
    /// <param name="junction">The junction the car stopped at.</param>
    public void OnCarReachedJunction(TrackJunction junction)
    {
        Debug.Log($"UI Manager: Car has reached junction. Junction object: {(junction != null ? junction.name : "null")}");
        if (junction != null && junction.nextTrackA != null && junction.nextTrackB != null)
        {
            HideAllButtons();
            currentJunction = junction;

            // Ensure the deceleration script has the correct junction reference.
            if (carCollisionDeceleration != null)
            {
                carCollisionDeceleration.SetCurrentJunction(junction);
            }

            track1Button.SetActive(true);
            track2Button.SetActive(true);
            Debug.Log($"Track options - A: {junction.nextTrackA.name}, B: {junction.nextTrackB.name}");
        }
        else
        {
            Debug.LogError($"Invalid junction state: junction={junction}, " +
                         $"nextTrackA={junction?.nextTrackA}, " +
                         $"nextTrackB={junction?.nextTrackB}");
        }
    }

    /// <summary>
    /// Called when the Continue button is clicked (for obstacles).
    /// </summary>
    private void OnContinueButtonClicked()
    {
        Debug.Log("Continue button clicked. Resuming car movement.");
        HideAllButtons();

        if (carCollisionDeceleration != null)
        {
            carCollisionDeceleration.ResumeCar();
        }
        else
        {
            Debug.LogWarning("CarCollisionDeceleration reference missing in CarStopUIManager!");
        }
    }

    /// <summary>
    /// Called when the Track 1 button is clicked (for junctions, to continue on Track 1).
    /// </summary>
    private void OnTrack1ButtonClicked()
    {
        Debug.Log("Track 1 button clicked");
        if (currentJunction == null)
        {
            Debug.LogError("No active junction when Track 1 clicked!");
            return;
        }
        if (carCollisionDeceleration == null)
        {
            Debug.LogError("CarCollisionDeceleration reference is missing!");
            return;
        }

        Debug.Log($"Calling ChooseTrack1");
        carCollisionDeceleration.ChooseTrack1(); // Removed parameter
        HideAllButtons();
    }

    /// <summary>
    /// Called when the Track 2 button is clicked (for junctions, to switch to Track 2).
    /// </summary>
    private void OnTrack2ButtonClicked()
    {
        Debug.Log("Track 2 button clicked");
        if (currentJunction == null)
        {
            Debug.LogError("No active junction when Track 2 clicked!");
            return;
        }
        if (currentJunction.nextTrackB == null)
        {
            Debug.LogError("Junction's Track B reference is null!");
            return;
        }
        if (carCollisionDeceleration == null)
        {
            Debug.LogError("CarCollisionDeceleration reference is missing!");
            return;
        }

        Debug.Log($"Switching to Track B: {currentJunction.nextTrackB.name}");
        carCollisionDeceleration.ChooseTrack2(currentJunction.nextTrackB);
        HideAllButtons();
    }

    /// <summary>
    /// Hides all UI buttons and clears the junction reference.
    /// </summary>
    private void HideAllButtons()
    {
        continueButton.SetActive(false);
        track1Button.SetActive(false);
        track2Button.SetActive(false);
        currentJunction = null;
    }
}