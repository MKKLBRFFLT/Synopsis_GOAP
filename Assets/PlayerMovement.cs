using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f; // Degrees per second

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false; // Ensure the Rigidbody is not kinematic
        rb.interpolation = RigidbodyInterpolation.Interpolate; // For smoother movement
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Better collision detection
    }

    private void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Get input from WASD or arrow keys
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create a new vector for movement
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical).normalized * moveSpeed;

        // Apply the movement vector to the Rigidbody's velocity
        rb.velocity = movement;

        // Rotate the player to face the direction of movement
        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
