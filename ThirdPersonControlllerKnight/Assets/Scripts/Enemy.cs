using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [SerializeField]
    private Animator enemyAnim;

    [SerializeField]
    private Transform playerTransform; // Reference to player game object

    [SerializeField]
    private float attackRange = 2f; // Distance to trigger attack

    [SerializeField]
    private float chaseRange = 10f; // Distance to trigger chase

    [SerializeField]
    private float patrolRange = 5f; // Distance to roam around patrol points

    [SerializeField]
    private Transform[] patrolPoints; // Array of patrol points. I still need to make this

    private int currentPatrolPoint = 0;

    private NavMeshAgent agent; // Reference to NavMeshAgent component

    private RaycastHit hit;

    private bool isAttacking = false;
    private bool isChasing = false;

    private float nextAttackTime = 0f;
    private bool isBlocking = false;

    private void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        // Check if player is alive
        if (playerTransform == null)
            return;

        // Check if player is in attack range
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= attackRange)
        {
            enemyAnim.SetBool("Run", false);
            if (Random.Range(0, 100) < 5 && !isAttacking) // 5% chance to block, will tweak later
            {
                isBlocking = true;
                enemyAnim.SetBool("Block", true);
            }
            else if (isBlocking && Random.Range(0, 100) < 80) // 80% chance to continue blocking, again will be tweaked
            {
                isBlocking = false;
                enemyAnim.SetBool("Block", false);
            }
            else if(!isAttacking)
            {
                Attack();
            }
            

        }
        else if (distance <= chaseRange)
        {
            Chase();
        }
        else
        {
            Patrol();
        }

        if (Time.time >= nextAttackTime)
        {
            Debug.Log("reset");
            ResetAttack();
        }

        
    }

    private void Attack()
    {
        if (isAttacking)
        {
            Debug.Log("return");
            return;
        }
        nextAttackTime = Time.time + Random.Range(1f, 2f);
        Debug.Log(nextAttackTime);
        // Stop movement
        agent.isStopped = true;
     

        // Face player
        transform.LookAt(playerTransform);

        // Check if player is in front
        if (Vector3.Dot(transform.forward, (playerTransform.position - transform.position).normalized) > 0.5f)
        {
            enemyAnim.SetTrigger("Attack1"); // Trigger attack animation
          
            isAttacking = true;
  
        }
    }


    public void EnemyRaycast()
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, attackRange))
        {

            if (hit.collider.gameObject.CompareTag("Player"))
            {
                playerTransform.GetComponent<PlayerController>()?.TakeDamage(20); // NEED DAMAGE ENGINE
            }
        }

    }

    private void Chase()
    {
     
        agent.isStopped = false;

        // Set player as destination
        enemyAnim.SetBool("Run", true);
        agent.SetDestination(playerTransform.position);
    }

    private void Patrol()
    {
        
        isChasing = false;
        agent.isStopped = false;

        // Move to patrol point
        agent.SetDestination(patrolPoints[currentPatrolPoint].position);

        // Check if reached 
        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) <= 1f)
        {
            // Choose next patrol point randomly
            currentPatrolPoint = Random.Range(0, patrolPoints.Length);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth);
        nextAttackTime = Time.time + Random.Range(1f, 2f);

        if (currentHealth <= 0)
        {
            // Enemy dies
            Destroy(gameObject); 
        }
        else
        {
            enemyAnim.SetTrigger("Hit");
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }
}
