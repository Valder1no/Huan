using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SuspendUntilGrabbed : MonoBehaviour
{
    public Transform suspendPoint; // Assignable Transform where the object will be suspended
    public GameObject objectPrefab; // Assignable prefab to clone when grabbed
    private Rigidbody rb;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;

    public bool IsGrabbed { get => isGrabbed; set => isGrabbed = value; }

    public SuspendUntilGrabbed(bool isGrabbed)
    {
        this.isGrabbed = isGrabbed;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (!TryGetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>(out grabInteractable))
        {
            Debug.LogError("XRGrabInteractable component is required but missing.", this);
            return;
        }

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        SuspendObject();
    }

    private void SuspendObject()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
            transform.SetPositionAndRotation(suspendPoint.position, suspendPoint.rotation);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        CloneObject();
    }

    private void CloneObject()
    {
        if (objectPrefab != null)
        {
            GameObject clone = Instantiate(objectPrefab, suspendPoint.position, suspendPoint.rotation);
            clone.GetComponent<SuspendUntilGrabbed>().SuspendObject();
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);
    }
}
