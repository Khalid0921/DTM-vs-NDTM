using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float acceleration = 10f; // Acceleration speed
    public float maxSpeed = 20f;     // Maximum speed of the car
    public float rotationSpeed = 10f; // Speed at which the car rotates
    private Rigidbody rb;            // Reference to the car's Rigidbody

    void Start()
    {
        // Get the Rigidbody component attached to the car
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get input from Horizontal (A/D or Left/Right) and Vertical (W/S or Up/Down)
        float moveHorizontal = Input.GetAxis("Horizontal"); // Left/Right
        float moveVertical = Input.GetAxis("Vertical");     // Up/Down

        // Create a movement vector based on input
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        // Check if the car's current speed is below the max speed
        if (rb.velocity.magnitude < maxSpeed)
        {
            // Apply force in the direction of the movement vector
            rb.AddForce(movement * acceleration, ForceMode.Acceleration);
        }

        // Rotate the car to face the movement direction
        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            rb.rotation = Quaternion.Lerp(rb.rotation, toRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
