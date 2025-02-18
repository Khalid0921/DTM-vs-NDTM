using System.Collections;
using UnityEngine;

public class ObstacleLifter : MonoBehaviour
{
    [Tooltip("How high (in units) the obstacle will be lifted.")]
    public float liftHeight = 3f;

    [Tooltip("Time (in seconds) it takes for the obstacle to lift.")]
    public float liftDuration = 1f;

    private bool isLifted = false;

    /// <summary>
    /// Lifts the obstacle smoothly upward.
    /// </summary>
    public IEnumerator LiftObstacle()
    {
        if (isLifted)
            yield break; // Prevent multiple lifts

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + Vector3.up * liftHeight;
        float elapsed = 0f;

        while (elapsed < liftDuration)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / liftDuration);
            yield return null;
        }
        transform.position = targetPos;
        isLifted = true;
    }
}
