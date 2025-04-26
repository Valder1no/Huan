using UnityEngine;
using UnityEngine.InputSystem;

public class SetNonKinematicOnAButton : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Input Action")]
    public InputActionReference aButtonAction; // Reference to A button action (create in Input Actions)

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (aButtonAction != null)
            aButtonAction.action.performed += OnAPressed;
    }

    void OnDisable()
    {
        if (aButtonAction != null)
            aButtonAction.action.performed -= OnAPressed;
    }

    private void OnAPressed(InputAction.CallbackContext context)
    {
        SetNonKinematic();
    }

    private void SetNonKinematic()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            Debug.Log("Rigidbody set to non-kinematic on A button press!");
        }
    }
}