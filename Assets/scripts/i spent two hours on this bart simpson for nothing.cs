using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class SphereLauncher : MonoBehaviour
{
    public GameObject cloneSpherePrefab;  // The prefab for the sphere (assigned in the Inspector)
    public float launchForce = 10f;      // Force with which the sphere is launched
    public float despawnTime = 5f;       // Time (in seconds) after which the sphere will despawn
    public GameObject sigmaboy;

    private Camera mainCamera;
    private InputDevice rightController;

    void Start()
    {
        mainCamera = Camera.main;
        InitializeRightController();
    }

    void InitializeRightController()
    {
        var rightHandedControllers = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(InputDeviceRole.RightHanded, rightHandedControllers);

        if (rightHandedControllers.Count > 0)
        {
            rightController = rightHandedControllers[0];
        }
    }


    void Update()
    {
        if (rightController.isValid)
        {
            bool triggerValue;
            if (rightController.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue) && triggerValue)
            {
                LaunchSphere();
            }
        }
        else
        {
            InitializeRightController(); // Re-initialize if the controller becomes invalid
        }
    }

    void LaunchSphere()
    {
        // Create the cloned sphere at a position in front of the camera
        Vector3 spawnPosition = sigmaboy.transform.position + sigmaboy.transform.forward * 0.1f;
        GameObject clonedSphere = Instantiate(cloneSpherePrefab, spawnPosition, Quaternion.identity);

        // Apply a force in the direction the camera is facing
        Rigidbody rb = clonedSphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(sigmaboy.transform.forward * launchForce, ForceMode.VelocityChange);
        }

        // Destroy the cloned sphere after the specified despawn time
        Destroy(clonedSphere, despawnTime);
    }
}
