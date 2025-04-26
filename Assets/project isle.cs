using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // For grabbing reset (optional)

public class PlantProjectile : MonoBehaviour
{
    public delegate void PlantLandedEvent(Vector3 landingPosition);
    public event PlantLandedEvent OnPlantLanded;

    private Rigidbody rb;
    private bool hasLanded = false;

    [Header("Throw Settings")]
    public float throwPower = 10f; // Configurable throw force in Inspector

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

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

    public void Throw(Vector3 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();

        hasLanded = false;
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero; // Reset velocity
        rb.AddForce(direction.normalized * throwPower, ForceMode.VelocityChange);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasLanded) return;

        hasLanded = true;
        rb.isKinematic = true; // Freeze when landed

        OnPlantLanded?.Invoke(transform.position); // Tell puller
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        rb.isKinematic = false; // Allow physics again
        hasLanded = false; // Reset so landing works again
    }
}