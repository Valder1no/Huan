using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 20f; // Start with 20 health (i.e., 20 hits to die)

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("projectile stuff")]
    public GameObject projectilePrefab;
    public Transform attackPoint;

    [Range(0, 360)]
    public float angle;
    public float radius;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (playerInSightRange)
        {
            if (HasLineOfSight())
            {
                if (playerInAttackRange)
                    AttackPlayer();
                else
                    ChasePlayer();
            }
            else
            {
                Patroling();
            }
        }
        else
        {
            Patroling();
        }
    }

    private bool HasLineOfSight()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, whatIsPlayer);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToPlayer) < angle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer))
                {
                    if (hit.transform.CompareTag("Player"))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
        {
            GetComponent<NavMeshAgent>().speed = 1.6f;
            agent.SetDestination(walkPoint);
        }

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 2.5f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (!Physics.Raycast(transform.position, (walkPoint - transform.position).normalized, out RaycastHit hit, Vector3.Distance(transform.position, walkPoint)))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        GetComponent<NavMeshAgent>().speed = 4f;
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            GameObject projectile = Instantiate(projectilePrefab, attackPoint.position, Quaternion.identity);
            projectile.GetComponent<Projectile>().SetTarget(player);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    // NEW: Detect hits from PlayerAttack-tagged GameObjects
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerAttack"))
        {
            TakeDamage(1); // Each hit reduces 1 health
            Destroy(other.gameObject); // Optional: destroy the attack object
        }
    }
}
