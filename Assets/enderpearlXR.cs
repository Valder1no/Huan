using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class EnderPearl : XRGrabInteractable
{
    public GameObject teleportEffectPrefab; // Assign a teleport effect in the Inspector
    private Rigidbody rb;
    private bool hasLanded = false;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    // FIXED: Using correct method signature
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        if (!args.isCanceled) // Ensure the exit is not canceled
        {
            ThrowPearl();
        }
    }

    private void ThrowPearl()
    {
        if (rb)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            TeleportPlayer(collision.contacts[0].point);
        }
    }

    private void TeleportPlayer(Vector3 targetPosition)
    {
        GameObject xrOrigin = GameObject.FindWithTag("Player"); // Ensure XR Origin has the "Player" tag
        if (xrOrigin != null)
        {
            Vector3 teleportPosition = targetPosition + Vector3.up * 0.5f;

            // Play teleport effect at destination
            if (teleportEffectPrefab)
                Instantiate(teleportEffectPrefab, teleportPosition, Quaternion.identity);

            // Move the player
            xrOrigin.transform.position = teleportPosition;
        }

        Destroy(gameObject, 0.5f); // Destroy pearl after teleporting
    }
}