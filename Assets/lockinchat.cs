using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HipSlotStorage : MonoBehaviour
{
    [Header("Hip Slot Settings")]
    public Transform hipSlotTransform;
    public float snapDistance = 0.3f;

    private GameObject storedObject;
    private XRGrabInteractable grabInteractable;

    private void OnTriggerEnter(Collider other)
    {
        if (storedObject != null) return; // Only one object at a time

        XRGrabInteractable interactable = other.GetComponent<XRGrabInteractable>();
        if (interactable != null && !interactable.isSelected)
        {
            float distance = Vector3.Distance(interactable.transform.position, hipSlotTransform.position);
            if (distance <= snapDistance)
            {
                StoreObject(interactable);
            }
        }
    }

    private void StoreObject(XRGrabInteractable interactable)
    {
        storedObject = interactable.gameObject;
        grabInteractable = interactable;

        storedObject.transform.SetParent(hipSlotTransform);
        storedObject.transform.localPosition = Vector3.zero;
        storedObject.transform.localRotation = Quaternion.identity;

        // Disable physics
        Rigidbody rb = storedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Listen for grab
        grabInteractable.selectEntered.AddListener(OnObjectGrabbed);
    }

    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        if (storedObject == null) return;

        grabInteractable.selectEntered.RemoveListener(OnObjectGrabbed);

        // Detach
        storedObject.transform.SetParent(null);

        // Re-enable physics
        Rigidbody rb = storedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        storedObject = null;
        grabInteractable = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (hipSlotTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(hipSlotTransform.position, snapDistance);
        }
    }
}