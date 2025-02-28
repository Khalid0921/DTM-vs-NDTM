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
        Debug.Log("UI Manager: Car has reached junction. Activating Track 1 and Track 2 buttons...");
        if (junction != null)
        {
            HideAllButtons();
            currentJunction = junction;
            track1Button.SetActive(true);
            track2Button.SetActive(true);
            Debug.Log($"Showing Track 1 and Track 2 buttons for junction: nextTrackA = {junction.nextTrackA.name}, nextTrackB = {junction.nextTrackB.name}");
        }
        else
        {
            Debug.LogWarning("Junction is null in OnCarReachedJunction!");
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
        Debug.Log("Track 1 button clicked. Staying on or continuing Track 1.");
        if (currentJunction != null && carCollisionDeceleration != null)
        {
            carCollisionDeceleration.ChooseTrack1(); // Call ChooseTrack1 instead of ChooseTrack
            HideAllButtons();
        }
        else
        {
            Debug.LogWarning("Track 1 button clicked but junction or CarCollisionDeceleration is missing!");
        }
    }

    /// <summary>
    /// Called when the Track 2 button is clicked (for junctions, to switch to Track 2).
    /// </summary>
    private void OnTrack2ButtonClicked()
    {
        Debug.Log("Track 2 button clicked. Switching to Track 2.");
        if (currentJunction != null && carCollisionDeceleration != null)
        {
            carCollisionDeceleration.ChooseTrack2(currentJunction.nextTrackB); // Call ChooseTrack2 with nextTrackB
            HideAllButtons();
        }
        else
        {
            Debug.LogWarning("Track 2 button clicked but junction or CarCollisionDeceleration is missing!");
        }
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