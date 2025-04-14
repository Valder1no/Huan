
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

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
    public Transform attackPoint; // A point from which the projectile spawns

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
        // Check if the player is within sight and attack range
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

        if(rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToPlayer = (player.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToPlayer) < angle / 2)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);

                        // Raycast from enemy to player
        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, distanceToPlayer))
        {
            // If the first thing hit is the player, they are in sight
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
            }
        }

        return false; // Player is behind a wall or object
    }


    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            GetComponent<NavMeshAgent>().speed = 1.6f;
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 2.5f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (!Physics.Raycast(transform.position, (walkPoint - transform.position).normalized, out RaycastHit hit, Vector3.Distance(transform.position, walkPoint)))
        {
            // If raycast does NOT hit anything, set it as the new walkPoint
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
            // Spawn and throw projectile
            agent.SetDestination(transform.position);
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

            if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
        }
        private void DestroyEnemy()
        {
            Destroy(gameObject);
    }

}