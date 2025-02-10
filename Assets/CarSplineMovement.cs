using UnityEngine;

public class CarSplineMovement : MonoBehaviour
{
    [Tooltip("Speed at which the car moves (units per second).")]
    public float speed = 10f;

    void Update()
    {
        float moveDirection = 0f;
        
        // Check for player input.
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveDirection = -1f;
        }
        
        // Calculate the movement vector along the car's forward axis.
        Vector3 movement = transform.forward * moveDirection * speed * Time.deltaTime;
        
        // Apply the movement.
        transform.position += movement;
    }
}
