using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PlantProjectile : MonoBehaviour
{
    public delegate void PlantLandedEvent(Vector3 landingPosition);
    public event PlantLandedEvent OnPlantLanded;

    private Rigidbody rb;
    private bool hasLanded = false;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
        }
    }

    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        hasLanded = true;
        rb.isKinematic = true; // Freeze physics when landed

        OnPlantLanded?.Invoke(transform.position); // Notify listeners
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        rb.isKinematic = false; // Allow physics again when grabbed
        hasLanded = false; // Reset landing state
    }
}