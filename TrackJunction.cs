using UnityEngine;
using UnityEngine.Splines;

public class TrackJunction : MonoBehaviour
{
    [Tooltip("Spline for the first possible next track (e.g., Track 1 or continuing on current track).")]
    public SplineContainer nextTrackA; // Should be Track 1 (to continue)

    [Tooltip("Spline for the second possible next track (e.g., Track 2).")]
    public SplineContainer nextTrackB; // Should be Track 2
}