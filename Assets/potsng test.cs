using UnityEngine;
public class ThrowableGrapple : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float pullSpeed = 5f;
    public float stopDistance = 1.5f;
    public LayerMask validHitLayers;

    [Header("Player References")]
    public CharacterController playerController; // XR Origin CharacterController
    public Transform playerTransform;            // XR Origin root

    [Header("Line Renderer (Optional)")]
    public LineRenderer lineRenderer;

    private Vector3 grapplePoint;
    private bool isGrappling = false;
    private bool hasHit = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Only register first valid hit
        if (hasHit) return;

        if (((1 << collision.gameObject.layer) & validHitLayers) == 0)
            return;

        hasHit = true;
        grapplePoint = collision.contacts[0].point;
        isGrappling = true;

        if (lineRenderer)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
        }
    }

    private void Update()
    {
        if (!isGrappling || playerController == null || playerTransform == null) return;

        Vector3 direction = (grapplePoint - playerTransform.position).normalized;
        float distance = Vector3.Distance(playerTransform.position, grapplePoint);

        playerController.Move(direction * pullSpeed * Time.deltaTime);

        if (lineRenderer)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, playerTransform.position);
        }

        if (distance < stopDistance)
        {
            StopGrapple();
        }
    }

    private void StopGrapple()
    {
        isGrappling = false;

        if (lineRenderer)
            lineRenderer.enabled = false;

        // Reset hit flag so you can reuse the object
        hasHit = false;
    }
}