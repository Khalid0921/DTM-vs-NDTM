using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform car; // Reference to the car
    public Vector3 offset = new Vector3(0, 5, -10); // Offset from the car

    void LateUpdate()
    {
        // Keep the camera at the specified offset position relative to the car
        transform.position = car.position + offset;

        // Optionally, keep the camera always looking at the car
        transform.LookAt(car);
    }
}
