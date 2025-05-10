using UnityEngine;

public class DestroyProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem dieParticles;

    private ParticleSystem dieParticlesInstance;
    public GameObject gameObject;

    private void Start()
    {
        GetComponent<Collider>().enabled = false;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("WhatIsProjectile"), LayerMask.NameToLayer("WhatIsProjectile"));
        Invoke(nameof(EnableCollision), 0.1f);
    }

    private void EnableCollision()
    {
        GetComponent<Collider>().enabled = true;
    }

    private void dieParticlesFunction()
    {
        dieParticlesInstance = Instantiate(dieParticles, transform.position, Quaternion.Euler(-90f, 0f, 0f));
    }

    void OnCollisionEnter(Collision collision)
    {
        dieParticlesFunction();
        Debug.Log("Collided with: " + collision.gameObject.name);
        Destroy(gameObject);
    }
}
