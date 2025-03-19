using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTile : MonoBehaviour
{
    [SerializeField] private float destroyTime = 8f; 
    [SerializeField] private float gravityFactor = 0.10f;
    [SerializeField] private float rotationForce = 0.5f; 
    
    private Rigidbody rb;
    private Vector3 moonGravity;
    
    void Start()
    {
        // Add Rigidbody for physics
        rb = gameObject.GetComponent<Rigidbody>();
        
        // Calculate moon gravity
        moonGravity = Physics.gravity * gravityFactor;
        
        // Add slight random rotation for more realistic floating effect
        float randomX = Random.Range(-rotationForce, rotationForce);
        float randomY = Random.Range(-rotationForce, rotationForce);
        float randomZ = Random.Range(-rotationForce, rotationForce);
        rb.AddTorque(new Vector3(randomX, randomY, randomZ), ForceMode.Impulse);
        
        // Destroy after longer time for moon effect
        Destroy(gameObject, destroyTime);
    }

    private void FixedUpdate()
    {
        // Apply moon gravity in FixedUpdate for consistent physics
        if (rb != null)
        {
            rb.AddForce(moonGravity, ForceMode.Acceleration);
        }
    }

    private void Update()
    {
        // Still destroy if it falls too far
        if (transform.position.y < -10f) // Increased from -4f to -10f to allow more floating
            Destroy(gameObject);
    }
}
