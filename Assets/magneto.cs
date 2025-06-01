using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPuller : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionReference pullAction; // Reference to the button input

    [Header("Pull Settings")]
    public float pullRadius = 5f;
    public float pullForce = 10f;
    public LayerMask pullableLayer; // Only pull objects on this layer

    private void OnEnable()
    {
        if (pullAction != null)
            pullAction.action.performed += OnPullPerformed;
    }

    private void OnDisable()
    {
        if (pullAction != null)
            pullAction.action.performed -= OnPullPerformed;
    }

    private void OnPullPerformed(InputAction.CallbackContext context)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, pullRadius, pullableLayer);
        foreach (Collider col in colliders)
        {
            Rigidbody rb = col.attachedRigidbody;
            if (rb != null)
            {
                Vector3 direction = (transform.position - rb.position).normalized;
                rb.AddForce(direction * pullForce, ForceMode.Impulse);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Show the pull radius in the editor
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pullRadius);
    }
}