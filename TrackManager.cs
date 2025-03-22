using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TrackManager : MonoBehaviour
{
    public GameObject oldTrack; // Reference to the old track
    public GameObject newTrack; // Reference to the new track
    public GameObject activeJunction; // Reference to the active junction

    public void OnTrack1ButtonClicked()
    {
        Debug.Log("Track 1 button clicked");
        if (activeJunction == null)
        {
            Debug.LogError("No active junction when Track 1 clicked!");
            return;
        }

        // Logic to handle Track 1
        oldTrack.SetActive(true);
        newTrack.SetActive(false);
    }

    public void OnTrack2ButtonClicked()
    {
        Debug.Log("Track 2 button clicked");
        if (activeJunction == null)
        {
            Debug.LogError("No active junction when Track 2 clicked!");
            return;
        }

        // Logic to handle Track 2
        oldTrack.SetActive(false);
        newTrack.SetActive(true);
    }
}
