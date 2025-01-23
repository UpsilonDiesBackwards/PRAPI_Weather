using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private Rigidbody rb;
    public MenuManager menuManager;
    
    [Header("Properties")]
    public float speed = 20f;
    public float lift = 5f;
    public float turnSpeed = 50f;
    public float rollSpeed = 50f;

    private float pitch; // Pitch angle
    private float yaw;   // Yaw angle
    private float roll;  // Roll angle

    public bool hasCrashed = false;
    
    [Header("Propellers")]
    public GameObject propellerObject;
    public GameObject[] propellerTransforms;
    public float propellerRotationSpeed = 250f;

    [Header("Advanced Movement Stuff")] 
    public bool isStalling = false;
    public float threshold = 0.5f;
    public float slowDownFactor = 0.5f;
    public float stallSpeed;
    
    private Vector3 initialForward;

    private float mouseX, mouseY;

    void OnDrawGizmos() {
        if (!Startup.Instance.debugMode) return;
            
        // Draw debug rays for forward vectors
        Debug.DrawLine(transform.position, transform.forward * 20f, Color.red);
        Debug.DrawLine(transform.position, initialForward * 20f, Color.green);
    }
    
    private void Awake() {
        menuManager = FindObjectOfType<MenuManager>();
        menuManager.stallWarning.GetComponent<TextMeshProUGUI>().text = "!!! STALLING !!!";
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Ensure the Rigidbody is not affected by gravity
        rb.useGravity = false;

        Cursor.lockState = CursorLockMode.Locked;
        
        foreach (GameObject transform in propellerTransforms) {
            Instantiate(propellerObject, transform.transform);
        }

        initialForward = Vector3.forward;
        
        hasCrashed = false;
    }

    void Update() {
        if (hasCrashed) return;
        
        // Get mouse input for pitch and yaw
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

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

        // Apply rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, roll);
        transform.rotation = rotation;

        // Apply propeller rotations
        foreach (GameObject p in propellerTransforms) {
            p.transform.Rotate(0, 0, Time.deltaTime * 
                                     (propellerRotationSpeed * (rb.velocity.magnitude * 0.4f)));
        }
        
        // Draw debug rays for forward vectors
        Debug.DrawRay(transform.position, transform.forward * 20f, Color.red);
        Debug.DrawRay(transform.position, initialForward * 20f, Color.green);
    }
    
    void FixedUpdate() {
        // Calculate Dot Product
        float dotProd = Vector3.Dot(initialForward, transform.forward);
        if (dotProd <= threshold) { // Then enable gravity and lower velocity to act as "stalling"
            rb.velocity = new Vector3(rb.velocity.x * slowDownFactor, rb.velocity.y, rb.velocity.z);

            rb.useGravity = true;
            rb.AddForce(Vector3.down * stallSpeed, ForceMode.Force);

            isStalling = true;
        } else { // Disable le gravity
            rb.useGravity = false;
            isStalling = false;
        }

        if (Input.GetKey(KeyCode.Q)) { // When the player presses `Q` and `E` ...
            rb.AddForce(-transform.right * speed * 0.75f, ForceMode.Force); // ... The plane should have a lil force applied
            rb.velocity *= 0.98f;
            yaw += mouseX * turnSpeed * Time.deltaTime; 
        } else if (Input.GetKey(KeyCode.E)) {
            rb.AddForce(transform.right * speed * 0.75f, ForceMode.Force);
            rb.velocity *= 0.98f;;
            yaw += mouseX * turnSpeed * Time.deltaTime;
        }
        
        if (Input.GetKey(KeyCode.Space) && rb.velocity.magnitude > 0) { // Apply lift
            rb.AddForce(Vector3.up * lift, ForceMode.Force);
        }
        
        if (Input.GetKey(KeyCode.S)) //Slow down plane
        {
            rb.AddForce(-transform.forward * speed, ForceMode.Force);
        }
        
        // Move the plane forward
        rb.AddForce(transform.forward * speed, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Terrain") {
            hasCrashed = true;
        }
    }
}
