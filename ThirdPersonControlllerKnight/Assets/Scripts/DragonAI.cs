using UnityEngine;
using UnityEngine.AI;

public class DragonAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    [SerializeField] private float health;
    [SerializeField] private float damage;
    [SerializeField] private float thresholdHealth;

    // Attacking
    private RaycastHit hit;
    [SerializeField] private float timeBetweenAttacks;
    private float timeSinceLastAttack;
    private float distanceToPlayer;
    private float groundTimer;
    [SerializeField] private float groundedTime;
    private float flyTimer;
    [SerializeField] private float airTime;
    [SerializeField] private float attackRange;
    [SerializeField] private bool alive, attacking, stage2, isFlying;
    [SerializeField] private int rotationSpeed;

    private void Awake()
    {
        player = GameObject.Find("PlayerArmature").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        // Ready for attack
        timeSinceLastAttack = timeBetweenAttacks;
        attacking = false;
        alive = true;
        stage2 = false;
    }

    private void Update()
    {
        // Make sure enemy is alive
        if (!alive)
        {
            return;
        }

        if (health < thresholdHealth)
        {
            stage2 = true;
            animator.SetBool("stage2", true);

            if (isFlying)
            {
                flyTimer += Time.deltaTime;
                if (flyTimer >= airTime)
                {
                    flyTimer = 0;

                    // land dragon so player can get attacks in
                    isFlying = false;
                    animator.SetBool("isFlying", false);

                }
            }
            else
            {
                groundTimer += Time.deltaTime;
                if (groundTimer >= groundedTime)
                {
                    groundTimer = 0;

                    // make dragon take off into the sky
                    isFlying = true;
                    animator.SetBool("isFlying", true);

                }
            }
        }
        
        
        timeSinceLastAttack += Time.deltaTime;
        // Check for attack range
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Rotate look direction towards player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

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
            TakeDamage(50);
        }
    }

    private void FollowPlayer()
    {
        // Set enemy's destination to player's position
        if (!attacking)
        {
            agent.SetDestination(player.position);
        }
        animator.SetBool("inRange", false);
    }

    private void AttackPlayer()
    {
        // Make sure enemy doesn't move when attacking player
        agent.SetDestination(transform.position);
        
        startAttack();
    }

    private void startAttack()
    {
        // Aim at enemy
        // Start attack (aiming at player) if not currently attacking for melee
        // Ranged characters can keep aiming at player while attacking
        if (attacking)
        {
            return;
        }

        animator.SetBool("inRange", true);
        if (timeSinceLastAttack >= timeBetweenAttacks)
        {
            animator.SetTrigger("attack");
            attacking = true;
        }
    }
    private void RaycastHit()
    {
        // Raycast for melee enemies
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
        {

            if (hit.collider.gameObject.CompareTag("Player"))
            {   
                // double damage in stage 2
                float calculatedDamage = stage2 ? damage * 2f : damage; // NEED DAMAGE ENGINE

                int roundedDamage = Mathf.RoundToInt(calculatedDamage);

                // Deal damage to the player
                player.GetComponent<PlayerController>()?.TakeDamage(roundedDamage);
            }
        }
    }
    private void endAttack()
    {
        attacking = false;
        animator.SetBool("inRange", false);
        timeSinceLastAttack = 0;
    }

    // TODO: DAMAGE IMPLEMENTATION
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            animator.SetBool("isAlive", false);
            
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
