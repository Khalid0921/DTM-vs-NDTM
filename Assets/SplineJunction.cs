using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

[System.Serializable]
public class SplineJunction
{
    [Tooltip("The main track spline container.")]
    public SplineContainer mainTrack;

    [Tooltip("List of subtrack spline containers.")]
    public List<SplineContainer> subTracks;
}