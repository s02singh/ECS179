using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private float health;

    // Attacking
    [SerializeField] private float timeBetweenAttacks;
    private float timeSinceLastAttack;
    private float distanceToPlayer;
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInAttackRange, animated, alive;
    [SerializeField] private int rotationSpeed;

    private void Awake()
    {
        player = GameObject.Find("PlayerArmature").transform;
        agent = GetComponent<NavMeshAgent>();
        if (animated)
        {
            animator = GetComponent<Animator>();
        }
        alive = true;
        // Ready for attack
        timeSinceLastAttack = timeBetweenAttacks;
    }

    private void Update()
    {
        // Make sure enemy is alive
        if (!alive)
        {
            return;
        }
        
        timeSinceLastAttack += Time.deltaTime;
        // Check for attack range
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < attackRange)
        {
            AttackPlayer();
        }

        if (distanceToPlayer >= attackRange)
        {
            FollowPlayer();
        }

        // TESTING DEATH ANIMATIONS // REMOVE WHEN DAMAGE IMPLEMENTED
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(100);
        }
    }

    private void FollowPlayer()
    {
        // Set enemy's destination to player's position
        agent.SetDestination(player.position);
        if (animated)
        {
            animator.SetBool("inRange", false);
        }
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move when attacking player
        agent.SetDestination(transform.position);

        // Aim at enemy
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        if (animated)
        {
            animator.SetBool("inRange", true);
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                animator.SetTrigger("attack");
                timeSinceLastAttack = 0;
            }
        }
    }

    // TODO: DAMAGE IMPLEMENTATION
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            if (animated)
            {
                animator.SetBool("isAlive", false);
            }
            agent.enabled = false;
            alive = false;
            Destroy(gameObject, 5f);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
