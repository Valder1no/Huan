using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    public GameObject objectToMove;        // The object you want to move
    public float moveSpeed = 5f;           // The speed at which the object will move

    private Camera mainCamera;             // Camera to use for raycasting (in case of VR, you'd track controller directly)
    
    // VR-specific controls can be added as needed
    // For now, mouse-based control will be implemented, with easy transition to VR controllers.

    void Start()
    {
        mainCamera = Camera.main;  // Get the main camera, which will help us with raycasting
    }

    void Update()
    {
        // Perform raycasting based on the input device
        Vector3 targetPosition = GetPointerPosition();
        MoveObject(targetPosition);

        // Check for input to trigger the action (click in this case)
        if (Input.GetMouseButtonDown(0))  // For mouse, left-click triggers the move.
        {
            // In VR, replace with a VR input check (e.g., trigger button on the controller)
            // Example for VR: if (controller.GetPressDown(VRInputButton.Trigger)) { MoveObject(targetPosition); }
            MoveObject(targetPosition);
        }
    }

    // Get the position where the pointer (mouse or VR controller) is pointing
    Vector3 GetPointerPosition()
    {
        // Raycast from the camera to where the pointer (mouse or controller) is pointing
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // This is for mouse

        // In VR, you can use a controller's position and direction instead of the mouse.
        // Example: Ray ray = new Ray(controller.position, controller.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;  // The point where the ray hits an object in the scene
        }

        // If nothing is hit, just return a default point ahead of the camera
        return ray.GetPoint(10f); // 10 units in front of the camera
    }

    // Move the object smoothly towards the target position
    void MoveObject(Vector3 targetPosition)
    {
        // Smoothly move the object towards the target position
        objectToMove.transform.position = Vector3.MoveTowards(objectToMove.transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
