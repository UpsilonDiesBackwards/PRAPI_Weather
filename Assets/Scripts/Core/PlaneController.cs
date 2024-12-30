using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public float speed = 20f;
    public float lift = 5f;
    public float turnSpeed = 50f;
    public float rollSpeed = 50f;
    private Rigidbody rb;

    private float pitch; // Pitch angle
    private float yaw;   // Yaw angle
    private float roll;  // Roll angle

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure the Rigidbody is not affected by gravity
        rb.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Get mouse input for pitch and yaw
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Adjust pitch and yaw based on mouse movement
        yaw += mouseX * turnSpeed * Time.deltaTime;
        pitch -= mouseY * turnSpeed * Time.deltaTime; // Inverted to match typical flight controls

        // Get input for rolling
        if (Input.GetKey(KeyCode.Q))
        {
            roll += rollSpeed * Time.deltaTime; // Roll left
        }
        else if (Input.GetKey(KeyCode.E))
        {
            roll -= rollSpeed * Time.deltaTime; // Roll right
        }
        // else
        // {
        //     roll = 0; // Reset roll if no key is pressed
        // }

        // Move the plane forward
        rb.AddForce(Vector3.forward * speed);

        //Slow down plane
        if (Input.GetKey(KeyCode.S))
        {

            // Move the plane backward
            rb.AddForce(Vector3.back * speed);

        }
        // Apply rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, roll);
        rb.MoveRotation(rotation);

        void FixedUpdate()
        {
            // Apply lift
            if (rb.velocity.magnitude > 0)
            {
                rb.AddForce(Vector3.up * lift);
            }
        }
    }
}