using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class StayAtTransformUntilGrabbed : XRGrabInteractable
{
    [Header("Settings")]
    public Transform stayTransform;

    private bool isGrabbed = false;
    private Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogWarning("No Rigidbody found, adding one.");
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        isGrabbed = true;
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        // After release, remains a normal object
    }

    private void Update()
    {
        if (!isGrabbed && stayTransform != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = stayTransform.position;
            transform.rotation = stayTransform.rotation;
        }
    }
}