using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class SphereLauncher : MonoBehaviour
{
    public GameObject cloneSpherePrefab;  // The prefab for the sphere (assigned in the Inspector)
    //public float launchForce;      // Force with which the sphere is launched
    public float despawnTime;       // Time (in seconds) after which the sphere will despawn
    public GameObject sigmaboy;

    public GameObject flameEffectPrefab;

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
        float launchForce = Random.Range(9f, 14f);
        Vector3 spawnPosition = sigmaboy.transform.position + sigmaboy.transform.forward * 0.1f;

        GameObject clonedSphere = Instantiate(cloneSpherePrefab, spawnPosition, Quaternion.identity);

        Rigidbody rb = clonedSphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(sigmaboy.transform.forward * launchForce, ForceMode.VelocityChange);
        }

        // Fire visual effect
        Quaternion flameRotation = Quaternion.LookRotation(sigmaboy.transform.forward);
        GameObject flameEffect = Instantiate(flameEffectPrefab, spawnPosition, flameRotation);

        flameEffect.transform.parent = clonedSphere.transform; // Optional
        Destroy(flameEffect, 1.5f);
        Destroy(clonedSphere, despawnTime = Random.Range(0.5f, 0.9f));
    }
}
