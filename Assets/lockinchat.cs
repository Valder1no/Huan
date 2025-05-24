using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class LockOnTarget : MonoBehaviour
{
    [Header("Lock-On Settings")]
    public Transform lockTarget;               // The transform to lock onto
    public float lockOnDistance = 0.1f;        // Distance threshold to lock on

    private XRGrabInteractable grabInteractable;
    private bool isLockedOn = false;
    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void Update()
    {
        if (!isGrabbed && !isLockedOn && Vector3.Distance(transform.position, lockTarget.position) <= lockOnDistance)
        {
            LockToTarget();
        }
        else if (isGrabbed && isLockedOn)
        {
            isLockedOn = false;
        }
    }

    private void LockToTarget()
    {
        transform.position = lockTarget.position;
        transform.rotation = lockTarget.rotation;
        isLockedOn = true;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }
}