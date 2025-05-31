using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private LayerMask attractableLayer = 1;
    [SerializeField] private float magnetRadius = 5f;
    [SerializeField] private float magnetForce = 10f;
    [SerializeField] private float stopDistance = 0.5f;
    [SerializeField] private bool useGravity = true;

    [Header("Visual Feedback")]
    [SerializeField] private bool showDebugSphere = true;
    [SerializeField] private Color debugColor = Color.red;

    private List<Rigidbody> attractedObjects = new List<Rigidbody>();

    void Update()
    {
        AttractObjects();
    }

    void AttractObjects()
    {
        // Find all objects in range on the specified layer
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, magnetRadius, attractableLayer);

        // Clear the list and repopulate
        attractedObjects.Clear();

        foreach (Collider col in objectsInRange)
        {
            Rigidbody rb = col.GetComponent<Rigidbody>();
            if (rb != null)
            {
                attractedObjects.Add(rb);
                PullObject(rb);
            }
        }
    }

    void PullObject(Rigidbody targetRb)
    {
        Vector3 direction = transform.position - targetRb.transform.position;
        float distance = direction.magnitude;

        // Stop pulling when close enough
        if (distance <= stopDistance)
        {
            // Optional: Stop the object completely when it reaches the magnet
            targetRb.velocity = Vector3.zero;
            targetRb.angularVelocity = Vector3.zero;
            return;
        }

        // Calculate force based on distance (stronger when closer)
        float forceMagnitude = magnetForce / (distance * distance);
        Vector3 force = direction.normalized * forceMagnitude;

        // Apply the magnetic force
        targetRb.AddForce(force, ForceMode.Force);

        // Optionally disable gravity for attracted objects
        if (!useGravity)
        {
            targetRb.useGravity = false;
        }
    }

    // Visual debugging
    void OnDrawGizmosSelected()
    {
        if (showDebugSphere)
        {
            Gizmos.color = debugColor;
            Gizmos.DrawWireSphere(transform.position, magnetRadius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, stopDistance);
        }
    }

    // Optional: Method to toggle magnet on/off (useful for VR interactions)
    public void ToggleMagnet()
    {
        enabled = !enabled;
    }

    public void SetMagnetActive(bool active)
    {
        enabled = active;
    }
}