using UnityEngine;

public class PlantPuller : MonoBehaviour
{
    public float pullSpeed = 5f; // Configurable pull speed
    private Vector3 targetPosition;
    private bool isPulling = false;

    void Start()
    {
        // Subscribe to the plant's event dynamically
        PlantProjectile plant = Object.FindAnyObjectByType<PlantProjectile>();
        if (plant != null)
        {
            plant.OnPlantLanded += StartPulling;
        }
    }

    void Update()
    {
        if (!isPulling) return;

        // Move player towards the plant landing position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, pullSpeed * Time.deltaTime);

        // Stop pulling when close enough
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isPulling = false;
        }
    }

    private void StartPulling(Vector3 landingPosition)
    {
        targetPosition = landingPosition;
        isPulling = true;
    }
}