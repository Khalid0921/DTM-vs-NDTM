using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WASD : MonoBehaviour
{
    public float moveSpeed = 10f; // Speed of the car
    public float turnSpeed = 50f; // Turning speed
    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Ensure the car starts upright and doesn't flip unnecessarily
        rb.freezeRotation = true;
    }

    void Update()
    {
        // Get player input
        float moveInput = Input.GetAxis("Vertical"); // W/S for forward/backward
        float turnInput = Input.GetAxis("Horizontal"); // A/D for left/right

        // Move the car forward or backward
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + moveDirection);

        // Rotate the car left or right
        if (moveInput != 0) // Only allow turning while moving
        {
            Quaternion turnRotation = Quaternion.Euler(0, turnInput * turnSpeed * Time.fixedDeltaTime, 0);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}

