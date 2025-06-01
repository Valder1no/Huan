using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public string attractableTag = "Magnetic";           // Tag for objects that can be attracted
    public float magnetRange = 5f;                       // Range of the magnet effect
    public float magnetForce = 10f;                      // Strength of the magnetic force
    public LayerMask attractableLayers = -1;             // Which layers can be attracted (optional filter)

    [Header("VR Controller")]
    public bool useRightController = true;               // True for right controller, false for left

    [Header("Visual Effects (Optional)")]
    public GameObject magnetEffectPrefab;                // Visual effect when magnet is active
    public Transform magnetCenter;                       // Point where magnetic force originates (if null, uses this transform)

    private InputDevice targetController;
    private bool isMagnetActive = false;
    private GameObject currentMagnetEffect;
    private List<Rigidbody> attractableObjects = new List<Rigidbody>();

    void Start()
    {
        InitializeController();

        // Use this transform as magnet center if none specified
        if (magnetCenter == null)
            magnetCenter = transform;
    }

    void InitializeController()
    {
        var controllers = new List<InputDevice>();
        InputDeviceRole targetRole = useRightController ? InputDeviceRole.RightHanded : InputDeviceRole.LeftHanded;
        InputDevices.GetDevicesWithRole(targetRole, controllers);

        if (controllers.Count > 0)
        {
            targetController = controllers[0];
        }
    }

    void Update()
    {
        // Re-initialize controller if it becomes invalid
        if (!targetController.isValid)
        {
            InitializeController();
            return;
        }

        // Check trigger state
        bool triggerPressed;
        if (targetController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed))
        {
            if (triggerPressed && !isMagnetActive)
            {
                ActivateMagnet();
            }
            else if (!triggerPressed && isMagnetActive)
            {
                DeactivateMagnet();
            }
        }

        // Apply magnetic force if active
        if (isMagnetActive)
        {
            ApplyMagneticForce();
        }
    }

    void ActivateMagnet()
    {
        isMagnetActive = true;
        FindAttractableObjects();

        // Spawn visual effect if available
        if (magnetEffectPrefab != null && currentMagnetEffect == null)
        {
            currentMagnetEffect = Instantiate(magnetEffectPrefab, magnetCenter.position, magnetCenter.rotation);
            currentMagnetEffect.transform.SetParent(magnetCenter);
        }

        Debug.Log("Magnet Activated!");
    }

    void DeactivateMagnet()
    {
        isMagnetActive = false;
        attractableObjects.Clear();

        // Destroy visual effect
        if (currentMagnetEffect != null)
        {
            Destroy(currentMagnetEffect);
            currentMagnetEffect = null;
        }

        Debug.Log("Magnet Deactivated!");
    }

    void FindAttractableObjects()
    {
        attractableObjects.Clear();

        // Find all colliders within range
        Collider[] nearbyColliders = Physics.OverlapSphere(magnetCenter.position, magnetRange, attractableLayers);

        foreach (Collider col in nearbyColliders)
        {
            // Check if object has the correct tag and a Rigidbody
            if (col.CompareTag(attractableTag))
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null && !attractableObjects.Contains(rb))
                {
                    attractableObjects.Add(rb);
                }
            }
        }

        Debug.Log($"Found {attractableObjects.Count} attractable objects");
    }

    void ApplyMagneticForce()
    {
        // Refresh the list periodically to catch new objects entering range
        if (Time.fixedTime % 0.1f < Time.fixedDeltaTime) // Every 0.1 seconds
        {
            FindAttractableObjects();
        }

        foreach (Rigidbody rb in attractableObjects)
        {
            if (rb == null) continue;

            // Calculate distance and direction
            Vector3 direction = magnetCenter.position - rb.transform.position;
            float distance = direction.magnitude;

            // Skip if object is too far (safety check)
            if (distance > magnetRange) continue;

            // Calculate force based on distance (stronger when closer)
            float forceMagnitude = magnetForce / (1 + distance);
            Vector3 force = direction.normalized * forceMagnitude;

            // Apply force
            rb.AddForce(force, ForceMode.Force);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw magnet range in scene view
        if (magnetCenter == null) magnetCenter = transform;

        Gizmos.color = isMagnetActive ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(magnetCenter.position, magnetRange);

        // Draw line to attracted objects when active
        if (isMagnetActive && attractableObjects != null)
        {
            Gizmos.color = Color.red;
            foreach (Rigidbody rb in attractableObjects)
            {
                if (rb != null)
                {
                    Gizmos.DrawLine(magnetCenter.position, rb.transform.position);
                }
            }
        }
    }
}